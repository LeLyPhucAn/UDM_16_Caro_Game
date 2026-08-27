using System;
using System.Drawing;
using System.Text.Json;
using System.Windows.Forms;
using Client.Network; // Giao tiếp mạng
using CaroGame.Protocol; // Gói tin Protocol

namespace Client.Forms
{
    public partial class GameForm : Form
    {
        private const int ROWS = 10;
        private const int COLS = 10;
        private const int CELL_SIZE = 50;

        // Khởi tạo mảng ngay lúc khai báo để tránh lỗi null
        private Button[,] _board = new Button[ROWS, COLS];

        // Quản lý Mạng và Trạng thái ván đấu
        private ClientConnection _clientConnection;
        private bool _isMyTurn = false; // Mặc định khóa bàn cờ, chờ Server cấp quyền mới mở
        private string _mySymbol = "";  // Sẽ được điền khi nhận GameSyncMessage

        // Yêu cầu truyền ClientConnection vào từ Lobby
        public GameForm(ClientConnection clientConnection)
        {
            InitializeComponent();

            _clientConnection = clientConnection;

            // Đăng ký nhận tin nhắn khi đang trong phòng chơi
            _clientConnection.OnMessageReceived += HandleGameMessage;
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            DrawBoard();
            //LoadDummyData();
        }

        // ======================================================
        // PHẦN 1: TẠO BÀN CỜ & BẮT SỰ KIỆN CLICK
        // ======================================================

