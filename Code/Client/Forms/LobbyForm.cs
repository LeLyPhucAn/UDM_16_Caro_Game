using System;
using System.Drawing;
using System.Windows.Forms;
using Client.Network; // Bổ sung thư viện mạng để dùng ClientConnection

namespace Client.Forms
{
    public partial class LobbyForm : Form
    {
        private string _playerName;
        private ClientConnection _clientConnection; // Khai báo biến giữ kết nối mạng

        // Cập nhật Constructor để nhận cả tên và cấu hình mạng từ LoginForm
        public LobbyForm(string playerName, ClientConnection clientConnection)
        {
            InitializeComponent();

            _playerName = playerName;
            _clientConnection = clientConnection;

            lblPlayerName.Text = _playerName;

            // Đăng ký sự kiện nút bấm
            btnJoinRoom.Click += btnJoinRoom_Click;
            btnCreateRoom.Click += btnJoinRoom_Click; // Tạm thời dùng chung hàm JoinRoom, sau này tách riêng
            btnExitGame.Click += btnExitGame_Click;
        }

        // Thêm dấu '?' vào object? sender để sửa lỗi cảnh báo màu vàng trên Terminal
        private void btnJoinRoom_Click(object? sender, EventArgs e)
        {
            // Ở Task tiếp theo (Task 3), bạn sẽ cần truyền _clientConnection sang GameForm tương tự như thế này
            GameForm gameForm = new GameForm();

            // Khi rời phòng thì hiển thị lại Sảnh chờ
            gameForm.FormClosed += (s, args) => this.Show();

            gameForm.Show();
            this.Hide();
        }

        private void FormLobby_Load(object? sender, EventArgs e)
        {
            LoadDummyData();
        }

        private void LoadDummyData()
        {
            dgvRooms.Rows.Clear();
            AddRoomRow("#101", "Phòng Vui Vẻ", "1/2", "● Đang chờ", Color.LimeGreen);
            AddRoomRow("#102", "Pro Only", "2/2", "● Đang chơi", Color.Orange);
            AddRoomRow("#103", "Newbie Room", "0/2", "● Trống", Color.Gray);
            AddRoomRow("#104", "Giao lưu nhẹ nhàng", "1/2", "● Đang chờ", Color.LimeGreen);
            AddRoomRow("#105", "Thách đấu vô địch", "2/2", "● Đang chơi", Color.Orange);
        }

        private void AddRoomRow(string roomId, string roomName, string playerCount, string statusText, Color statusColor)
        {
            int rowIndex = dgvRooms.Rows.Add(roomId, roomName, playerCount, statusText);
            DataGridViewRow row = dgvRooms.Rows[rowIndex];

            row.Cells[0].Style.ForeColor = Color.DeepSkyBlue;
            row.Cells[0].Style.SelectionForeColor = Color.DeepSkyBlue;

            row.Cells[3].Style.ForeColor = statusColor;
            row.Cells[3].Style.SelectionForeColor = statusColor;
        }

        private void btnExitGame_Click(object? sender, EventArgs e)
        {
            // Xác nhận trước khi thoát
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát game không?",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                // Ngắt kết nối mạng trước khi thoát (Nếu cần thiết, gọi _clientConnection.Disconnect();)
                Application.Exit();
            }
        }

        private void LobbyForm_Load(object? sender, EventArgs e)
        {
            // Để trống
        }
    }
}