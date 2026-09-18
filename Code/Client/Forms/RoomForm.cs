using CaroGame.Protocol;
using CaroGame.Protocol.Messages; // Namespace chứa ResponseMessage và RoomStateDto
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Network;
using CaroGame.Protocol.Messages.Game;
using CaroGame.Protocol.Messages.Response;
using CaroGame.Protocol.Messages.Room; // Thêm dòng này

namespace Client.Forms
{
    public partial class RoomForm : Form
    {
        private ClientConnection _clientConnection;
        private string _roomName;
        private string _playerName;
        private bool _isHost;
        private string _roomId = "";
        private bool _isReady = false;
        private int _boardSize = 15; // Co dinh 15x15
        private string? _challengeTarget;
        
        private Button btnSwapSymbol = null!;
        private Label lblSpectatorsCount = null!;

        private bool _isSpectator = false;

        public RoomForm(ClientConnection connection, string roomName, string playerName, bool isHost, string? challengeTarget = null, bool isSpectator = false, string roomId = "")
        {
            InitializeComponent();

            _clientConnection = connection;
            _roomName = roomName;
            _playerName = playerName;
            _isHost = isHost;
            _challengeTarget = challengeTarget;
            _isSpectator = isSpectator;
            _roomId = roomId;

            ApplyInitialLogic();

            // Đăng ký lắng nghe thông báo từ Server khi vừa mở Form
            _clientConnection.OnMessageReceived += HandleRoomMessage;
        }

        private void ApplyInitialLogic()
        {
            lblRoomName.Text = "PHÒNG: " + _roomName.ToUpper();
            lblTitleX.Text = "Chủ phòng (X)";
            lblTitleO.Text = "Khách (O)";

            if (_isHost)
            {
                lblPlayerX_Name.Text = _playerName;
                lblPlayerX_Name.ForeColor = Color.DeepSkyBlue;
                lblPlayerX_Status.Text = "Chủ phòng";
                lblPlayerX_Status.ForeColor = Color.LimeGreen;

                lblPlayerO_Name.Text = "Đang trống...";
                lblPlayerO_Name.ForeColor = Color.Gray;
                lblPlayerO_Status.Text = "Đang chờ khách...";
                lblPlayerO_Status.ForeColor = Color.Gray;

                btnStartGame.Visible = true;
                btnStartGame.BackColor = Color.Gray;
                btnStartGame.Enabled = false;
            }
            else
            {
                if (_isSpectator)
                {
                    lblPlayerX_Name.Text = "Đang tải...";
                    lblPlayerO_Name.Text = "Đang tải...";
                    lblPlayerX_Status.Text = "Khán giả";
                    lblPlayerO_Status.Text = "Khán giả";
                    btnStartGame.Visible = false;
                }
                else
                {
                    lblPlayerO_Name.Text = _playerName;
                    lblPlayerO_Name.ForeColor = Color.Tomato;
                    lblPlayerO_Status.Text = "Đang chờ...";
                    lblPlayerO_Status.ForeColor = Color.Orange;

                    btnStartGame.Visible = true;
                    btnStartGame.Text = "SẴN SÀNG";
                    btnStartGame.BackColor = Color.SeaGreen;
                    btnStartGame.Enabled = true;
                }
            }

            this.Text = "Caro Arena - Đang chờ...";
            SetupRoomControls();
        }

        private void SetupRoomControls()
        {
            btnSwapSymbol = new Button();
            btnSwapSymbol.Text = "⇄\nĐỔI";
            btnSwapSymbol.Size = new Size(70, 50);
            btnSwapSymbol.Location = new Point(285, 160);
            btnSwapSymbol.BackColor = Color.FromArgb(52, 152, 219);
            btnSwapSymbol.ForeColor = Color.White;
            btnSwapSymbol.FlatStyle = FlatStyle.Flat;
            btnSwapSymbol.FlatAppearance.BorderSize = 0;
            btnSwapSymbol.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSwapSymbol.Cursor = Cursors.Hand;
            btnSwapSymbol.Click += BtnSwapSymbol_Click;
            btnSwapSymbol.Visible = _isHost; // Chỉ chủ phòng mới đổi quân được

            lblSpectatorsCount = new Label();
            lblSpectatorsCount.Text = "Khán giả đang xem: 0";
            lblSpectatorsCount.ForeColor = Color.LightSkyBlue;
            lblSpectatorsCount.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
            lblSpectatorsCount.Location = new Point(217, 65);
            lblSpectatorsCount.Size = new Size(200, 25);
            lblSpectatorsCount.TextAlign = ContentAlignment.MiddleCenter;

            this.Controls.Add(btnSwapSymbol);
            this.Controls.Add(lblSpectatorsCount);
            btnSwapSymbol.BringToFront();
        }

