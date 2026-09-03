using System;
using System.Drawing;
using System.Windows.Forms;
using Client.Network;       // Giao tiếp mạng
using CaroGame.Protocol;    // Gói tin Protocol
using CaroGame.Protocol.Messages;
using Client.Controls;      // Dùng UserControl BoardControl
using CaroGame.Protocol.Messages.Game;
using CaroGame.Protocol.Messages.Response;

namespace Client.Forms
{
    public partial class GameForm : Form
    {
        // Quản lý Mạng, Bàn cờ và Trạng thái ván đấu
        private ClientConnection _clientConnection;
        private BoardControl _boardControl = null!;

        private bool _isMyTurn = false; // Mặc định khóa bàn cờ, chờ Server cấp quyền
        private string _mySymbol = "";  // Sẽ được điền khi nhận GameSyncMessage

        // 👉 BỔ SUNG: Khai báo đủ các biến lưu trữ
        private string _roomName;
        private string _playerName;
        private bool _isHost;

        // 👉 CHỈNH SỬA: Hàm khởi tạo giờ đã nhận đủ 4 tham số
        public GameForm(ClientConnection connection, string roomName, string playerName, bool isHost)
        {
            InitializeComponent();

            // Gán dữ liệu vào các biến toàn cục của Form
            _clientConnection = connection;
            _roomName = roomName;
            _playerName = playerName;
            _isHost = isHost;

            this.Text = "Caro Arena - " + _roomName;

            // Đăng ký nhận tin nhắn khi đang trong phòng chơi
            _clientConnection.OnMessageReceived += HandleGameMessage;
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            SetupBoardControl();
            //LoadDummyData();
        }

        // ======================================================
        // PHẦN 1: TẠO BÀN CỜ & GỬI DỮ LIỆU
        // ======================================================

        private void SetupBoardControl()
        {
            _boardControl = new BoardControl();

            // Lắng nghe sự kiện click từ BoardControl để gửi mạng
            _boardControl.OnCellClicked += async (row, col) =>
            {
                if (!_isMyTurn) return; // Chưa đến lượt

                var moveMsg = new MoveMessage
                {
                    Row = row,
                    Column = col,
                    Symbol = _mySymbol
                };

                try
                {
                    await _clientConnection.SendMessageAsync(moveMsg);
                    _isMyTurn = false; // Khóa bàn cờ ngay lập tức để chờ Server phản hồi
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi gửi nước đi: " + ex.Message);
                }
            };

            // Gắn bàn cờ vào Panel hiện có trên Giao diện
            pnlBoard.Controls.Clear();
            pnlBoard.Controls.Add(_boardControl);
        }

        // ======================================================
        // PHẦN 2: CẬP NHẬT GIAO DIỆN & DỮ LIỆU
        // ======================================================

        private void LoadDummyData()
        {
            rtbChatHistory.AppendText("Minh456: Chào bạn nhé, chúc chơi vui vẻ!\n\n");
            rtbChatHistory.AppendText("Nam123: Chào bạn, tí nương tay nha haha\n\n");
            rtbChatHistory.AppendText("Minh456: Góc kia nước đi hay đấy!\n\n");

            _boardControl.UpdateBoardUI(3, 3, "X");
            _boardControl.UpdateBoardUI(4, 4, "O");
            _boardControl.UpdateBoardUI(5, 4, "O");
            _boardControl.UpdateBoardUI(5, 5, "X");
            _boardControl.UpdateBoardUI(6, 5, "O");
            _boardControl.UpdateBoardUI(6, 6, "X");
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
                // 1. XỬ LÝ LÚC VỪA VÀO PHÒNG
                // ==========================================
                if (message.Type == MessageType.GameState && message is GameStateMessage syncMsg)
                {
                    // Cập nhật giao diện Label
                    lblPlayerX.Text = $"X: {syncMsg.PlayerXName}"; // Đã sửa tên label cho chuẩn
                    lblPlayerO.Text = $"O: {syncMsg.PlayerOName}";

                    // Lưu ký hiệu của mình (X hoặc O)
                    _mySymbol = syncMsg.MySymbol;

                    // Kiểm tra lượt: Nếu tên CurrentTurnName trùng với tên mình thì mở khóa bàn cờ
                    if (syncMsg.CurrentTurnName == _playerName) _isMyTurn = true;

                    // Cập nhật giao diện lượt đi ban đầu
                    lblPlayerX.Text = $"Lượt đi hiện tại: X ({syncMsg.CurrentTurnName})";
                }

                // ==========================================
                // 2. XỬ LÝ KHI CÓ NGƯỜI ĐÁNH CỜ
                // ==========================================
                else if (message.Type == MessageType.Move && message is MoveMessage moveMsg)
                {
                    // Vẽ quân cờ lên UI thông qua BoardControl
                    _boardControl.UpdateBoardUI(moveMsg.Row, moveMsg.Column, moveMsg.Symbol);

                    // Đảo lượt nội bộ
                    _isMyTurn = (moveMsg.Symbol != _mySymbol);

                    // Đổi thông báo lượt đi trên giao diện
                    if (moveMsg.Symbol == "X")
                    {
                        string playerOName = lblPlayerO.Text.Replace("O: ", "");
                        lblPlayerX.Text = $"Lượt đi hiện tại: O ({playerOName})";
                    }
                    else
                    {
                        string playerXName = lblPlayerX.Text.Replace("X: ", "");
                        lblPlayerX.Text = $"Lượt đi hiện tại: X ({playerXName})";
                    }
                }

                // ==========================================
                // 3. XỬ LÝ KẾT THÚC TRẬN ĐẤU
                // ==========================================
                else if (message.Type == MessageType.GameOver && message is GameOverMessage gameOverMsg)
                {
                    _isMyTurn = false;

                    lblPlayerX.Text = "Trận đấu kết thúc!";
                    lblPlayerX.ForeColor = Color.Yellow;

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
                var requestMsg = new RequestMessage
                {
                    Type = MessageType.Request,
                    SenderId = "",
                    Action = "LeaveRoom",
                    Data = ""
                };

                _ = Task.Run(async () => {
                    try { await _clientConnection.SendMessageAsync(requestMsg); }
                    catch { /* Bỏ qua lỗi nếu ngắt kết nối mạng rồi */ }
                });

                // Ngắt sự kiện lắng nghe để tránh lỗi rò rỉ bộ nhớ
                _clientConnection.OnMessageReceived -= HandleGameMessage;
            }
        }

        // Giữ lại event handler gốc để file Designer.cs không bị lỗi
        private void lblPlayerX_Click(object sender, EventArgs e)
        {
        }
    }
}