using System;
using System.Drawing;
using System.Windows.Forms;

namespace Player
{
    public partial class LobbyForm : Form
    {
        public LobbyForm(string playerName)
        {
            InitializeComponent();

            lblPlayerName.Text = playerName;
            btnJoinRoom.Click += btnJoinRoom_Click;
            btnCreateRoom.Click += btnJoinRoom_Click;
            btnExitGame.Click += btnExitGame_Click;
        }
        private void btnJoinRoom_Click(object sender, EventArgs e)
        {
            GameForm gameForm = new GameForm();

            //Khi rời phòng thì hiển thị lại Sảnh chờ
            gameForm.FormClosed += (s, args) => this.Show();

            gameForm.Show();
            this.Hide();
        }
        private void FormLobby_Load(object sender, EventArgs e)
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

        private void btnExitGame_Click(object sender, EventArgs e)
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
                Application.Exit(); 
            }
        }
    }
}