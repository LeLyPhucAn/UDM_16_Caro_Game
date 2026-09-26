using System;
using System.Drawing;
using System.Windows.Forms;
using Client.Network;       // Giao tiếp mạng
using CaroGame.Protocol;    // Gói tin Protocol
using CaroGame.Protocol.Messages;
using Client.Controls;      // Dùng UserControl BoardControl
using CaroGame.Protocol.Messages.Game;
using CaroGame.Protocol.Messages.Response;

namespace Client.Forms
{
    public partial class GameForm : Form
    {
        // Quản lý Mạng, Bàn cờ và Trạng thái ván đấu
        private ClientConnection _clientConnection;
        private BoardControl _boardControl = null!;

        private bool _isMyTurn = false; // Mặc định khóa bàn cờ, chờ Server cấp quyền
        private string _mySymbol = "";

        // BỔ SUNG: Khai báo đủ các biến lưu trữ
        private string _roomId;
        private string _roomName;
        private string _playerName;
        private bool _isHost;
        private int _boardSize;
        private bool _isSpectator;

        private const int TURN_TIMEOUT_SECONDS = 60;
        private const int GRACE_PERIOD_SECONDS = 90;
        private const int REMATCH_PROMPT_DELAY_MS = 5000; // Đợi 5 giây trước khi hỏi tái đấu

        private bool _gameEnded = false;

        private Button? _btnSurrender;
        private Button? _btnDraw;
        private System.Windows.Forms.Timer _clientTimer = new System.Windows.Forms.Timer();
        private int _remainingSeconds = TURN_TIMEOUT_SECONDS;

        // Reconnect: Timer đếm ngược grace period trên UI
        private System.Windows.Forms.Timer _reconnectCountdownTimer = new System.Windows.Forms.Timer();
        private int _reconnectSecondsLeft = 0;
        private Label? _lblReconnectStatus = null;

        // Victory effect
        private Label? _lblVictory;
        private System.Windows.Forms.Timer? _confettiTimer;
        private List<PointF> _confettiParticles = new List<PointF>();
        private List<float> _confettiSpeeds = new List<float>();

        public GameForm(ClientConnection connection, string roomId, string roomName, string playerName, bool isHost, int boardSize = 15, bool isSpectator = false)
        {
            InitializeComponent();
            var _ = this.Handle; // Bắt buộc tạo Window Handle ngay lập tức để InvokeRequired hoạt động an toàn

            _clientConnection = connection;
            _roomId = roomId;
            _roomName = roomName;
            _playerName = playerName;
            _isHost = isHost;
            _boardSize = boardSize;
            _isSpectator = isSpectator;

            this.Text = "Caro Arena - " + _roomName;

            // Khoi tao ban co ngay trong constructor de luon san sang nhan goi tin dong bo
            SetupBoardControl();

            // Đăng ký nhận tin nhắn khi đang trong phòng chơi
            _clientConnection.OnMessageReceived += HandleGameMessage;
        }

        public void InitGameState(GameStateMessage syncMsg)
        {
            HandleGameMessage(syncMsg);
        }

