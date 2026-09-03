using System;
using System.Drawing;
using System.Windows.Forms;

namespace Client.Controls
{
    public class BoardControl : UserControl
    {
        private const int ROWS = 10;
        private const int COLS = 10;
        private const int CELL_SIZE = 50;

        // Khởi tạo mảng ngay lúc khai báo để tránh lỗi null
        private readonly Button[,] _board = new Button[ROWS, COLS];

        // Tạo sự kiện để báo cho GameForm biết khi có người click vào 1 ô
        public event Action<int, int>? OnCellClicked;

        public BoardControl()
        {
            this.DoubleBuffered = true;
            this.Size = new Size(COLS * CELL_SIZE, ROWS * CELL_SIZE);
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            this.Controls.Clear();

            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLS; j++)
                {
                    Button btn = new Button
                    {
                        Size = new Size(CELL_SIZE, CELL_SIZE),
                        Location = new Point(j * CELL_SIZE, i * CELL_SIZE),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.FromArgb(34, 36, 40),
                        Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                        Cursor = Cursors.Hand,
                        Tag = new Point(i, j) // LƯU TỌA ĐỘ VÀO NÚT
                    };

                    btn.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
                    btn.FlatAppearance.BorderSize = 1;
                    btn.Click += Btn_Click;

                    this.Controls.Add(btn);
                    _board[i, j] = btn;
                }
            }
        }

        private void Btn_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn || btn.Tag == null) return;
            if (!string.IsNullOrEmpty(btn.Text)) return; // Ô đã có người đánh

            Point pos = (Point)btn.Tag;

            // Bắn tọa độ ra ngoài cho GameForm xử lý
            OnCellClicked?.Invoke(pos.X, pos.Y);
        }

        // Hàm Thread-Safe để cập nhật UI
        public void UpdateBoardUI(int row, int col, string mark)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateBoardUI(row, col, mark)));
                return;
            }

            if (row < 0 || row >= ROWS || col < 0 || col >= COLS) return;

            _board[row, col].Text = mark;
            _board[row, col].ForeColor = (mark == "X")
                ? Color.DeepSkyBlue
                : Color.FromArgb(217, 83, 79);
        }
    }
}