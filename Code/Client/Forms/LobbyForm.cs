using CaroGame.Protocol;
using CaroGame.Protocol.Messages;
using Client.Controls;
using Client.Network;
using CaroGame.Protocol.Messages.Room; // Thêm dòng này
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Windows.Forms;
using CaroGame.Protocol.Messages.Game;
using CaroGame.Protocol.Messages.Response;
using CaroGame.Protocol.Messages.History;


namespace Client.Forms
{
    public partial class LobbyForm : Form
    {
        private string _playerName;
        private ClientConnection _clientConnection;
        private Button? btnHistory;

        // Profile UI Controls in TopBar
        private Panel? pnlAvatar;
        private Label? lblAvatarChar;
        private Label? lblBadgeScore;
        private Label? lblBadgeWins;
        private Label? lblBadgeDraws;
        private Label? lblBadgeLosses;
        private Label? lblBadgeWinRate;

        public LobbyForm(string playerName, ClientConnection clientConnection)
        {
            InitializeComponent();

            _playerName = playerName;
            _clientConnection = clientConnection;

            SetupProfileHeaderUI();

            btnJoinRoom.Click += btnJoinRoom_Click;
            btnCreateRoom.Click += btnCreateRoom_Click;
            btnExitGame.Click += btnExitGame_Click;

            this.Load += LobbyForm_Load;

            // Đăng ký nhận tin nhắn từ Server
            _clientConnection.OnMessageReceived += HandleServerMessage;
            _clientConnection.OnConnectionLost += HandleConnectionLost;
            _clientConnection.OnError += HandleError;
        }

        private void SetupProfileHeaderUI()
        {
            pnlTopBar.Height = 85;

            // Ẩn nhãn "Người chơi:" cũ để giao diện gọn gàng, hiện đại
            if (lblPlayerTitle != null) lblPlayerTitle.Visible = false;

            // Khung Avatar hiện đại (50x50px)
            pnlAvatar = new Panel
            {
                Size = new Size(50, 50),
                Location = new Point(20, 17),
                BackColor = Color.FromArgb(41, 128, 185)
            };

            lblAvatarChar = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                Text = !string.IsNullOrEmpty(_playerName) ? _playerName.Substring(0, 1).ToUpper() : "U"
            };
            pnlAvatar.Controls.Add(lblAvatarChar);
            pnlTopBar.Controls.Add(pnlAvatar);

            // Đặt chấm kết nối nhỏ ở góc dưới của Avatar
            if (lblConnection != null)
            {
                lblConnection.Font = new Font("Segoe UI", 10F);
                lblConnection.Location = new Point(56, 48);
                lblConnection.BringToFront();
            }

            // Tên người chơi
            if (lblPlayerName != null)
            {
                lblPlayerName.Location = new Point(80, 16);
                lblPlayerName.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
                lblPlayerName.ForeColor = Color.DeepSkyBlue;
                lblPlayerName.Text = _playerName;
            }

            // Stat Badges (Điểm, Thắng, Hòa, Thua, Tỉ lệ)
            lblBadgeScore = CreateStatBadge("🏆 Điểm: 0", Color.FromArgb(50, 52, 60), Color.FromArgb(220, 220, 220), new Point(80, 48));
            lblBadgeWins = CreateStatBadge("🟢 Thắng: 0", Color.FromArgb(25, 60, 40), Color.FromArgb(46, 204, 113), new Point(190, 48));
            lblBadgeDraws = CreateStatBadge("🟡 Hòa: 0", Color.FromArgb(65, 55, 25), Color.FromArgb(241, 196, 15), new Point(295, 48));
            lblBadgeLosses = CreateStatBadge("🔴 Thua: 0", Color.FromArgb(65, 30, 30), Color.FromArgb(231, 76, 60), new Point(390, 48));
            lblBadgeWinRate = CreateStatBadge("📊 Tỉ lệ: 0%", Color.FromArgb(30, 50, 70), Color.FromArgb(52, 152, 219), new Point(485, 48));

            pnlTopBar.Controls.Add(lblBadgeScore);
            pnlTopBar.Controls.Add(lblBadgeWins);
            pnlTopBar.Controls.Add(lblBadgeDraws);
            pnlTopBar.Controls.Add(lblBadgeLosses);
            pnlTopBar.Controls.Add(lblBadgeWinRate);

