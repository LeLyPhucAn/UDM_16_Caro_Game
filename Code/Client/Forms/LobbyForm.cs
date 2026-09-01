using CaroGame.Protocol;
using CaroGame.Protocol.Messages;
using Client.Controls;
using Client.Network;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Windows.Forms;


namespace Client.Forms
{
    public partial class LobbyForm : Form
    {
        private string _playerName;
        private ClientConnection _clientConnection;

        public LobbyForm(string playerName, ClientConnection clientConnection)
        {
            InitializeComponent();

            _playerName = playerName;
            _clientConnection = clientConnection;

            lblPlayerName.Text = _playerName;

            btnJoinRoom.Click += btnJoinRoom_Click;
            btnCreateRoom.Click += btnCreateRoom_Click;
            btnExitGame.Click += btnExitGame_Click;

            this.Load += LobbyForm_Load;

            // Đăng ký nhận tin nhắn từ Server
            _clientConnection.OnMessageReceived += HandleServerMessage;
            _clientConnection.OnConnectionLost += HandleConnectionLost;
            _clientConnection.OnError += HandleError;
        }

        private void LobbyForm_Load(object? sender, EventArgs e)
        {
            dgvRooms.Rows.Clear();
            UpdateConnectionStatus(true);

            // Gửi yêu cầu chạy ngầm, không chặn UI
            _ = Task.Run(async () =>
            {
                try
                {
                    var reqMsg = new RequestMessage
                    {
                        Type = MessageType.Request,
                        SenderId = _playerName,
                        Action = "RefreshLobby",
                        Data = ""
                    };
                    await _clientConnection.SendMessageAsync(reqMsg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi gửi RefreshLobby: {ex.Message}");
                }
            });
        }

        public void UpdateRoomList(List<RoomInfo> rooms)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => UpdateRoomList(rooms)));
                return;
            }

            dgvRooms.Rows.Clear();

            int emptyRooms = 0, waitingRooms = 0, playingRooms = 0;

            if (rooms != null && rooms.Count > 0)
            {
                foreach (var room in rooms)
                {
                    string playerCount = $"{room.CurrentPlayers}/{room.MaxPlayers}";
                    string statusText;
                    Color statusColor;

                    if (room.IsPlaying || room.CurrentPlayers >= room.MaxPlayers)
                    {
                        statusText = "● Đang chơi"; statusColor = Color.Orange;
                        playingRooms++;
                    }
                    else if (room.CurrentPlayers > 0)
                    {
                        statusText = "● Đang chờ"; statusColor = Color.LimeGreen;
                        waitingRooms++;
                    }
                    else
                    {
                        statusText = "● Trống"; statusColor = Color.Gray;
                        emptyRooms++;
                    }

                    int rowIndex = dgvRooms.Rows.Add(room.RoomId, room.RoomName, playerCount, statusText);
                    dgvRooms.Rows[rowIndex].Cells[0].Style.ForeColor = Color.DeepSkyBlue;
                    dgvRooms.Rows[rowIndex].Cells[3].Style.ForeColor = statusColor;
                }
            }

            if (lblStats != null)
            {
                lblStats.Text = $"Phòng trống: {emptyRooms}\nĐang chờ ghép: {waitingRooms}\nĐang thi đấu: {playingRooms}";
            }
        }

        // Đã xóa tham số ping không sử dụng
        public void UpdateOnlineCount(int onlineCount)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => UpdateOnlineCount(onlineCount)));
                return;
            }

            if (lblServerInfo != null)
            {
                lblServerInfo.Text = $"Online: {onlineCount}";
            }
        }

        public void UpdateConnectionStatus(bool isConnected)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => UpdateConnectionStatus(isConnected)));
                return;
            }

            if (lblConnection != null)
            {
                lblConnection.ForeColor = isConnected ? Color.LimeGreen : Color.Red;
            }
        }

        private void HandleServerMessage(BaseMessage message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => HandleServerMessage(message)));
                return;
            }

            try
            {
                if (message.Type == MessageType.Response && message is ResponseMessage response)
                {
                    if (response.Success && !string.IsNullOrEmpty(response.Data))
                    {
                        if (response.Data.Contains("OnlineCount"))
                        {
                            var lobbyState = System.Text.Json.JsonSerializer.Deserialize<LobbyStateDto>(response.Data);
                            if (lobbyState != null)
                            {
                                UpdateOnlineCount(lobbyState.OnlineCount);
                                UpdateRoomList(lobbyState.Rooms);

                                // 👉 3. GỌI CONTROL ĐỂ HIỂN THỊ DANH SÁCH NGƯỜI CHƠI LÊN MÀN HÌNH
                                if (playerListControl1 != null)
                                {
                                    playerListControl1.UpdateList(lobbyState.OnlinePlayers);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lobby Parse Error] {ex.Message}");
            }
        }

        private void HandleConnectionLost()
        {
            UpdateConnectionStatus(false);
            MessageBox.Show("Mất kết nối với máy chủ!", "Ngắt kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.Close();
        }

        private void HandleError(Exception ex) => Console.WriteLine($"[Lobby Error] {ex.Message}");

        // ======================================================
        // NÚT BẤM VÀO GAME
        // ======================================================
        private void btnCreateRoom_Click(object? sender, EventArgs e)
        {
            string roomName = $"Phòng của {_playerName}";
            // Báo cho Server biết để tạo phòng
            var requestMsg = new RequestMessage
            {
                Type = MessageType.Request,
                SenderId = _playerName,
                Action = "CreateRoom",
                Data = roomName
            };

            _ = Task.Run(async () =>
            {
                try { await _clientConnection.SendMessageAsync(requestMsg); }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            });

            // Mở màn hình Game
            RoomForm roomForm = new RoomForm(_clientConnection, roomName, _playerName, true); // true = Chủ phòng
            roomForm.FormClosed += (s, args) => this.Show();
            roomForm.Show();
            this.Hide();
        }

        private void btnJoinRoom_Click(object? sender, EventArgs e)
        {
            // 1. Kiểm tra xem người chơi đã click chọn dòng nào trên bảng chưa
            if (dgvRooms.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng click chọn một phòng trong danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 👉 1. ĐỌC TRẠNG THÁI PHÒNG TỪ CỘT SỐ 3 (Cells[3])
            string roomStatus = dgvRooms.SelectedRows[0].Cells[3].Value?.ToString() ?? "";

            // 👉 2. LẬP CHỐT CHẶN: NẾU PHÒNG ĐÃ KÍN CHỖ THÌ TỪ CHỐI
            if (roomStatus.Contains("Đang chơi") || roomStatus.Contains("Đã đầy"))
            {
                MessageBox.Show("Phòng này đã đủ người hoặc đang thi đấu. Vui lòng chọn phòng khác!", "Từ chối", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                return; // Lệnh return này sẽ dừng ngay lập tức, không cho phép gửi tin lên Server và KHÔNG mở GameForm!
            }
            // 2. Trích xuất "Mã Phòng" từ cột đầu tiên (Cells[0]) của dòng đang chọn
            string selectedRoomId = dgvRooms.SelectedRows[0].Cells[0].Value?.ToString() ?? "";
            string selectedRoomName = dgvRooms.SelectedRows[0].Cells[1].Value?.ToString() ?? "Phòng ẩn";

            // 3. Đóng gói lệnh xin gia nhập và gửi lên Server
            var requestMsg = new RequestMessage
            {
                Type = MessageType.Request,
                SenderId = _playerName,
                Action = "JoinRoom",
                Data = selectedRoomId // Gửi kèm Mã Phòng
            };

            _ = Task.Run(async () =>
            {
                try { await _clientConnection.SendMessageAsync(requestMsg); }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            });

            // 4. Chuyển sang màn hình thi đấu
            RoomForm roomForm = new RoomForm(_clientConnection, selectedRoomName, _playerName, false); // false = Khách
            roomForm.FormClosed += (s, args) => this.Show();
            roomForm.Show();
            this.Hide();
        }

        private void btnExitGame_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn thoát?", "Thoát", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _clientConnection.Disconnect();
                Application.Exit();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _clientConnection.OnMessageReceived -= HandleServerMessage;
            _clientConnection.OnConnectionLost -= HandleConnectionLost;
            base.OnFormClosed(e);
        }

        private void dgvRooms_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void playerListControl1_Load(object sender, EventArgs e)
        {

        }
    }
}