        private void GameForm_Load(object sender, EventArgs e)
        {

            // Cập nhật nhãn kích thước
            if (lblBadge != null)
            {
                lblBadge.Text = $"Kích cỡ: {_boardSize}x{_boardSize}";
            }

            if (_isSpectator)
            {
                lblTurnValue.Text = "Đang xem...";
                _isMyTurn = false;
            }

            // Gán sự kiện cho chat

            // Setup buttons
            if (!_isSpectator)
            {
                _btnSurrender = new Button
                {
                    Text = "ĐẦU HÀNG",
                    Size = new Size(120, 35),
                    Location = new Point(btnLeaveRoom.Location.X - 130, btnLeaveRoom.Location.Y),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    BackColor = Color.OrangeRed,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                _btnSurrender.FlatAppearance.BorderSize = 0;
                _btnSurrender.Click += (s, e) =>
                {
                    if (MessageBox.Show("Bạn có chắc chắn muốn ĐẦU HÀNG?", "Đầu hàng", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        var req = new CaroGame.Protocol.Messages.RequestMessage { SenderId = _playerName, Action = "Surrender", Data = _roomId };
                        _ = _clientConnection.SendMessageAsync(req);
                    }
                };
                pnlTop.Controls.Add(_btnSurrender);

                _btnDraw = new Button
                {
                    Text = "CẦU HÒA",
                    Size = new Size(120, 35),
                    Location = new Point(btnLeaveRoom.Location.X - 260, btnLeaveRoom.Location.Y),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    BackColor = Color.SteelBlue,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                _btnDraw.FlatAppearance.BorderSize = 0;
                _btnDraw.Click += (s, e) =>
                {
                    if (MessageBox.Show("Bạn muốn gửi yêu cầu CẦU HÒA?", "Cầu hòa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        var req = new CaroGame.Protocol.Messages.RequestMessage { SenderId = _playerName, Action = "DrawRequest", Data = _roomId };
                        _ = _clientConnection.SendMessageAsync(req);
                    }
                };
                pnlTop.Controls.Add(_btnDraw);
            }



            btnSend.Click += BtnSend_Click;
            txtChatInput.KeyDown += TxtChatInput_KeyDown;

            // Cài đặt Timer
            _clientTimer.Interval = 1000; // 1 giây
            _clientTimer.Tick += ClientTimer_Tick;
        }

        private void ClientTimer_Tick(object? sender, EventArgs e)
        {
            _remainingSeconds--;
            if (_remainingSeconds <= 0)
            {
                _remainingSeconds = 0;
                _clientTimer.Stop(); // Hết giờ thì dừng đếm
            }
            // Cập nhật UI an toàn trên form thread
            UpdateTimerUI(_remainingSeconds);
        }

        private void UpdateTimerUI(int seconds)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateTimerUI(seconds)));
                return;
            }
            if (lblTimerValue != null)
                lblTimerValue.Text = seconds + "s";
        }
        // PHẦN 1: TẠO BÀN CỜ & GỬI DỮ LIỆU

        private void SetupBoardControl()
        {
            _boardControl = new BoardControl();
            _boardControl.InitializeBoard(_boardSize);

            // Lắng nghe sự kiện click từ BoardControl để gửi mạng
            _boardControl.OnCellClicked += async (row, col) =>
            {
                if (!_isMyTurn) return; // Chưa đến lượt

                var moveMsg = new MoveMessage
                {
                    RoomId = _roomId,
                    Row = row,
                    Column = col,
                    Symbol = _mySymbol
                };

                try
                {
                    await _clientConnection.SendMessageAsync(moveMsg);
                    _isMyTurn = false; // Khóa bàn cờ ngay lập tức để chờ Server phản hồi
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi gửi nước đi: " + ex.Message);
                }
            };

            // Gắn bàn cờ vào Panel hiện có trên Giao diện
            pnlBoard.Controls.Clear();
            pnlBoard.Controls.Add(_boardControl);
        }
        // PHẦN 3: GIAO TIẾP SERVER TRONG GAME & THOÁT

