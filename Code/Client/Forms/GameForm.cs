using System;
using System.Drawing;
using System.Windows.Forms;

namespace Client.Forms
{
    public partial class GameForm : Form
    {
        private const int ROWS = 10;
        private const int COLS = 10;
        private const int CELL_SIZE = 50;

        private Button[,] board;

        public GameForm()
        {
            InitializeComponent();
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            DrawBoard();
            LoadDummyData();
        }

        private void DrawBoard()
        {
            board = new Button[ROWS, COLS];

            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLS; j++)
                {
                    Button btn = new Button();
                    btn.Size = new Size(CELL_SIZE, CELL_SIZE);
                    btn.Location = new Point(j * CELL_SIZE, i * CELL_SIZE);

                    btn.FlatStyle = FlatStyle.Flat;
                    btn.BackColor = Color.FromArgb(34, 36, 40);
                    btn.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
                    btn.FlatAppearance.BorderSize = 1;
                    btn.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
                    btn.Cursor = Cursors.Hand;

                    pnlBoard.Controls.Add(btn);
                    board[i, j] = btn;
                }
            }
        }

        private void LoadDummyData()
        {
            rtbChatHistory.AppendText("Minh456: Chào bạn nhé, chúc chơi vui vẻ!\n\n");
            rtbChatHistory.AppendText("Nam123: Chào bạn, tí nương tay nha haha\n\n");
            rtbChatHistory.AppendText("Minh456: Góc kia nước đi hay đấy!\n\n");

            SetCaroMark(3, 3, "X", Color.DeepSkyBlue);
            SetCaroMark(4, 4, "O", Color.FromArgb(217, 83, 79));
            SetCaroMark(5, 4, "O", Color.FromArgb(217, 83, 79));
            SetCaroMark(5, 5, "X", Color.DeepSkyBlue);
            SetCaroMark(6, 5, "O", Color.FromArgb(217, 83, 79));
            SetCaroMark(6, 6, "X", Color.DeepSkyBlue);
        }

        private void SetCaroMark(int row, int col, string mark, Color color)
        {
            board[row, col].Text = mark;
            board[row, col].ForeColor = color;
        }

        // Người chơi rời phòng
        private void btnLeaveRoom_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

        // Xác nhận khi rời phòng 
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
        }

    }
}