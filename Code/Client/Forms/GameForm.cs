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

        private System.Windows.Forms.Timer _clientTimer = new System.Windows.Forms.Timer();
        private int _remainingSeconds = 30;

        // Reconnect: Timer đếm ngược grace period trên UI
        private System.Windows.Forms.Timer _reconnectCountdownTimer = new System.Windows.Forms.Timer();
        private int _reconnectSecondsLeft = 0;
        private Label? _lblReconnectStatus = null;

        public GameForm(ClientConnection connection, string roomId, string roomName, string playerName, bool isHost, int boardSize = 15, bool isSpectator = false)
        {
            InitializeComponent();

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

        // ======================================================
        // PHẦN 1: TẠO BÀN CỜ & GỬI DỮ LIỆU
        // ======================================================

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

        // ======================================================
        // PHẦN 3: GIAO TIẾP SERVER TRONG GAME & THOÁT
        // ======================================================

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

                // ==========================================
                // 1. XỬ LÝ LÚC VỪA VÀO PHÒNG
                // ==========================================
                if (message.Type == MessageType.GameState && message is GameStateMessage syncMsg)
                {
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
                                        _boardControl.UpdateBoardUI(r, c, ch.ToString());
                                    }
                                }
                            }
                        }
                    }

                    // Bắt đầu đếm ngược thời gian
                    _remainingSeconds = 30;
                    _clientTimer.Start();
                }

                // ==========================================
                // 2. XỬ LÝ KHI CÓ NGƯỜI ĐÁNH CỜ
                // ==========================================
                else if (message.Type == MessageType.Move && message is MoveMessage moveMsg)
                {
                    // Vẽ quân cờ lên UI thông qua BoardControl
                    _boardControl.UpdateBoardUI(moveMsg.Row, moveMsg.Column, moveMsg.Symbol);

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
                    _remainingSeconds = 30;
                    _clientTimer.Start();
                }

                // ==========================================
                // 3. XỬ LÝ NHẬN TIN NHẮN CHAT
                // ==========================================
                else if (message.Type == MessageType.Chat && message is ChatMessage chatMsg)
                {
                    rtbChatHistory?.AppendText($"[{DateTime.Now:HH:mm}] {chatMsg.SenderName}: {chatMsg.Message}\n");
                    rtbChatHistory?.ScrollToCaret();
                }

                // ==========================================
                // 4. XỬ LÝ KẾT THÚC TRẬN ĐẤU
                // ==========================================
                else if (message.Type == MessageType.GameOver && message is GameOverMessage gameOverMsg)
                {
                    _isMyTurn = false;
                    _clientTimer.Stop();

                    lblTurnValue.Text = "Trận đấu kết thúc!";
                    lblTurnValue.ForeColor = Color.Gold;
                    lblTurnValue.BackColor = Color.FromArgb(50, 50, 20);

                    // Làm nổi bật 5 ô chiến thắng liên tiếp trên bàn cờ
                    if (gameOverMsg.WinningLine != null && gameOverMsg.WinningLine.Length > 0)
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
                        _boardControl.HighlightWinningCells(winningPts);
                    }

                    if (gameOverMsg.ResultType == "Win")
                    {
                        bool isMe = !string.IsNullOrEmpty(gameOverMsg.WinnerName) && gameOverMsg.WinnerName == _playerName;
                        if (isMe)
                        {
                            MessageBox.Show($"CHIẾN THẮNG TUYỆT VỜI!\n\nChúc mừng bạn [{gameOverMsg.WinnerName}] đã tạo thành chuỗi 5 ô cờ liên tiếp và giành chiến thắng vẻ vang!",
                                            "Chúc mừng chiến thắng", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (!_isSpectator)
                        {
                            MessageBox.Show($"Đối thủ [{gameOverMsg.WinnerName}] đã tạo thành 5 ô cờ liên tiếp và chiến thắng.\nHãy cố gắng ở ván đấu tiếp theo nhé!",
                                            "Kết quả trận đấu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show($"Người chơi [{gameOverMsg.WinnerName}] đã giành chiến thắng với 5 ô cờ liên tiếp!",
                                            "Trận đấu kết thúc", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else if (gameOverMsg.ResultType == "Draw")
                    {
                        MessageBox.Show("Ván đấu hòa! Không còn ô trống nào trên bàn cờ.",
                                        "Kết thúc ván đấu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (gameOverMsg.ResultType == "Timeout")
                    {
                        bool isMe = !string.IsNullOrEmpty(gameOverMsg.WinnerName) && gameOverMsg.WinnerName == _playerName;
                        if (isMe)
                        {
                            MessageBox.Show($"BẠN ĐÃ THẮNG!\n\nĐối thủ đã hết thời gian suy nghĩ 30s.",
                                            "Chiến thắng do hết giờ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show($"Bạn đã hết giờ suy nghĩ! Người chơi [{gameOverMsg.WinnerName}] được xử thắng.",
                                            "Hết giờ thi đấu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else if (gameOverMsg.ResultType == "Disconnect")
                    {
                        // Grace period đã hết, đối thủ không reconnect kịp
                        bool isWinner = gameOverMsg.WinnerName == _playerName;
                        if (isWinner)
                            MessageBox.Show($"Đối thủ không kết nối lại kịp trong {Managers_GracePeriodSeconds} giây!\nBạn được xử thắng.",
                                            "Thắng do đối thủ mất kết nối", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show($"Bạn không kết nối lại kịp thời gian cho phép!\n[{gameOverMsg.WinnerName}] được xử thắng.",
                                            "Hết thời gian chờ kết nối lại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (gameOverMsg.ResultType == "Surrender")
                    {
                        MessageBox.Show($"Đối thủ đã rời phòng / đầu hàng! Chúc mừng [{gameOverMsg.WinnerName}] giành chiến thắng.",
                                        "Đối thủ đầu hàng", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // Ẩn panel reconnect nếu đang hiện
                    HideReconnectPanel();
                }
                
                // ==========================================
                // 5. XỬ LÝ CẬP NHẬT TRẠNG THÁI PHÒNG (SỐ KHÁN GIẢ)
                // ==========================================
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

                // ==========================================
                // 6. XỬ LÝ ĐỐI THỦ MẤT KẾT NỐI (CHỜ RECONNECT)
                // ==========================================
                else if (message is ResponseMessage disconnMsg && disconnMsg.Action == "OpponentDisconnected")
                {
                    _isMyTurn = false; // Khóa bàn cờ trong lúc chờ
                    _clientTimer.Stop();

                    if (int.TryParse(disconnMsg.Data, out int graceSeconds))
                        _reconnectSecondsLeft = graceSeconds;
                    else
                        _reconnectSecondsLeft = 90;

                    ShowReconnectPanel(_reconnectSecondsLeft);
                }

                // ==========================================
                // 7. XỬ LÝ ĐỐI THỦ ĐÃ RECONNECT
                // ==========================================
                else if (message is ResponseMessage reconnMsg && reconnMsg.Action == "OpponentReconnected")
                {
                    HideReconnectPanel();
                    MessageBox.Show($"Đối thủ [{reconnMsg.Data}] đã kết nối lại thành công!",
                                    "Kết nối lại", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Timer sẽ được khởi động lại khi nhận MoveMessage tiếp theo
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi giao diện: {ex.Message}\nData: {message.Type}", "Lỗi Client");
            }
        }

        private void btnLeaveRoom_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
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

                _ = Task.Run(async () => {
                    try { await _clientConnection.SendMessageAsync(requestMsg); }
                    catch { /* Bỏ qua lỗi nếu ngắt kết nối mạng rồi */ }
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

        // ==========================================
        // RECONNECT UI HELPERS
        // ==========================================

        /// <summary>Thời gian grace period (phải trùng với ReconnectManager.GracePeriodSeconds)</summary>
        private const int GRACE_PERIOD_SECONDS = 60;
        /// <summary>Dùng trong message Disconnect GameOver thay vì tham chiếu trực tiếp Server assembly</summary>
        private const int Managers_GracePeriodSeconds = GRACE_PERIOD_SECONDS;

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
            _lblReconnectStatus.Text = $"⚠ Đối thủ mất kết nối!\nChờ kết nối lại... {_reconnectSecondsLeft}s";
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
    }
}