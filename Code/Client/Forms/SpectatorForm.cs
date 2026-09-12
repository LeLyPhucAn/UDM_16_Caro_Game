using CaroGame.Protocol.Messages;
using CaroGame.Protocol.Messages.Game;
using Client.Controls;
using Client.Network;
using System;
using System.Windows.Forms;

namespace Client.Forms
{
    public partial class SpectatorForm : Form
    {
        private readonly ClientConnection _clientConnection;
        private readonly string _roomId;

        public SpectatorForm(string roomId, ClientConnection clientConnection)
        {
            InitializeComponent();
            _roomId = roomId;
            _clientConnection = clientConnection;

            boardControl1.IsSpectatorMode = true;

            this.Load += SpectatorForm_Load;
            this.FormClosed += SpectatorForm_FormClosed;
        }

        private void SpectatorForm_Load(object? sender, EventArgs e)
        {
            lblStatus.Text = "Đang kết nối để xem trận đấu...";

            // Đăng ký nhận tin nhắn từ Server
            _clientConnection.OnMessageReceived += HandleSpectatorMessage;
        }

        private void HandleSpectatorMessage(BaseMessage message)
        {
            // Bảo vệ luồng UI bằng BeginInvoke (Bài học xương máu từ LoginForm!)
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => HandleSpectatorMessage(message)));
                return;
            }

            // 1. Đồng bộ thông tin trận đấu khi vừa vào xem
            if (message.Type == MessageType.GameSync && message is GameSyncMessage syncMsg)
            {
                lblPlayerX.Text = $"Quân X: {syncMsg.PlayerXName}";
                lblPlayerO.Text = $"Quân O: {syncMsg.PlayerOName}";
                lblStatus.Text = $"Lượt hiện tại: {syncMsg.CurrentTurnName}";

                // Lưu ý: Khán giả không có MySymbol nên không cần lưu
            }
            // 2. Nhận bước đi của 2 người chơi và vẽ lên bàn cờ
            else if (message.Type == MessageType.Move && message is MoveMessage moveMsg)
            {
                // Gọi hàm vẽ chữ X hoặc O lên UI thông qua BoardControl
                boardControl1.UpdateBoardUI(moveMsg.Row, moveMsg.Column, moveMsg.Symbol);
            }
            // 3. Đổi lượt đánh
            else if (message.Type == MessageType.Turn && message is TurnMessage turnMsg)
            {
                lblStatus.Text = $"Lượt hiện tại: {turnMsg.CurrentPlayerId}";
            }
            // 4. Trận đấu kết thúc
            else if (message.Type == MessageType.GameOver && message is GameOverMessage overMsg)
            {
                string resultText = overMsg.ResultType == "Draw"
                    ? "Trận đấu HÒA!"
                    : $"Người thắng: {overMsg.WinnerName}";

                lblStatus.Text = "ĐÃ KẾT THÚC";
                MessageBox.Show(resultText, "Kết thúc trận đấu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void btnExit_Click(object sender, EventArgs e)
        {
            // Gửi thông báo cho Server biết khán giả này rời phòng (nếu Server có quản lý lượng view)
            var leaveMsg = new CaroGame.Protocol.Messages.RequestMessage
            {
                Action = "SpectatorLeave",
                Data = _roomId
            };
            await _clientConnection.SendMessageAsync(leaveMsg);

            this.Close();
        }

        private void SpectatorForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            // Gỡ đăng ký sự kiện để tránh rò rỉ bộ nhớ
            _clientConnection.OnMessageReceived -= HandleSpectatorMessage;

            // Mở lại form trước đó (LobbyForm hoặc RoomForm)
            // Code tùy thuộc vào luồng của bạn
        }

        private void SpectatorForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}