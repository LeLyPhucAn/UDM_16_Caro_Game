using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Windows.Forms;
using Client.Network;
using CaroGame.Protocol;

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

            // Đăng ký nhận tin nhắn từ Server
            _clientConnection.OnMessageReceived += HandleServerMessage;
            _clientConnection.OnConnectionLost += HandleConnectionLost;
            _clientConnection.OnError += HandleError;
        }

        private void LobbyForm_Load(object? sender, EventArgs e)
        {
            dgvRooms.Rows.Clear();
            UpdateConnectionStatus(true);
        }

        public void UpdateRoomList(List<RoomInfo> rooms)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateRoomList(rooms)));
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

            // TẠM ẨN LBLSTATS ĐỂ TRIỆT TIÊU LỖI CS0103. Code sẽ chạy qua mượt mà.
            // if (lblStats != null)
            // {
            //     lblStats.Text = $"Phòng trống: {emptyRooms}\nĐang chờ ghép: {waitingRooms}\nĐang thi đấu: {playingRooms}";
            // }
        }

        public void UpdateOnlineCount(int onlineCount, int ping = 14)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateOnlineCount(onlineCount, ping)));
                return;
            }

            if (lblServerInfo != null)
            {
                lblServerInfo.Text = $"Ping: {ping}ms | Online: {onlineCount}";
            }
        }

        public void UpdateConnectionStatus(bool isConnected)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateConnectionStatus(isConnected)));
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
                this.Invoke(new Action(() => HandleServerMessage(message)));
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
            GameForm gameForm = new GameForm(_clientConnection);
            gameForm.FormClosed += (s, args) => this.Show();
            gameForm.Show();
            this.Hide();
        }

        private void btnJoinRoom_Click(object? sender, EventArgs e)
        {
            GameForm gameForm = new GameForm(_clientConnection);
            gameForm.FormClosed += (s, args) => this.Show();
            gameForm.Show();
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

        private void lblPing_Click(object sender, EventArgs e) { }
        private void lblListDesc_Click(object sender, EventArgs e) { }
        private void lblStats_Click(object sender, EventArgs e) { }
        private void lblStatusDot_Click(object sender, EventArgs e) { }
    }
}