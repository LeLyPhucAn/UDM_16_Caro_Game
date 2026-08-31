using System;
using System.Collections.Generic;
using System.Linq; // [ĐÃ THÊM] Bắt buộc phải có thư viện này để dùng .Select()
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using CaroGame.Protocol;
using CaroGame.Protocol.Messages;
using Server.Config;
using Server.Managers;
using Server.Utils;

namespace Server.Network
{
    public class TcpServer
    {
        private TcpListener? _listener;
        private bool _isRunning;
        private CancellationTokenSource? _cts;

        private readonly ConnectionManager _connectionManager = new();

        // Khai báo MatchManager để xử lý logic Thắng/Thua/Luật chơi
        private readonly MatchManager _matchManager = new();
        private readonly RoomManager _roomManager = new();

        public RoomManager RoomManager => _roomManager;
        public ConnectionManager ConnectionManager => _connectionManager;
        public MatchManager MatchManager => _matchManager;

        public void Start(ServerConfig config)
        {
            IPAddress ip = IPAddress.Parse(config.Ip);

            _listener = new TcpListener(ip, config.Port);
            _listener.Start();

            _isRunning = true;
            _cts = new CancellationTokenSource();

            Logger.Info("========================================");
            Logger.Info($"Server đã khởi động thành công!");
            Logger.Info($"Địa chỉ IP : {config.Ip}");
            Logger.Info($"Cổng Port  : {config.Port}");
            Logger.Info("========================================");

            _ = AcceptClientsAsync(_cts.Token);
        }

