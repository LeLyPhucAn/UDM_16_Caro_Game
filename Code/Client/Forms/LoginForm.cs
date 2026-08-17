using System;
using System.Text.RegularExpressions; // Thêm thư viện để dùng Regex kiểm tra ký tự
using System.Windows.Forms;

namespace Client.Forms
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

            // --- BẮT ĐẦU KIỂM TRA DỮ LIỆU NHẬP (VALIDATION) ---

            // 1. Kiểm tra rỗng
            if (string.IsNullOrWhiteSpace(playerName))
            {
                MessageBox.Show("Vui lòng nhập tên người chơi!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPlayerName.Focus();
                return;
            }

            // 2. Kiểm tra độ dài (3 - 15 ký tự)
            if (playerName.Length < 3 || playerName.Length > 15)
            {
                MessageBox.Show("Tên người chơi phải có từ 3 đến 15 ký tự.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPlayerName.Focus();
                return;
            }

            // 3. Kiểm tra ký tự đặc biệt (chỉ cho phép chữ cái, số và dấu gạch dưới)
            if (!Regex.IsMatch(playerName, "^[a-zA-Z0-9_]+$"))
            {
                MessageBox.Show("Tên không được chứa khoảng trắng và ký tự đặc biệt (chỉ dùng chữ, số, dấu _).", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPlayerName.Focus();
                return;
            }

            // 4. Kiểm tra các tên hệ thống bị cấm giả mạo
            string lowerName = playerName.ToLower();
            if (lowerName == "admin" || lowerName == "system" || lowerName == "server" || lowerName == "root")
            {
                MessageBox.Show("Tên này đã được hệ thống bảo lưu. Vui lòng chọn tên khác.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPlayerName.Focus();
                return;
            }

            // --- KẾT THÚC KIỂM TRA ---

            // Nếu hợp lệ: Mở FormLobby và truyền tên người chơi sang
            LobbyForm formLobby = new LobbyForm(playerName);

            // Đăng ký sự kiện: Khi FormLobby đóng -> Đóng hoàn toàn ứng dụng (tránh chạy ngầm)
            formLobby.FormClosed += (s, args) => this.Close();

            // Hiển thị Lobby và ẩn FormLogin
            formLobby.Show();
            this.Hide();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Để trống nếu không dùng đến
        }

        private void LoginForm_Load_1(object sender, EventArgs e)
        {
            // Để trống nếu không dùng đến
        }
    }
}