            // Nút LỊCH SỬ ĐẤU trên TopBar bên cạnh nút THOÁT GAME
            btnHistory = new Button
            {
                Text = "📜 LỊCH SỬ ĐẤU",
                Size = new Size(135, 38),
                BackColor = Color.FromArgb(106, 90, 205),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(pnlTopBar.Width - 300, 23),
                Cursor = Cursors.Hand
            };
            btnHistory.FlatAppearance.BorderSize = 0;
            btnHistory.Click += BtnHistory_Click;
            pnlTopBar.Controls.Add(btnHistory);

            // Căn chỉnh lại nút Thoát và ServerInfo
            if (btnExitGame != null)
            {
                btnExitGame.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                btnExitGame.Size = new Size(130, 38);
                btnExitGame.Location = new Point(pnlTopBar.Width - 150, 23);
                btnExitGame.Cursor = Cursors.Hand;
            }

            if (lblServerInfo != null)
            {
                lblServerInfo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                lblServerInfo.Location = new Point(pnlTopBar.Width - 410, 31);
            }
        }

        private Label CreateStatBadge(string text, Color backColor, Color foreColor, Point location)
        {
            return new Label
            {
                Text = text,
                Location = location,
                AutoSize = true,
                BackColor = backColor,
                ForeColor = foreColor,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Padding = new Padding(5, 3, 5, 3),
                Margin = new Padding(0)
            };
        }

