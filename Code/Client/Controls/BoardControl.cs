using System;
using System.Drawing;
using System.Windows.Forms;

namespace Client.Controls
{
    public class BoardControl : UserControl
    {
        private int _rows = 15;
        private int _cols = 15;
        private int _cellSize = 30; // Giảm kích cỡ để 20x20 không bị tràn

        private Button[,] _board;

        // Tạo sự kiện để báo cho GameForm biết khi có người click vào 1 ô
        public event Action<int, int>? OnCellClicked;

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool IsSpectatorMode { get; set; } = false;

        public BoardControl()
        {
            this.DoubleBuffered = true;
            _board = new Button[_rows, _cols];
            this.Size = new Size(_cols * _cellSize, _rows * _cellSize);
            // Để trống, gọi InitializeBoard(size) từ bên ngoài
        }

        public void InitializeBoard(int boardSize)
        {
            _rows = boardSize;
            _cols = boardSize;
            
            // Tính toán cell size phù hợp để không vượt quá khoảng 600px
            _cellSize = Math.Min(40, 600 / boardSize);
            
            this.Size = new Size(_cols * _cellSize, _rows * _cellSize);
            _board = new Button[_rows, _cols];

            this.Controls.Clear();

            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    Button btn = new Button
                    {
                        Size = new Size(_cellSize, _cellSize),
                        Location = new Point(j * _cellSize, i * _cellSize),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.FromArgb(34, 36, 40),
                        Font = new Font("Segoe UI", _cellSize / 2.5f, FontStyle.Bold),
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
            if (IsSpectatorMode) return;
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

            if (row < 0 || row >= _rows || col < 0 || col >= _cols) return;

            _board[row, col].Text = mark;
            _board[row, col].ForeColor = (mark == "X")
                ? Color.DeepSkyBlue
                : Color.FromArgb(217, 83, 79);
        }
    }
}