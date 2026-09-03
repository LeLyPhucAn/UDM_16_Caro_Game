using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Network;
using CaroGame.Protocol.Messages;
using CaroGame.Protocol.Messages.Response;

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

            // Xử lý lỗi mạng an toàn với UI Thread
            _clientConnection.OnError += (ex) =>
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show("Lỗi mạng: " + ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btnEnterLobby.Enabled = true;
                        btnEnterLobby.Text = "ĐĂNG NHẬP"; // Đồng bộ chữ hiển thị
                    }));
                }
            };
        }

        private void XyLyKetQuaLogin(BaseMessage message)
        {
            // Bọc toàn bộ hàm này vào UI thread để Form không bao giờ bị treo
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => XyLyKetQuaLogin(message)));
                return;
            }

            // Server phản hồi ResponseMessage
            if (message is ResponseMessage res)
            {
                if (res.Success)
                {
                    string playerName = txtPlayerName.Text.Trim();

                    // Hủy lắng nghe tin nhắn để không tranh chấp dữ liệu với LobbyForm
                    _clientConnection.OnMessageReceived -= XyLyKetQuaLogin;

                    LobbyForm formLobby = new LobbyForm(playerName, _clientConnection);
                    formLobby.FormClosed += (s, args) => this.Close();

                    formLobby.Show();
                    this.Hide();
                }
                else
                {
                    // Lúc này gọi MessageBox hoàn toàn an toàn, Client không bị Server đá nữa
                    MessageBox.Show("Đăng nhập thất bại: " + res.ErrorMessage, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnEnterLobby.Enabled = true;
                    btnEnterLobby.Text = "ĐĂNG NHẬP"; // Đổi lại thành Đăng nhập cho khớp ảnh
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

                // 2. Tạo đối tượng LoginMessage
                LoginMessage loginMsg = new LoginMessage
                {
                    Username = playerName,
                    Password = string.Empty,
                    SenderId = playerName
                };

                // 3. Gửi Message đi
                await _clientConnection.SendMessageAsync(loginMsg);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối Server: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnEnterLobby.Enabled = true;
                btnEnterLobby.Text = "ĐĂNG NHẬP";
            }
        }

        private void btnExit_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LoginForm_Load_1(object sender, EventArgs e)
        {
            // Trống
        }
    }
}