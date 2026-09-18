using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Client.Controls
{
    public class BoardControl : UserControl
    {
        private int _rows = 15;
        private int _cols = 15;
        private int _cellSize = 40;
        private string[,] _grid;
        private List<Point> _winningCells = new List<Point>();
        
        private Point _hoveredCell = new Point(-1, -1);
        private bool _isMyTurn = true; // Để biết có được hover highlight không
        
        // Cấu hình màu sắc
        private readonly Color _lineColor = Color.FromArgb(60, 60, 60);
        private readonly Color _bgColor = Color.FromArgb(34, 36, 40);
        private readonly Color _hoverColor = Color.FromArgb(50, 50, 55);
        private readonly Color _xColor = Color.DeepSkyBlue;
        private readonly Color _oColor = Color.FromArgb(217, 83, 79);
        private readonly Color _winBgColor = Color.FromArgb(60, 241, 196, 15); // Vàng trong suốt
        private readonly Color _winLineColor = Color.FromArgb(217, 83, 79); // Đỏ kẻ xuyên qua
        
        private Font _markFont;

        public event Action<int, int>? OnCellClicked;

        public BoardControl()
        {
            this.DoubleBuffered = true; // Chống nháy hình (flickering) khi vẽ lại liên tục
            this.BackColor = _bgColor;
            this.Cursor = Cursors.Hand;
            _grid = new string[15, 15]; // Default
            _markFont = new Font("Segoe UI", _cellSize / 2.5f, FontStyle.Bold);
            
            this.MouseMove += BoardControl_MouseMove;
            this.MouseLeave += BoardControl_MouseLeave;
            this.MouseClick += BoardControl_MouseClick;
        }

        public void InitializeBoard(int boardSize)
        {
            _rows = boardSize;
            _cols = boardSize;
            
            _cellSize = Math.Min(40, this.Parent != null ? Math.Min(this.Parent.Width, this.Parent.Height) / boardSize : 600 / boardSize);
            this.Size = new Size(_cols * _cellSize, _rows * _cellSize);
            
            _grid = new string[_rows, _cols];
            _winningCells.Clear();
            _markFont?.Dispose();
            _markFont = new Font("Segoe UI", _cellSize / 2.5f, FontStyle.Bold);
            
            this.Invalidate(); // Yêu cầu vẽ lại
        }

        public void SetMyTurn(bool isMyTurn)
        {
            _isMyTurn = isMyTurn;
        }

        private void BoardControl_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!_isMyTurn) return;
            
            int c = e.X / _cellSize;
            int r = e.Y / _cellSize;

            if (r >= 0 && r < _rows && c >= 0 && c < _cols)
            {
                if (_hoveredCell.X != r || _hoveredCell.Y != c)
                {
                    _hoveredCell = new Point(r, c);
                    this.Invalidate(); // Vẽ lại hiệu ứng hover
                }
            }
        }

        private void BoardControl_MouseLeave(object? sender, EventArgs e)
        {
            if (_hoveredCell.X != -1)
            {
                _hoveredCell = new Point(-1, -1);
                this.Invalidate();
            }
        }

        private void BoardControl_MouseClick(object? sender, MouseEventArgs e)
        {
            int c = e.X / _cellSize;
            int r = e.Y / _cellSize;

            if (r >= 0 && r < _rows && c >= 0 && c < _cols)
            {
                if (string.IsNullOrEmpty(_grid[r, c])) // Chỗ này trống
                {
                    OnCellClicked?.Invoke(r, c);
                }
            }
        }

        public void UpdateBoardUI(int row, int col, string mark)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateBoardUI(row, col, mark)));
                return;
            }

            if (row < 0 || row >= _rows || col < 0 || col >= _cols) return;

            _grid[row, col] = mark;
            this.Invalidate(); // Cập nhật lại UI
        }

        public void HighlightWinningCells(IEnumerable<Point> winningCells)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => HighlightWinningCells(winningCells)));
                return;
            }

            _winningCells.Clear();
            _winningCells.AddRange(winningCells);
            this.Invalidate();
        }

        // ============================================
        // HÀM VẼ GIAO DIỆN (GDI+) CHÍNH
        // ============================================
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias; // Làm mượt nét vẽ

            // 1. Vẽ Hover Background
            if (_hoveredCell.X >= 0 && _hoveredCell.X < _rows && _hoveredCell.Y >= 0 && _hoveredCell.Y < _cols)
            {
                if (string.IsNullOrEmpty(_grid[_hoveredCell.X, _hoveredCell.Y]))
                {
                    using (var brush = new SolidBrush(_hoverColor))
                    {
                        g.FillRectangle(brush, _hoveredCell.Y * _cellSize, _hoveredCell.X * _cellSize, _cellSize, _cellSize);
                    }
                }
            }

            // 2. Vẽ Nền 5 ô chiến thắng
            if (_winningCells.Count >= 5)
            {
                using (var winBrush = new SolidBrush(_winBgColor))
                {
                    foreach (var pt in _winningCells)
                    {
                        if (pt.X >= 0 && pt.X < _rows && pt.Y >= 0 && pt.Y < _cols)
                        {
                            g.FillRectangle(winBrush, pt.Y * _cellSize, pt.X * _cellSize, _cellSize, _cellSize);
                        }
                    }
                }
            }

            // 3. Vẽ Lưới (Grid)
            using (Pen gridPen = new Pen(_lineColor, 1))
            {
                for (int i = 0; i <= _rows; i++)
                {
                    int y = i == _rows ? i * _cellSize - 1 : i * _cellSize;
                    g.DrawLine(gridPen, 0, y, _cols * _cellSize, y); // Đường ngang
                }
                for (int j = 0; j <= _cols; j++)
                {
                    int x = j == _cols ? j * _cellSize - 1 : j * _cellSize;
                    g.DrawLine(gridPen, x, 0, x, _rows * _cellSize); // Đường dọc
                }
            }

            // 4. Vẽ Chữ X / O
            using (var format = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                for (int r = 0; r < _rows; r++)
                {
                    for (int c = 0; c < _cols; c++)
                    {
                        string mark = _grid[r, c];
                        if (!string.IsNullOrEmpty(mark))
                        {
                            RectangleF rect = new RectangleF(c * _cellSize, r * _cellSize, _cellSize, _cellSize);
                            using (var brush = new SolidBrush(mark == "X" ? _xColor : _oColor))
                            {
                                g.DrawString(mark, _markFont, brush, rect, format);
                            }
                        }
                    }
                }
            }

            // 5. Vẽ Đường kẻ đỏ xuyên qua 5 ô chiến thắng
            if (_winningCells.Count >= 5)
            {
                // Lấy ô đầu và ô cuối để vẽ đường kẻ xuyên qua tâm
                var first = _winningCells[0];
                var last = _winningCells[_winningCells.Count - 1];

                int startX = first.Y * _cellSize + _cellSize / 2;
                int startY = first.X * _cellSize + _cellSize / 2;
                int endX = last.Y * _cellSize + _cellSize / 2;
                int endY = last.X * _cellSize + _cellSize / 2;

                using (Pen winLinePen = new Pen(_winLineColor, 4))
                {
                    winLinePen.StartCap = LineCap.Round;
                    winLinePen.EndCap = LineCap.Round;
                    g.DrawLine(winLinePen, startX, startY, endX, endY);
                }
            }
        }
    }
}