        private void HandleGameMessage(BaseMessage message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => HandleGameMessage(message)));
                return;
            }

            try
            {
                if (_boardControl == null)
                {
                    SetupBoardControl();
                }
                // 1. XỬ LÝ LÚC VỪA VÀO PHÒNG
                if (message.Type == MessageType.GameState && message is GameStateMessage syncMsg)
                {
                    // Tái đấu Bot: Reset bàn cờ và UI kết thúc trận (nếu có)
                    _boardControl?.InitializeBoard(syncMsg.BoardSize > 0 ? syncMsg.BoardSize : _boardSize);
                    if (lblTurnValue != null)
                    {
                        lblTurnValue.BackColor = Color.Transparent;
                        lblTurnValue.ForeColor = Color.White;
                    }
                    HideVictoryEffect();

                    // Cập nhật giao diện Label
                    lblPlayerX.Text = $"X: {syncMsg.PlayerXName}";
                    lblPlayerO.Text = $"O: {syncMsg.PlayerOName}";

                    // Cap nhat nhan vai tro Chu phong va Khach tren giao dien VS
                    bool pXIsHost = false;
                    if (_isHost)
                    {
                        pXIsHost = (syncMsg.PlayerXName == _playerName);
                    }
                    else if (!_isSpectator)
                    {
                        pXIsHost = (syncMsg.PlayerOName == _playerName);
                    }
                    else
                    {
                        pXIsHost = _roomName.Contains(syncMsg.PlayerXName, StringComparison.OrdinalIgnoreCase);
                    }

                    if (lblPlayerX_Status != null)
                    {
                        lblPlayerX_Status.Text = pXIsHost ? "Chủ phòng" : "Khách";
                        lblPlayerX_Status.ForeColor = pXIsHost ? Color.LimeGreen : Color.LightSkyBlue;
                    }
                    if (lblPlayerO_Status != null)
                    {
                        lblPlayerO_Status.Text = pXIsHost ? "Khách" : "Chủ phòng";
                        lblPlayerO_Status.ForeColor = pXIsHost ? Color.LightSkyBlue : Color.LimeGreen;
                    }

                    // Lưu ký hiệu của mình (X, O hoặc S)
                    _mySymbol = syncMsg.MySymbol;
                    if (_mySymbol == "S")
                    {
                        _isSpectator = true;
                        _isMyTurn = false;
                    }
                    else
                    {
                        if (syncMsg.CurrentTurnName == _playerName) _isMyTurn = true;
                    }

                    // Cập nhật giao diện lượt đi ban đầu
                    string turnSymbol = (syncMsg.CurrentTurnName == syncMsg.PlayerXName) ? "X" : "O";
                    SetTurnUI(turnSymbol, syncMsg.CurrentTurnName);

                    // Cập nhật số khán giả
                    if (lblSpectators != null)
                    {
                        lblSpectators.Text = $"Khán giả: {syncMsg.SpectatorCount}";
                    }

                    // Vẽ lại bàn cờ nếu có dữ liệu các nước đi trước
                    if (!string.IsNullOrEmpty(syncMsg.BoardState))
                    {
                        int size = syncMsg.BoardSize > 0 ? syncMsg.BoardSize : _boardSize;
                        for (int r = 0; r < size; r++)
                        {
                            for (int c = 0; c < size; c++)
                            {
                                int idx = r * size + c;
                                if (idx < syncMsg.BoardState.Length)
                                {
                                    char ch = syncMsg.BoardState[idx];
                                    if (ch == 'X' || ch == 'O')
                                    {
                                        _boardControl?.UpdateBoardUI(r, c, ch.ToString());
                                    }
                                }
                            }
                        }
                    }

                    // Bắt đầu đếm ngược thời gian lượt đi
                    _remainingSeconds = TURN_TIMEOUT_SECONDS;
                    UpdateTimerUI(_remainingSeconds);

                    if (_boardControl != null) _boardControl.Enabled = !_isSpectator;
                    _clientTimer.Start();
                }
                // 2. XỬ LÝ KHI CÓ NGƯỜI ĐÁNH CỜ
                else if (message.Type == MessageType.Move && message is MoveMessage moveMsg)
                {
                    // Vẽ quân cờ lên UI thông qua BoardControl
                    _boardControl?.UpdateBoardUI(moveMsg.Row, moveMsg.Column, moveMsg.Symbol);

                    // Đảo lượt nội bộ (nếu là khán giả thì không có lượt)
                    if (_isSpectator)
                    {
                        _isMyTurn = false;
                    }
                    else
                    {
                        _isMyTurn = (moveMsg.Symbol != _mySymbol);
                    }

                    // Đổi thông báo lượt đi trên giao diện với màu sắc tương ứng X hoặc O
                    if (moveMsg.Symbol == "X")
                    {
                        string playerOName = lblPlayerO?.Text?.Replace("O: ", "") ?? "";
                        SetTurnUI("O", playerOName);
                    }
                    else
                    {
                        string playerXName = lblPlayerX?.Text?.Replace("X: ", "") ?? "";
                        SetTurnUI("X", playerXName);
                    }

                    // Reset đồng hồ cho lượt mới
                    _remainingSeconds = TURN_TIMEOUT_SECONDS;
                    _clientTimer.Start();
                }
                // 3. XỬ LÝ NHẬN TIN NHẮN CHAT
                else if (message.Type == MessageType.Chat && message is ChatMessage chatMsg)
                {
                    rtbChatHistory?.AppendText($"[{DateTime.Now:HH:mm}] {chatMsg.SenderName}: {chatMsg.Message}\n");
                    rtbChatHistory?.ScrollToCaret();
                }
                // 4. XỬ LÝ KẾT THÚC TRẬN ĐẤU
                else if (message.Type == MessageType.GameOver && message is GameOverMessage gameOverMsg)
                {
                    _isMyTurn = false;
                    _clientTimer.Stop();

                    lblTurnValue.Text = "Trận đấu kết thúc!";
                    lblTurnValue.ForeColor = Color.Gold;
                    lblTurnValue.BackColor = Color.FromArgb(50, 50, 20);

                    // Chỉ làm nổi bật 5 ô chiến thắng khi thắng bằng cờ (ResultType == Win)
                    if (gameOverMsg.ResultType == "Win" && gameOverMsg.WinningLine != null && gameOverMsg.WinningLine.Length >= 5)
                    {
                        var winningPts = new List<Point>();
                        foreach (var coord in gameOverMsg.WinningLine)
                        {
                            var parts = coord.Split(',');
                            if (parts.Length == 2 && int.TryParse(parts[0], out int r) && int.TryParse(parts[1], out int c))
                            {
                                winningPts.Add(new Point(r, c));
                            }
                        }
                        _boardControl?.HighlightWinningCells(winningPts);
                    }

                    _gameEnded = true; // Đánh dấu game đã kết thúc

                    // Gọi phương thức async xử lý hiệu ứng 5s và hộp thoại tái đấu
                    HandleEndGameSequenceAsync(gameOverMsg);
                }



                else if (message is ResponseMessage resMsgOppRematch && resMsgOppRematch.Action == "OpponentRematchRequest")
                {
                    if (!_isSpectator)
                    {
                        var dialogResult = MessageBox.Show(
                            "Đối thủ muốn TÁI ĐẤU với bạn. Bạn có đồng ý không?",
                            "Tái đấu",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );
                        if (dialogResult == DialogResult.Yes)
                        {
                            var req = new CaroGame.Protocol.Messages.RequestMessage { SenderId = _playerName, Action = "RematchAccepted", Data = _roomId };
                            _ = _clientConnection.SendMessageAsync(req);
                            // Không cần đóng form. Server sẽ gửi GameStateMessage để reset game.
                        }
                        else
                        {
                            var req = new CaroGame.Protocol.Messages.RequestMessage { SenderId = _playerName, Action = "RematchDeclined", Data = _roomId };
                            _ = _clientConnection.SendMessageAsync(req);
                        }
                    }
                }
                else if (message is ResponseMessage resMsgRematch && resMsgRematch.Action == "RematchDeclined")
                {
                    if (!_isSpectator)
                    {
                        MessageBox.Show("Đối thủ đã từ chối yêu cầu tái đấu.", "Tái đấu bị từ chối", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                // RematchAccepted: Server sẽ tự gửi GameStateMessage mới, không cần đóng form
                // (handler GameStateMessage ở trên sẽ tự reset bàn cờ)
                else if (message is ResponseMessage resMsgDraw && resMsgDraw.Action == "OpponentDrawRequest")
                {
                    if (!_isSpectator)
                    {
                        var dialogResult = MessageBox.Show("Đối thủ muốn XIN HÒA. Bạn có đồng ý không?", "Lời cầu hòa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult == DialogResult.Yes)
                        {
                            var req = new CaroGame.Protocol.Messages.RequestMessage { SenderId = _playerName, Action = "DrawAccepted", Data = _roomId };
                            _ = _clientConnection.SendMessageAsync(req);
                        }
                        else
                        {
                            var req = new CaroGame.Protocol.Messages.RequestMessage { SenderId = _playerName, Action = "DrawDeclined", Data = _roomId };
                            _ = _clientConnection.SendMessageAsync(req);
                        }
                    }
                }
                else if (message is ResponseMessage resMsgDrawDeclined && resMsgDrawDeclined.Action == "DrawDeclined")
                {
                    if (!_isSpectator)
                    {
                        MessageBox.Show("Đối thủ đã từ chối lời cầu hòa của bạn.", "Từ chối cầu hòa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                // 5. XỬ LÝ CẬP NHẬT TRẠNG THÁI PHÒNG (SỐ KHÁN GIẢ)
                else if (message is ResponseMessage resMsg && resMsg.Action == "RoomStateUpdate")
                {
                    var state = System.Text.Json.JsonSerializer.Deserialize<RoomStateDto>(resMsg.Data);
                    if (state != null && (state.RoomId == _roomId || state.RoomName == _roomName))
                    {
                        if (lblSpectators != null)
                        {
                            lblSpectators.Text = $"Khán giả: {state.SpectatorCount}";
                        }
                    }
                }
                // 6. XỬ LÝ ĐỐI THỦ MẤT KẾT NỐI (CHỜ RECONNECT)
                else if (message is ResponseMessage disconnMsg && disconnMsg.Action == "OpponentDisconnected")
                {
                    _isMyTurn = false; // Khóa bàn cờ trong lúc chờ
                    _clientTimer.Stop();

                    if (int.TryParse(disconnMsg.Data, out int graceSeconds))
                        _reconnectSecondsLeft = graceSeconds;
                    else
                        _reconnectSecondsLeft = GRACE_PERIOD_SECONDS;

                    ShowReconnectPanel(_reconnectSecondsLeft);
                }
                // 7. XỬ LÝ ĐỐI THỦ ĐÃ RECONNECT
                else if (message is ResponseMessage reconnMsg && reconnMsg.Action == "OpponentReconnected")
                {
                    HideReconnectPanel();
                    MessageBox.Show($"Đối thủ [{reconnMsg.Data}] đã kết nối lại thành công!",
                                    "Kết nối lại", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Timer sẽ được khởi động lại khi nhận MoveMessage tiếp theo
                }
                // 8. HỎI Ý KIẾN CHỜ THÊM (NẾU ĐỐI THỦ KHÔNG RECONNECT KỊP 60S)
                else if (message is ResponseMessage askMsg && askMsg.Action == "AskWaitOpponent")
                {
                    HideReconnectPanel();

                    DialogResult result = MessageBox.Show(
                        "Thời gian chờ 60s đã hết nhưng đối thủ chưa kết nối lại.\nBạn có muốn chờ thêm 120s không?",
                        "Chờ đối thủ",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    var requestMsg = new RequestMessage
                    {
                        Action = "AnswerWaitOpponent",
                        Data = result == DialogResult.Yes ? "Yes" : "No"
                    };
                    _ = _clientConnection.SendMessageAsync(requestMsg);
                }
            }
            catch
            {
                // Lỗi giao diện hoặc parse data, xử lý an toàn
            }
        }

        private void btnLeaveRoom_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.Retry || _isSpectator || !_clientConnection.IsConnected) 
            {
                base.OnFormClosing(e);
                return;
            }

            base.OnFormClosing(e);

            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn rời phòng đấu?",
                "Xác nhận rời phòng",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                var requestMsg = new RequestMessage
                {
                    Type = MessageType.Request,
                    SenderId = "",
                    Action = "LeaveRoom",
                    Data = ""
                };

                _ = Task.Run(async () =>
                {
                    try { await _clientConnection.SendMessageAsync(requestMsg); }
                    catch (Exception) { /* Bỏ qua ngoại lệ đường truyền khi đang đóng form */ }
                });

                // Ngắt sự kiện lắng nghe để tránh lỗi rò rỉ bộ nhớ
                _clientConnection.OnMessageReceived -= HandleGameMessage;
            }
        }

        // Giữ lại event handler gốc để file Designer.cs không bị lỗi
        private void lblPlayerX_Click(object sender, EventArgs e)
        {
        }

        private void BtnSend_Click(object? sender, EventArgs e)
        {
            SendChatMessage();
        }

        private void TxtChatInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Chặn tiếng beep
                SendChatMessage();
            }
        }

        private void SendChatMessage()
        {
            if (string.IsNullOrWhiteSpace(txtChatInput.Text))
                return;

            var chatMsg = new ChatMessage
            {
                RoomId = _roomId,
                SenderName = _playerName,
                Message = txtChatInput.Text.Trim()
            };

            _ = _clientConnection.SendMessageAsync(chatMsg);
            txtChatInput.Clear();
        }

        private void SetTurnUI(string currentSymbol, string playerName)
        {
            if (currentSymbol == "X")
            {
                lblTurnValue.Text = _isSpectator ? $"X ({playerName}) [Đang xem]" : $"X ({playerName})";
                lblTurnValue.ForeColor = Color.DeepSkyBlue;
                lblTurnValue.BackColor = Color.FromArgb(20, 50, 75);
            }
            else
            {
                lblTurnValue.Text = _isSpectator ? $"O ({playerName}) [Đang xem]" : $"O ({playerName})";
                lblTurnValue.ForeColor = Color.FromArgb(255, 110, 110);
                lblTurnValue.BackColor = Color.FromArgb(75, 25, 25);
            }
        }
        // RECONNECT UI HELPERS

        // Grace period đếm ngược sử dụng hằng số GRACE_PERIOD_SECONDS

        /// <summary>
        /// Hiện overlay đếm ngược khi đối thủ mất kết nối.
        /// Tạo Label màu đỏ cam phủ lên bàn cờ, tự cập nhật mỗi giây.
        /// </summary>
        private void ShowReconnectPanel(int secondsLeft)
        {
            if (_lblReconnectStatus == null)
            {
                _lblReconnectStatus = new Label
                {
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(200, 180, 60, 0), // Cam đỏ bán trong suốt
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.None,
                    AutoSize = false
                };
                this.Controls.Add(_lblReconnectStatus);
                _lblReconnectStatus.BringToFront();
            }

            // Căn giữa form
            _lblReconnectStatus.Size = new Size(this.ClientSize.Width - 40, 80);
            _lblReconnectStatus.Location = new Point(20, (this.ClientSize.Height - 80) / 2);
            _lblReconnectStatus.Visible = true;

            _reconnectSecondsLeft = secondsLeft;
            UpdateReconnectLabel();

            _reconnectCountdownTimer.Interval = 1000;
            _reconnectCountdownTimer.Tick -= ReconnectTimer_Tick; // Tránh đăng ký trùng
            _reconnectCountdownTimer.Tick += ReconnectTimer_Tick;
            _reconnectCountdownTimer.Start();
        }

        private void ReconnectTimer_Tick(object? sender, EventArgs e)
        {
            _reconnectSecondsLeft--;
            if (_reconnectSecondsLeft <= 0)
            {
                _reconnectCountdownTimer.Stop();
                return;
            }
            UpdateReconnectLabel();
        }

        private void UpdateReconnectLabel()
        {
            if (_lblReconnectStatus == null) return;
            _lblReconnectStatus.Text = $"[!] Đối thủ mất kết nối!\nChờ kết nối lại... {_reconnectSecondsLeft}s";
        }

        /// <summary>
        /// Ẩn overlay đếm ngược (khi đối thủ reconnect hoặc khi kết thúc trận).
        /// </summary>
        private void HideReconnectPanel()
        {
            _reconnectCountdownTimer.Stop();
            if (_lblReconnectStatus != null)
                _lblReconnectStatus.Visible = false;
        }
        // HIỆU ỨNG CHIẾN THẮNG (CONFETTI)
        private void ShowEndGameEffect(bool isWin)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ShowEndGameEffect(isWin)));
                return;
            }

            if (_lblVictory == null)
            {
                _lblVictory = new Label
                {
                    AutoSize = false,
                    Size = new Size(pnlMain.Width, 100),
                    Font = new Font("Segoe UI", 48, FontStyle.Bold),
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor = Cursors.Hand,
                    Anchor = AnchorStyles.Left | AnchorStyles.Right
                };

                _lblVictory.Click += (s, e) => HideVictoryEffect();

                _lblVictory.Location = new Point(0, (pnlMain.Height - 100) / 2 - 50);
                pnlMain.Controls.Add(_lblVictory);
            }

            _lblVictory.Text = isWin ? "Bạn đã thắng" : "Bạn đã thua";
            _lblVictory.ForeColor = isWin ? Color.Red : Color.DodgerBlue;

            _lblVictory.Visible = true;
            _lblVictory.BringToFront();

            if (isWin)
            {
                _confettiParticles.Clear();
                _confettiSpeeds.Clear();
                Random rand = new Random();
                for (int i = 0; i < 150; i++)
                {
                    _confettiParticles.Add(new PointF(rand.Next(pnlMain.Width), rand.Next(-800, 0)));
                    _confettiSpeeds.Add((float)(rand.NextDouble() * 6 + 4));
                }

                if (_confettiTimer == null)
                {
                    _confettiTimer = new System.Windows.Forms.Timer { Interval = 20 };
                    _confettiTimer.Tick += ConfettiTimer_Tick;
                    pnlMain.Paint += PnlMain_Paint;
                }
                _confettiTimer.Start();
            }
            else
            {
                if (_confettiTimer != null) _confettiTimer.Stop();
                pnlMain.Invalidate();
            }
        }

        private async void HandleEndGameSequenceAsync(GameOverMessage gameOverMsg)
        {
            bool isWin = gameOverMsg.WinnerName == _playerName;
            bool isDraw = gameOverMsg.ResultType == "Draw";

            HideReconnectPanel();

            if (isDraw)
            {
                MessageBox.Show("Ván đấu hòa! (Cầu hòa hoặc hết ô trống).", "Kết thúc ván đấu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ShowEndGameEffect(isWin);

                // Hien thi thong bao ket qua rieng cho cac truong hop dac biet
                if (gameOverMsg.ResultType == "Timeout")
                {
                    MessageBox.Show(
                        isWin ? "CHIẾN THắNG!\n\nĐối thủ đã hết thời gian suy nghĩ." : "Bạn đã hết giờ suy nghĩ! Đối thủ được xử thắng.",
                        "Kết thúc", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (gameOverMsg.ResultType == "Disconnect")
                {
                    MessageBox.Show(
                        isWin ? "Đối thủ không kết nối lại kịp trong 90 giây!\nBạn được xử thắng." : "Bạn không kết nối lại kịp thời gian cho phép!\nĐối thủ được xử thắng.",
                        "Kết thúc", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (gameOverMsg.ResultType == "Surrender")
                {
                    MessageBox.Show(
                        isWin ? "Đối thủ đã đầu hàng! Chúc mừng giành chiến thắng." : "Bạn đã đầu hàng.",
                        "Kết thúc", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Đợi 5 giây hiệu ứng trước khi hiển thị hộp thoại tái đấu
                await Task.Delay(REMATCH_PROMPT_DELAY_MS);

                // Ca 2 ben (Win, Timeout, Surrender) deu duoc hoi tai dau (tru khan gia va Disconnect)
                bool canRematch = !_isSpectator && gameOverMsg.ResultType != "Disconnect";
                if (canRematch)
                {
                    var dialogResult = MessageBox.Show(
                        "Bạn có muốn gửi YÊU CẦU TÁI ĐẤU tới đối thủ không?",
                        "Tái đấu",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (dialogResult == DialogResult.Yes)
                    {
                        var req = new CaroGame.Protocol.Messages.RequestMessage { SenderId = _playerName, Action = "RematchRequest", Data = _roomId };
                        _ = _clientConnection.SendMessageAsync(req);
                    }
                }
            }
        }
        private void ConfettiTimer_Tick(object? sender, EventArgs e)
        {
            for (int i = 0; i < _confettiParticles.Count; i++)
            {
                var p = _confettiParticles[i];
                p.Y += _confettiSpeeds[i];
                if (p.Y > pnlMain.Height)
                {
                    Random rand = new Random();
                    p.Y = rand.Next(-50, -10);
                    p.X = rand.Next(pnlMain.Width);
                }
                _confettiParticles[i] = p;
            }
            pnlMain.Invalidate();
        }

        private void PnlMain_Paint(object? sender, PaintEventArgs e)
        {
            if (_confettiTimer != null && _confettiTimer.Enabled)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(Color.DeepSkyBlue))
                {
                    foreach (var pt in _confettiParticles)
                    {
                        e.Graphics.FillRectangle(brush, pt.X, pt.Y, 12, 12);
                    }
                }
            }
        }

        private void HideVictoryEffect()
        {
            if (_lblVictory != null) _lblVictory.Visible = false;
            if (_confettiTimer != null) _confettiTimer.Stop();
            pnlMain.Invalidate();
        }
    }
}