        private void RequestProfile()
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    var reqMsg = new RequestMessage
                    {
                        Type = MessageType.Request,
                        SenderId = _playerName,
                        Action = "GetProfile",
                        Data = _playerName
                    };
                    await _clientConnection.SendMessageAsync(reqMsg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Lobby RequestProfile Error] {ex.Message}");
                }
            });
        }

        private void UpdateProfileUI(UserProfileDto profile)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => UpdateProfileUI(profile)));
                return;
            }

            if (lblBadgeScore != null) lblBadgeScore.Text = $"🏆 Điểm: {profile.Score}";
            if (lblBadgeWins != null) lblBadgeWins.Text = $"🟢 Thắng: {profile.Wins}";
            if (lblBadgeDraws != null) lblBadgeDraws.Text = $"🟡 Hòa: {profile.Draws}";
            if (lblBadgeLosses != null) lblBadgeLosses.Text = $"🔴 Thua: {profile.Losses}";
            if (lblBadgeWinRate != null) lblBadgeWinRate.Text = $"📊 Tỉ lệ: {profile.WinRate}%";
        }

        private void LobbyForm_Load(object? sender, EventArgs e)
        {
            dgvRooms.Rows.Clear();
            UpdateConnectionStatus(true);
            RequestProfile();

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

            if (playerListControl1 != null)
            {
                playerListControl1.OnChallengePlayer += PlayerListControl1_OnChallengePlayer;
            }
        }

        private void BtnHistory_Click(object? sender, EventArgs e)
        {
            var req = new HistoryRequestMessage
            {
                Username = _playerName
            };
            _ = _clientConnection.SendMessageAsync(req);
        }

        private void PlayerListControl1_OnChallengePlayer(string targetPlayer)
        {
            if (targetPlayer == _playerName)
            {
                MessageBox.Show("Bạn không thể tự thách đấu chính mình!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Bạn muốn gửi lời thách đấu tới '{targetPlayer}'?",
                "Thách đấu",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                string roomName = $"Thách đấu: {_playerName} vs {targetPlayer}";
                var requestMsg = new CaroGame.Protocol.Messages.Room.CreateRoomMessage
                {
                    SenderId = _playerName,
                    RoomName = roomName,
                    HostId = _playerName,
                    MaxPlayers = 2,
                    BoardSize = 20,
                    IsPrivate = false,
                    Password = ""
                };

                _ = Task.Run(async () =>
                {
                    try { await _clientConnection.SendMessageAsync(requestMsg); }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                });

                RoomForm roomForm = new RoomForm(_clientConnection, roomName, _playerName, true, targetPlayer);
                roomForm.FormClosed += (s, args) => { this.Show(); RequestProfile(); };
                roomForm.Show();
                this.Hide();
            }
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
                        if (response.Action == "ProfileResponse")
                        {
                            var profile = System.Text.Json.JsonSerializer.Deserialize<CaroGame.Protocol.Messages.UserProfileDto>(response.Data);
                            if (profile != null)
                            {
                                UpdateProfileUI(profile);
                            }
                        }
                        else if (response.Data.Contains("OnlineCount"))
                        {
                            var lobbyState = System.Text.Json.JsonSerializer.Deserialize<LobbyStateDto>(response.Data);
                            if (lobbyState != null)
                            {
                                UpdateOnlineCount(lobbyState.OnlineCount);
                                UpdateRoomList(lobbyState.Rooms);

                                // 3. GỌI CONTROL ĐỂ HIỂN THỊ DANH SÁCH NGƯỜI CHƠI LÊN MÀN HÌNH
                                if (playerListControl1 != null)
                                {
                                    playerListControl1.UpdateList(lobbyState.OnlinePlayers);
                                }
                            }
                        }
                    }
                }
                else if (message.Type == MessageType.HistoryResponse && message is HistoryResponseMessage historyRes)
                {
                    var historyForm = new HistoryForm(historyRes.Matches, _playerName);
                    historyForm.ShowDialog();
                }
                else if (message.Type == MessageType.Invite && message is InviteMessage inviteMsg)
                {
                    DialogResult result = MessageBox.Show(
                        $"Người chơi '{inviteMsg.SenderId}' muốn thách đấu với bạn.\nBạn có đồng ý tham gia không?",
                        "Lời mời Thách Đấu",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes)
                    {
                        var joinMsg = new CaroGame.Protocol.Messages.Room.JoinRoomMessage
                        {
                            SenderId = _playerName,
                            RoomId = inviteMsg.RoomId,
                            PlayerId = _playerName,
                            PlayerName = _playerName,
                            Password = "",
                            IsSpectator = false
                        };
                        _ = _clientConnection.SendMessageAsync(joinMsg);

                        RoomForm roomForm = new RoomForm(_clientConnection, "Phòng thách đấu", _playerName, false);
                        roomForm.FormClosed += (s, args) => { this.Show(); RequestProfile(); };
                        roomForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        // (Tùy chọn) Gửi tin nhắn từ chối lại cho Sender nếu muốn
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
            var requestMsg = new CaroGame.Protocol.Messages.Room.CreateRoomMessage
            {
                SenderId = _playerName,
                RoomName = roomName,
                HostId = _playerName,
                MaxPlayers = 2,
                BoardSize = 20,
                IsPrivate = false,
                Password = ""
            };

            _ = Task.Run(async () =>
            {
                try { await _clientConnection.SendMessageAsync(requestMsg); }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            });

            // Mở màn hình Game
            RoomForm roomForm = new RoomForm(_clientConnection, roomName, _playerName, true); // true = Chủ phòng
            roomForm.FormClosed += (s, args) => { this.Show(); RequestProfile(); };
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
            // 1. ĐỌC TRẠNG THÁI PHÒNG TỪ CỘT SỐ 3 (Cells[3])
            string roomStatus = dgvRooms.SelectedRows[0].Cells[3].Value?.ToString() ?? "";

            // 2. LẬP CHỐT CHẶN: NẾU PHÒNG ĐÃ KÍN CHỖ THÌ TỪ CHỐI
            bool isSpectator = false;
            if (roomStatus.Contains("Đang chơi") || roomStatus.Contains("Đã đầy"))
            {
                var result = MessageBox.Show("Phòng này đã đủ người hoặc đang thi đấu. Bạn có muốn vào với vai trò khán giả không?", "Khán giả", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    isSpectator = true;
                }
                else
                {
                    return; // Lệnh return này sẽ dừng ngay lập tức, không cho phép gửi tin lên Server và KHÔNG mở GameForm!
                }
            }
            // 2. Trích xuất "Mã Phòng" từ cột đầu tiên (Cells[0]) của dòng đang chọn
            string selectedRoomId = dgvRooms.SelectedRows[0].Cells[0].Value?.ToString() ?? "";
            string selectedRoomName = dgvRooms.SelectedRows[0].Cells[1].Value?.ToString() ?? "Phòng ẩn";

            // 3. Đóng gói lệnh xin gia nhập và gửi lên Server
            var requestMsg = new CaroGame.Protocol.Messages.Room.JoinRoomMessage
            {
                SenderId = _playerName,
                RoomId = selectedRoomId,
                PlayerId = _playerName,
                PlayerName = _playerName,
                Password = "",
                IsSpectator = isSpectator
            };

            _ = Task.Run(async () =>
            {
                try { await _clientConnection.SendMessageAsync(requestMsg); }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            });

            // 4. Chuyển sang màn hình thi đấu
            RoomForm roomForm = new RoomForm(_clientConnection, selectedRoomName, _playerName, false, null, isSpectator); // false = Khách
            roomForm.FormClosed += (s, args) => { this.Show(); RequestProfile(); };
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