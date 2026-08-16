using System;
using System.Windows.Forms;

namespace Player
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            btnEnterLobby.Click += btnEnterLobby_Click;
        }

        private void btnEnterLobby_Click(object sender, EventArgs e)
        {
            string playerName = txtPlayerName.Text.Trim();

            // 1. Kiểm tra ô nhập tên
            if (string.IsNullOrWhiteSpace(playerName))
            {
                MessageBox.Show("Vui lòng nhập tên người chơi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Mở FormLobby và truyền tên người chơi sang
            LobbyForm formLobby = new LobbyForm(playerName);

            // 3. Đăng ký sự kiện: Khi FormLobby đóng -> Đóng hoàn toàn ứng dụng (tránh chạy ngầm)
            formLobby.FormClosed += (s, args) => this.Close();

            // 4. Hiển thị Lobby và ẩn FormLogin
            formLobby.Show();
            this.Hide();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}