        private void DrawBoard()
        {
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

                    // 1. LƯU TỌA ĐỘ VÀO NÚT
                    btn.Tag = new Point(i, j);

                    // 2. GẮN SỰ KIỆN CLICK
                    btn.Click += Cell_Click;

                    pnlBoard.Controls.Add(btn);
                    _board[i, j] = btn;
                }
            }
        }

        private async void Cell_Click(object? sender, EventArgs e)
        {
            Button? clickedBtn = sender as Button;
            if (clickedBtn == null) return;

            // 1. Kiểm tra tính hợp lệ cục bộ
            if (!_isMyTurn) return; // Chưa đến lượt
            if (!string.IsNullOrEmpty(clickedBtn.Text)) return; // Ô đã có người đánh

            // Lấy tọa độ
            if (clickedBtn.Tag == null) return;
            Point pos = (Point)clickedBtn.Tag;

            // 2. Đóng gói dữ liệu thành MoveMessage
            var moveMsg = new MoveMessage
            {
                Row = pos.X,
                Col = pos.Y,
                Symbol = _mySymbol
            };

            // 3. Gửi thẳng đối tượng Message qua mạng
            try
            {
                await _clientConnection.SendMessageAsync(moveMsg);

                // 4. Khóa bàn cờ ngay lập tức để chờ Server phản hồi (Chống Spam click)
                _isMyTurn = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi gửi nước đi: " + ex.Message);
            }
        }

        // ======================================================
        // PHẦN 2: CẬP NHẬT GIAO DIỆN & DỮ LIỆU
        // ======================================================

        // Hàm Thread-Safe để Server có thể gọi an toàn
        public void UpdateBoardUI(int row, int col, string mark)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateBoardUI(row, col, mark)));
                return;
            }

            _board[row, col].Text = mark;
            _board[row, col].ForeColor = (mark == "X") ? Color.DeepSkyBlue : Color.FromArgb(217, 83, 79);
        }

        private void LoadDummyData()
        {
            rtbChatHistory.AppendText("Minh456: Chào bạn nhé, chúc chơi vui vẻ!\n\n");
            rtbChatHistory.AppendText("Nam123: Chào bạn, tí nương tay nha haha\n\n");
            rtbChatHistory.AppendText("Minh456: Góc kia nước đi hay đấy!\n\n");

            UpdateBoardUI(3, 3, "X");
            UpdateBoardUI(4, 4, "O");
            UpdateBoardUI(5, 4, "O");
            UpdateBoardUI(5, 5, "X");
            UpdateBoardUI(6, 5, "O");
            UpdateBoardUI(6, 6, "X");
        }

        // ======================================================
        // PHẦN 3: GIAO TIẾP SERVER TRONG GAME & THOÁT
        // ======================================================

        private void HandleGameMessage(BaseMessage message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => HandleGameMessage(message)));
                return;
            }

            try
            {
                // ==========================================
                // 1. XỬ LÝ LÚC VỪA VÀO PHÒNG (BƯỚC 3)
                // ==========================================
                if (message.Type == MessageType.GameSync && message is GameSyncMessage syncMsg)
                {
                    // Cập nhật giao diện Label
                    lblCurrentTurn.Text = $"X: {syncMsg.PlayerXName}";
                    lblPlayerO.Text = $"O: {syncMsg.PlayerOName}";

                    // ĐÃ SỬA: Lấy cờ từ Server thay vì so sánh tên
                    _mySymbol = syncMsg.MySymbol;

                    // Người cầm cờ X luôn được đi trước
                    _isMyTurn = (_mySymbol == "X");

                    // Cập nhật giao diện lượt đi ban đầu
                    lblCurrentTurn.Text = $"Lượt đi hiện tại: X ({syncMsg.CurrentTurnName})";
                }

                // ==========================================
                // 2. XỬ LÝ KHI CÓ NGƯỜI ĐÁNH CỜ
                // ==========================================
                else if (message.Type == MessageType.Move && message is MoveMessage moveMsg)
                {
                    // Vẽ quân cờ lên UI
                    UpdateBoardUI(moveMsg.Row, moveMsg.Col, moveMsg.Symbol);

                    // Đảo lượt nội bộ
                    _isMyTurn = (moveMsg.Symbol != _mySymbol);

                    // Đổi thông báo lượt đi trên giao diện
                    if (moveMsg.Symbol == "X")
                    {
                        // X vừa đánh xong -> Chuyển sang lượt của O
                        string playerOName = lblPlayerO.Text.Replace("O: ", "");
                        lblCurrentTurn.Text = $"Lượt đi hiện tại: O ({playerOName})";
                    }
                    else
                    {
                        // O vừa đánh xong -> Chuyển sang lượt của X
                        string playerXName = lblCurrentTurn.Text.Replace("X: ", "");
                        lblCurrentTurn.Text = $"Lượt đi hiện tại: X ({playerXName})";
                    }
                }

                // ==========================================
                // 3. XỬ LÝ KẾT THÚC TRẬN ĐẤU (BƯỚC 4)
                // ==========================================
                else if (message.Type == MessageType.GameOver && message is GameOverMessage gameOverMsg)
                {
                    // 1. Khóa vĩnh viễn bàn cờ không cho click nữa
                    _isMyTurn = false;

                    // 2. Đổi nhãn trạng thái
                    lblCurrentTurn.Text = "Trận đấu kết thúc!";
                    lblCurrentTurn.ForeColor = Color.Yellow; // Đổi màu cho nổi bật (Tùy chọn)

                    // 3. Hiển thị bảng thông báo kết quả
                    if (gameOverMsg.ResultType == "Win")
                    {
                        MessageBox.Show($"Chúc mừng người chơi [{gameOverMsg.WinnerName}] đã giành chiến thắng!",
                                        "Kết thúc ván đấu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (gameOverMsg.ResultType == "Draw")
                    {
                        MessageBox.Show("Ván đấu hòa! Không còn ô trống nào trên bàn cờ.",
                                        "Kết thúc ván đấu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi giao diện: {ex.Message}\nData: {message.Type}", "Lỗi Client");
            }
        }

        private void btnLeaveRoom_Click(object sender, EventArgs e)
        {
            this.Close();
        }

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
            else
            {
                // Ngắt sự kiện lắng nghe để tránh lỗi rò rỉ bộ nhớ
                _clientConnection.OnMessageReceived -= HandleGameMessage;
            }
        }

        private void lblPlayerX_Click(object sender, EventArgs e)
        {

        }
    }
}