        private async Task AcceptClientsAsync(CancellationToken cancellationToken)
        {
            while (_isRunning && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = await _listener!.AcceptTcpClientAsync(cancellationToken);

                    ClientSession session = new(client);
                    _connectionManager.Add(session);

                    // Lắng nghe tin nhắn ngầm từ Client
                    _ = Task.Run(() => NetworkHandler.ListenForMessagesAsync(
                        session,
                        OnMessageReceivedAsync,
                        OnClientDisconnected, // Đã khớp với hàm bên dưới
                        cancellationToken
                    ), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (SocketException)
                {
                    if (!_isRunning) break;
                }
                catch (Exception ex)
                {
                    Logger.Error("Lỗi khi chấp nhận Client mới", ex);
                }
            }
        }

        private async Task OnMessageReceivedAsync(ClientSession session, BaseMessage message)
        {
            Logger.Debug($"Nhận Message từ {session.SessionId}: Type={message.Type}, Sender={message.SenderId}");

            // =======================================================
            // XỬ LÝ LỆNH TỪ CLIENT KHI BẤM "TẠO PHÒNG" HOẶC "LOGIN"
            // =======================================================
            if (message.Type == MessageType.Login)
            {
                await BroadcastLobbyStateAsync();
                return; // Có thể return hoặc để code chạy tiếp xuống dưới
            }
            if (message.Type == MessageType.Request && message is RequestMessage reqMsg)
            {
                // Trích xuất thông tin người gửi
                string playerId = session.SessionId.ToString();
                string playerName = reqMsg.SenderId;

                if (reqMsg.Action == "CreateRoom")
                {
                    // 1. Tạo phòng mới
                    var newRoom = _roomManager.CreateRoom(reqMsg.Data);

                    // 2. Ép người tạo phòng phải bước vào chính phòng đó
                    Player creator = new Player(playerId, playerName);
                    _roomManager.JoinRoom(newRoom.RoomId, creator);

                    // 3. Cập nhật sảnh (lúc này phòng sẽ hiện 1/2)
                    await BroadcastLobbyStateAsync();
                }
                else if (reqMsg.Action == "JoinRoom")
                {
                    string targetRoomId = reqMsg.Data; // Client gửi lên Mã Phòng

                    // 1. Tạo đối tượng Player cho người xin vào
                    Player joiningPlayer = new Player(playerId, playerName);

                    // 2. Thử cho người chơi vào phòng
                    bool success = _roomManager.JoinRoom(targetRoomId, joiningPlayer);

                    if (success)
                    {
                        // 3. Nếu vào thành công, kiểm tra xem phòng đã đủ 2 người chưa
                        if (_roomManager.CanStartGame(targetRoomId))
                        {
                            // Khóa phòng lại thành trạng thái "Đang chơi"
                            _roomManager.SetPlaying(targetRoomId, true);

                            // (Phần tạo trận đấu của MatchManager sẽ được móc nối ở đây sau)
                        }

                        // 4. Phát sóng cập nhật Sảnh (lúc này phòng sẽ hiện 2/2 và đổi màu Đang chơi)
                        await BroadcastLobbyStateAsync();
                    }
                    else
                    {
                        Logger.Warn($"[ROOM] {playerName} không thể tham gia phòng {targetRoomId} (Phòng đầy hoặc lỗi).");
                    }
                }
                else if (reqMsg.Action == "LeaveRoom")
                {
                    // 1. Tìm xem người chơi này đang ngồi ở phòng nào
                    var room = _roomManager.FindPlayerRoom(playerId);

                    if (room != null)
                    {
                        // 2. Trục xuất người chơi khỏi phòng
                        // (Hàm LeaveRoom của bạn đã tự động xóa phòng nếu hết người)
                        _roomManager.LeaveRoom(room.RoomId, playerId);

                        // 3. Nếu phòng chưa bị xóa (vẫn còn 1 người), trả lại trạng thái Đang chờ
                        if (_roomManager.RoomExists(room.RoomId))
                        {
                            _roomManager.SetPlaying(room.RoomId, false);
                        }

                        // 4. Phát sóng để làm mới bảng của tất cả mọi người
                        await BroadcastLobbyStateAsync();
                    }
                }
                else if (reqMsg.Action == "RefreshLobby")
                {
                    await BroadcastLobbyStateAsync();
                }

                return; // Xử lý xong Request thì thoát nhánh này
            }

            // =======================================================
            // XỬ LÝ NƯỚC ĐI (MOVE) TRONG GAME
            // =======================================================
            if (message.Type == MessageType.Move && message is MoveMessage moveMsg)
            {
                string playerId = session.SessionId.ToString();
                var currentMatch = _matchManager.FindPlayerMatch(playerId);

                if (currentMatch == null)
                {
                    // Giả lập tạo trận đấu cho 1 người test (Đối thủ ảo)
                    var testPlayerX = new Player(Guid.NewGuid().ToString(), "Nam_Test");
                    var testPlayerO = new Player(Guid.NewGuid().ToString(), "Minh_Bot");

                    currentMatch = _matchManager.CreateMatch("TEST_ROOM", testPlayerX, testPlayerO);
                    currentMatch?.Start();

                    var syncMsg = new GameSyncMessage
                    {
                        PlayerXName = testPlayerX.PlayerName,
                        PlayerOName = testPlayerO.PlayerName,
                        MySymbol = "X",
                        CurrentTurnName = testPlayerX.PlayerName
                    };

                    await session.SendAsync(syncMsg);
                }

                if (currentMatch != null)
                {
                    bool isMoveValid = _matchManager.MakeMove(currentMatch.MatchId, playerId, moveMsg.Row, moveMsg.Col);

                    if (isMoveValid)
                    {
                        Guid sessionX = Guid.Parse(currentMatch.PlayerX.PlayerId);
                        Guid sessionO = Guid.Parse(currentMatch.PlayerO.PlayerId);

                        ClientSession? clientX = _connectionManager.Get(sessionX);
                        ClientSession? clientO = _connectionManager.Get(sessionO);

                        if (clientX != null) await clientX.SendAsync(moveMsg);
                        if (clientO != null && sessionX != sessionO) await clientO.SendAsync(moveMsg);

                        if (currentMatch.IsFinished())
                        {
                            var gameOverMsg = new GameOverMessage();

                            if (currentMatch.IsDraw())
                            {
                                gameOverMsg.ResultType = "Draw";
                            }
                            else
                            {
                                gameOverMsg.ResultType = "Win";
                                var winner = currentMatch.WinnerId == currentMatch.PlayerX.PlayerId
                                             ? currentMatch.PlayerX
                                             : currentMatch.PlayerO;
                                gameOverMsg.WinnerName = winner.PlayerName;
                            }

                            if (clientX != null) await clientX.SendAsync(gameOverMsg);
                            if (clientO != null && sessionX != sessionO) await clientO.SendAsync(gameOverMsg);
                        }
                    }
                    else
                    {
                        Logger.Warn($"[Game] Client {playerId} gửi nước đi không hợp lệ!");
                    }
                }
                return;
            }

            // =======================================================
            // TRẢ LỜI MẶC ĐỊNH NẾU KHÔNG PHẢI CÁC LOẠI TRÊN
            // =======================================================
            ResponseMessage response = new ResponseMessage
            {
                SenderId = "Server",
                Success = true,
                ErrorMessage = string.Empty,
                Data = $"Server đã nhận {message.Type} thành công lúc {DateTime.Now:HH:mm:ss}"
            };

            await session.SendAsync(response);
        }

        // [ĐÃ SỬA] Đưa hàm này vào đúng vị trí bên trong class và thêm async
        private async void OnClientDisconnected(ClientSession session)
        {
            _connectionManager.Remove(session.SessionId);

            // 👉 BỔ SUNG: Dọn dẹp phòng nếu người này đang trong Game mà bị rớt mạng
            string disconnectedPlayerId = session.SessionId.ToString();
            var currentRoom = _roomManager.FindPlayerRoom(disconnectedPlayerId);

            if (currentRoom != null)
            {
                // Trục xuất người chơi khỏi phòng
                // (Hàm LeaveRoom của bạn đã có sẵn logic: tự xóa phòng nếu số người = 0)
                _roomManager.LeaveRoom(currentRoom.RoomId, disconnectedPlayerId);

                // Nếu phòng vẫn chưa bị xóa (tức là còn người chơi kia ở lại), 
                // thì chuyển phòng đó về trạng thái "Đang chờ" (IsPlaying = false)
                if (_roomManager.RoomExists(currentRoom.RoomId))
                {
                    _roomManager.SetPlaying(currentRoom.RoomId, false);
                }
            }
            // Tự động phát dữ liệu số lượng online mới nhất (và danh sách phòng vừa được dọn dẹp) cho các Client khác
            await BroadcastLobbyStateAsync();
        }

        public void Stop()
        {
            _isRunning = false;
            _cts?.Cancel();

            _connectionManager.ClearAll();
            _listener?.Stop();

            Logger.Warn("Server đã dừng hoạt động.");
        }

        // Hàm phát dữ liệu cho tất cả Client
        private async Task BroadcastLobbyStateAsync()
        {
            var lobbyState = new LobbyStateDto
            {
                OnlineCount = _connectionManager.Count,
                Rooms = _roomManager.GetRooms().Select(r => new RoomInfo
                {
                    RoomId = r.RoomId,
                    RoomName = r.RoomName,
                    CurrentPlayers = r.Players.Count,
                    MaxPlayers = r.MaxPlayers,
                    IsPlaying = r.IsPlaying
                }).ToList()
            };

            var response = new ResponseMessage
            {
                SenderId = "Server",
                Success = true,
                Data = System.Text.Json.JsonSerializer.Serialize(lobbyState)
            };

            await _connectionManager.BroadcastAsync(response);
        }
    }
}