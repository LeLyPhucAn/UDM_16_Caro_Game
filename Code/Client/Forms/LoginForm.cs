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
        private ClientConnection _clientConnection;

        public LoginForm()
        {
            InitializeComponent();
            _clientConnection = new ClientConnection();
            btnEnterLobby.Click += btnEnterLobby_Click;
        }

        private void LoginForm_Load(object? sender, EventArgs e)
        {
            this.ActiveControl = null;

            // Đăng ký sự kiện khi nhận tin nhắn từ Server
            _clientConnection.OnMessageReceived += XyLyKetQuaLogin;

            // Xử lý luôn sự kiện nếu mạng bị lỗi đứt gánh
            _clientConnection.OnError += (ex) => {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => {
                        MessageBox.Show("Lỗi mạng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btnEnterLobby.Enabled = true;
                        btnEnterLobby.Text = "VÀO SẢNH CHỜ";
                    }));
                }
            };
        }

        private void XyLyKetQuaLogin(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => XyLyKetQuaLogin(message)));
                return;
            }

            // KIỂM TRA LOGIC KẾT QUẢ ĐĂNG NHẬP
            // Tạm thời giả định Server sẽ gửi về chữ "SUCCESS" hoặc gói tin có chứa chữ này
            if (message.Contains("SUCCESS") || message.Contains("True"))
            {
                string playerName = txtPlayerName.Text.Trim();

                LobbyForm formLobby = new LobbyForm(playerName, _clientConnection);
                formLobby.FormClosed += (s, args) => this.Close();

                formLobby.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Server thông báo: " + message, "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnEnterLobby.Enabled = true;
                btnEnterLobby.Text = "VÀO SẢNH CHỜ";
            }
        }

        private async void btnEnterLobby_Click(object? sender, EventArgs e)
        {
            string playerName = txtPlayerName.Text.Trim();

            if (string.IsNullOrWhiteSpace(playerName))
            {
                MessageBox.Show("Vui lòng nhập tên!");
                return;
            }

            try
            {
                btnEnterLobby.Enabled = false;
                btnEnterLobby.Text = "ĐANG KẾT NỐI...";

                // 1. Kết nối đến Server
                if (!_clientConnection.IsConnected)
                {
                    await _clientConnection.ConnectToServer("127.0.0.1", 5000);
                }

                // 2. Tạo gói tin (Bắt buộc dùng CaroGame.Protocol)
                CaroGame.Protocol.LoginMessage loginMsg = new CaroGame.Protocol.LoginMessage();
                loginMsg.Username = playerName;
                loginMsg.Password = "";

                string packetStr = CaroGame.Protocol.JsonSerializer.Serialize(loginMsg);

                // 3. GỬI TIN NHẮN THÔNG QUA ĐƯỜNG ỐNG CHUẨN
                // Nó sẽ tự động trỏ về hàm SendDataAsync có 4 byte kích thước của bạn
                await _clientConnection.SendMessage(packetStr);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
                btnEnterLobby.Enabled = true;
                btnEnterLobby.Text = "VÀO SẢNH CHỜ";
            }
        }

        private void btnExit_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LoginForm_Load_1(object? sender, EventArgs e)
        {
        }
    }
}