        private void BtnSwapSymbol_Click(object? sender, EventArgs e)
        {
            var req = new RequestMessage { Action = "SwapSymbol", Data = "", SenderId = "" };
            _ = _clientConnection.SendMessageAsync(req);
        }

        // ==========================================
        // NHẬN DỮ LIỆU TỪ SERVER VÀ VẼ LẠI UI
        // ==========================================
        private void HandleRoomMessage(BaseMessage message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => HandleRoomMessage(message)));
                return;
            }

            if (message is GameStateMessage gameState)
            {
                // Kiểm tra xem ID có trùng không
                if (gameState.RoomId == this._roomId || gameState.RoomId == this._roomName || (!string.IsNullOrEmpty(this._roomId) && gameState.RoomId.Contains(this._roomId)))
                {
                    _clientConnection.OnMessageReceived -= HandleRoomMessage;

                    GameForm gameForm = new GameForm(_clientConnection, this._roomId, _roomName, _playerName, _isHost, _boardSize, _isSpectator);
                    gameForm.FormClosed += (s, args) => this.Close();
                    gameForm.InitGameState(gameState);

                    this.Hide();
                    gameForm.Show();
                }
                return;
            }

            var resMsg = message as ResponseMessage;
            if (resMsg != null)
            {
                if (resMsg.Action == "RoomStateUpdate")
                {
                    var state = System.Text.Json.JsonSerializer.Deserialize<RoomStateDto>(resMsg.Data);

                    if (state != null && (state.RoomName == this._roomName || state.RoomId == this._roomId))
                    {
                        this._roomId = state.RoomId;
                        this._boardSize = state.BoardSize;

                        // Nếu có mục tiêu thách đấu, gửi lời mời và xóa mục tiêu để không gửi lại
                        if (!string.IsNullOrEmpty(_challengeTarget))
                        {
                            var invite = new InviteMessage
                            {
                                SenderId = this._playerName,
                                TargetPlayerId = _challengeTarget,
                                RoomId = this._roomId
                            };
                            _ = _clientConnection.SendMessageAsync(invite);
                            _challengeTarget = null;
                        }

                        string hostSymbol = !string.IsNullOrEmpty(state.HostSymbol) ? state.HostSymbol : "X";
                        string guestSymbol = !string.IsNullOrEmpty(state.GuestSymbol) ? state.GuestSymbol : (hostSymbol == "X" ? "O" : "X");
                        string hostName = !string.IsNullOrEmpty(state.HostName) ? state.HostName : state.PlayerX;
                        string guestName = !string.IsNullOrEmpty(state.GuestName) ? state.GuestName : state.PlayerO;

                        // Cập nhật thẻ Chủ phòng (bên trái)
                        lblTitleX.Text = $"Chủ phòng ({hostSymbol})";
                        lblTitleX.ForeColor = (hostSymbol == "X") ? Color.DeepSkyBlue : Color.Tomato;
                        lblPlayerX_Name.Text = !string.IsNullOrEmpty(hostName) ? hostName : "Chưa rõ";
                        lblPlayerX_Name.ForeColor = (hostSymbol == "X") ? Color.DeepSkyBlue : Color.Tomato;
                        lblPlayerX_Status.Text = "Chủ phòng";
                        lblPlayerX_Status.ForeColor = Color.LimeGreen;

                        // Cập nhật thẻ Khách (bên phải)
                        lblTitleO.Text = $"Khách ({guestSymbol})";
                        lblTitleO.ForeColor = (guestSymbol == "X") ? Color.DeepSkyBlue : Color.Tomato;

                        if (!string.IsNullOrEmpty(guestName))
                        {
                            lblPlayerO_Name.Text = guestName;
                            lblPlayerO_Name.ForeColor = (guestSymbol == "X") ? Color.DeepSkyBlue : Color.Tomato;

                            bool isGuestReady = state.IsGuestReady || state.IsPlayerOReady;
                            if (isGuestReady)
                            {
                                lblPlayerO_Status.Text = "Đã sẵn sàng";
                                lblPlayerO_Status.ForeColor = Color.LimeGreen;
                            }
                            else
                            {
                                lblPlayerO_Status.Text = "Đang chờ...";
                                lblPlayerO_Status.ForeColor = Color.Orange;
                            }
                        }
                        else
                        {
                            lblPlayerO_Name.Text = "Đang trống...";
                            lblPlayerO_Name.ForeColor = Color.Gray;
                            lblPlayerO_Status.Text = "Đang chờ khách...";
                            lblPlayerO_Status.ForeColor = Color.Gray;
                        }
                        
                        if (lblSpectatorsCount != null)
                        {
                            lblSpectatorsCount.Text = "Khán giả đang xem: " + state.SpectatorCount;
                        }

                        if (_isHost)
                        {
                            bool hasGuest = !string.IsNullOrEmpty(guestName);
                            bool isGuestReady = state.IsGuestReady || state.IsPlayerOReady;

                            btnStartGame.Enabled = hasGuest && isGuestReady;
                            btnStartGame.BackColor = btnStartGame.Enabled ? Color.SeaGreen : Color.Gray;
                        }
                    }
                }
                else if (resMsg.Action == "StartGame" && (resMsg.Data == this._roomId || resMsg.Data == this._roomName))
                {
                    _clientConnection.OnMessageReceived -= HandleRoomMessage;

                    GameForm gameForm = new GameForm(_clientConnection, this._roomId, _roomName, _playerName, _isHost, _boardSize, _isSpectator);
                    gameForm.FormClosed += (s, args) => this.Close();

                    this.Hide();
                    gameForm.Show();
                }
            }
        }

        private void BtnStartGame_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_roomId))
            {
                MessageBox.Show("Chưa nhận được thông tin phòng từ server. Vui lòng đợi...", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_isHost)
            {
                // Bấm nút xong thì khóa lại ngay để tránh spam click nhiều lần
                btnStartGame.Enabled = false;

                // Đóng gói yêu cầu Bắt đầu game
                var request = new CaroGame.Protocol.Messages.Room.StartMatchMessage
                {
                    SenderId = _playerName,
                    RoomId = _roomId // Dùng RoomId đúng chuẩn
                };

                _ = Task.Run(async () => {
                    try { await _clientConnection.SendMessageAsync(request); }
                    catch { /* Bỏ qua nếu lỗi mạng */ }
                });
            }
            else
            {
                // Là Khách (Guest) - Đảo trạng thái sẵn sàng
                _isReady = !_isReady;
                if (_isReady)
                {
                    btnStartGame.Text = "HỦY BỎ";
                    btnStartGame.BackColor = Color.Orange;
                    lblPlayerO_Status.Text = "Đã sẵn sàng";
                    lblPlayerO_Status.ForeColor = Color.LimeGreen;
                }
                else
                {
                    btnStartGame.Text = "SẴN SÀNG";
                    btnStartGame.BackColor = Color.SeaGreen;
                    lblPlayerO_Status.Text = "Đang chờ...";
                    lblPlayerO_Status.ForeColor = Color.Orange;
                }

                var request = new CaroGame.Protocol.Messages.Room.ReadyMessage
                {
                    SenderId = _playerName,
                    RoomId = _roomId,
                    IsReady = _isReady
                };
                
                _ = Task.Run(async () => {
                    try { await _clientConnection.SendMessageAsync(request); }
                    catch { /* Bỏ qua nếu lỗi mạng */ }
                });
            }
        }

        private void BtnLeaveRoom_Click(object? sender, EventArgs e)
        {
            // Báo cho Server là mình thoát
            var req = new RequestMessage { Type = MessageType.Request, Action = "LeaveRoom", Data = "" };
            _ = Task.Run(async () => { await _clientConnection.SendMessageAsync(req); });

            this.Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // HỦY ĐĂNG KÝ SỰ KIỆN ĐỂ TRÁNH LỖI KHI ĐÓNG FORM
            _clientConnection.OnMessageReceived -= HandleRoomMessage;
            base.OnFormClosed(e);
        }
    }
}