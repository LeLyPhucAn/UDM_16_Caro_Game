using CaroGame.Protocol;
using CaroGame.Protocol.Messages; // Namespace chứa ResponseMessage và RoomStateDto
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Network;

namespace Client.Forms
{
    public partial class RoomForm : Form
    {
        private ClientConnection _clientConnection;
        private string _roomName;
        private string _playerName;
        private bool _isHost;

        public RoomForm(ClientConnection connection, string roomName, string playerName, bool isHost)
        {
            InitializeComponent();

            _clientConnection = connection;
            _roomName = roomName;
            _playerName = playerName;
            _isHost = isHost;

            ApplyInitialLogic();

            // 👉 ĐĂNG KÝ LẮNG NGHE THÔNG BÁO TỪ SERVER KHI VỪA MỞ FORM
            _clientConnection.OnMessageReceived += HandleRoomMessage;
        }

        private void ApplyInitialLogic()
        {
            lblRoomName.Text = "PHÒNG: " + _roomName.ToUpper();
            // (Đoạn này đã hiển thị tốt như trong ảnh của bạn nên không cần thay đổi)
            if (_isHost)
            {
                lblPlayerX_Name.Text = "👤 " + _playerName;
                lblPlayerX_Name.ForeColor = Color.DeepSkyBlue;
                lblPlayerX_Status.Text = "Đang chờ khách...";
                lblPlayerX_Status.ForeColor = Color.Orange;

                btnStartGame.Visible = true;
                btnStartGame.BackColor = Color.Gray;
                btnStartGame.Enabled = false;
            }
            else
            {
                lblPlayerO_Name.Text = "👤 " + _playerName;
                lblPlayerO_Name.ForeColor = Color.Tomato;
                lblPlayerO_Status.Text = "Sẵn sàng";
                lblPlayerO_Status.ForeColor = Color.LimeGreen;

                btnStartGame.Visible = false;
            }
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

            // Dùng 'as' thay vì 'is' để tránh lỗi chưa khởi tạo biến CS0165
            var resMsg = message as ResponseMessage;
            if (resMsg != null)
            {
                if (resMsg.Action == "RoomStateUpdate")
                {
                    var state = System.Text.Json.JsonSerializer.Deserialize<RoomStateDto>(resMsg.Data);

                    if (state != null && state.RoomName == this._roomName)
                    {
                        if (!string.IsNullOrEmpty(state.PlayerX))
                        {
                            lblPlayerX_Name.Text = "👤 " + state.PlayerX;
                            lblPlayerX_Name.ForeColor = System.Drawing.Color.DeepSkyBlue;
                        }
                        else
                        {
                            lblPlayerX_Name.Text = "Đang trống...";
                            lblPlayerX_Name.ForeColor = System.Drawing.Color.Gray;
                        }

                        if (!string.IsNullOrEmpty(state.PlayerO))
                        {
                            lblPlayerO_Name.Text = "👤 " + state.PlayerO;
                            lblPlayerO_Name.ForeColor = System.Drawing.Color.Tomato;
                        }
                        else
                        {
                            lblPlayerO_Name.Text = "Đang trống...";
                            lblPlayerO_Name.ForeColor = System.Drawing.Color.Gray;
                        }

                        if (_isHost)
                        {
                            if (!string.IsNullOrEmpty(state.PlayerX) && !string.IsNullOrEmpty(state.PlayerO))
                            {
                                btnStartGame.Enabled = true;
                                btnStartGame.BackColor = System.Drawing.Color.SeaGreen;
                                lblPlayerX_Status.Text = "Đã sẵn sàng";
                                lblPlayerX_Status.ForeColor = System.Drawing.Color.LimeGreen;
                            }
                            else
                            {
                                btnStartGame.Enabled = false;
                                btnStartGame.BackColor = System.Drawing.Color.Gray;
                                lblPlayerX_Status.Text = "Đang chờ khách...";
                                lblPlayerX_Status.ForeColor = System.Drawing.Color.Orange;
                            }
                        }
                    }
                }
                else if (resMsg.Action == "StartGame")
                {
                    if (resMsg.Data == this._roomName)
                    {
                        _clientConnection.OnMessageReceived -= HandleRoomMessage;

                        GameForm gameForm = new GameForm(_clientConnection, _roomName, _playerName, _isHost);
                        gameForm.FormClosed += (s, args) => this.Close();

                        this.Hide();
                        gameForm.Show();
                    }
                }
            }
        }

        private void BtnStartGame_Click(object? sender, EventArgs e)
        {
            // Bấm nút xong thì khóa lại ngay để tránh spam click nhiều lần
            btnStartGame.Enabled = false;

            // Đóng gói yêu cầu Bắt đầu game
            var request = new RequestMessage
            {
                Type = MessageType.Request,
                SenderId = _playerName,
                Action = "StartGame",
                Data = _roomName // Gửi tên phòng lên để Server biết phòng nào đòi bắt đầu
            };

            _ = Task.Run(async () => {
                try { await _clientConnection.SendMessageAsync(request); }
                catch { /* Bỏ qua nếu lỗi mạng */ }
            });
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
            // 👉 HỦY ĐĂNG KÝ SỰ KIỆN ĐỂ TRÁNH LỖI KHI ĐÓNG FORM
            _clientConnection.OnMessageReceived -= HandleRoomMessage;
            base.OnFormClosed(e);
        }
    }
}