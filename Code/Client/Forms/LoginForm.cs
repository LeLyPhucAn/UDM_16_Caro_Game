using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Network;
using CaroGame.Protocol;

namespace Client.Forms
{
    public partial class LoginForm : Form
    {
        private readonly ClientConnection _clientConnection;

        public LoginForm()
        {
            InitializeComponent();
            _clientConnection = new ClientConnection();
            btnEnterLobby.Click += btnEnterLobby_Click;
            this.Load += LoginForm_Load;
        }

        private void LoginForm_Load(object? sender, EventArgs e)
        {
            this.ActiveControl = null;

            // Đăng ký sự kiện khi nhận được Message từ Server
            _clientConnection.OnMessageReceived += XyLyKetQuaLogin;

            // Xử lý lỗi mạng
            _clientConnection.OnError += (ex) =>
            {
                if (this.InvokeRequired)
                {
                    // 👉 ĐỔI SANG BeginInvoke ĐỂ BẢO VỆ LUỒNG GIAO DIỆN KHỎI DEADLOCK
                    this.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show("Lỗi mạng: " + ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btnEnterLobby.Enabled = true;
                        btnEnterLobby.Text = "VÀO SẢNH CHỜ";
                    }));
                }
            };
        }

        private void XyLyKetQuaLogin(BaseMessage message)
        {
            if (this.InvokeRequired)
            {
                // 👉 ĐỔI SANG BeginInvoke ĐỂ FORM LOGIN KHÔNG BỊ TREO KHI NHẬN TIN
                this.BeginInvoke(new Action(() => XyLyKetQuaLogin(message)));
                return;
            }

            // Server phản hồi ResponseMessage
            if (message is ResponseMessage res)
            {
                if (res.Success)
                {
                    string playerName = txtPlayerName.Text.Trim();

                    // 👉 DÒNG CODE QUAN TRỌNG NHẤT: BỊT TAI LOGIN FORM LẠI!
                    // Hủy lắng nghe tin nhắn để nó không tranh chấp dữ liệu với LobbyForm nữa
                    _clientConnection.OnMessageReceived -= XyLyKetQuaLogin;

                    LobbyForm formLobby = new LobbyForm(playerName, _clientConnection);
                    formLobby.FormClosed += (s, args) => this.Close();

                    formLobby.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Đăng nhập thất bại: " + res.ErrorMessage, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnEnterLobby.Enabled = true;
                    btnEnterLobby.Text = "VÀO SẢNH CHỜ";
                }
            }
        }

        private async void btnEnterLobby_Click(object? sender, EventArgs e)
        {
            string playerName = txtPlayerName.Text.Trim();

            if (string.IsNullOrWhiteSpace(playerName))
            {
                MessageBox.Show("Vui lòng nhập tên người chơi!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnEnterLobby.Enabled = false;
                btnEnterLobby.Text = "ĐANG KẾT NỐI...";

                // 1. Kết nối đến Server nếu chưa kết nối
                if (!_clientConnection.IsConnected)
                {
                    await _clientConnection.ConnectToServer("127.0.0.1", 5000);
                }

                // 2. Tạo đối tượng LoginMessage chuẩn
                LoginMessage loginMsg = new LoginMessage
                {
                    Username = playerName,
                    Password = string.Empty,
                    SenderId = playerName
                };

                // 3. Gửi Message qua socket đã được đóng gói chuẩn 8 byte Header
                await _clientConnection.SendMessageAsync(loginMsg);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối Server: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnEnterLobby.Enabled = true;
                btnEnterLobby.Text = "VÀO SẢNH CHỜ";
            }
        }

        private void btnExit_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LoginForm_Load_1(object sender, EventArgs e)
        {
            // Cứ để nguyên hàm trống này, tránh lỗi file Designer nếu bạn lỡ click đúp trên màn hình kéo thả.
        }
    }
}