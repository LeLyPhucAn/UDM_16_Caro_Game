using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using CaroGame.Protocol;
using Server.Config;
using Server.Managers;
using Server.Services;
using Server.Utils;

namespace Server.Network
{
    public class TcpServer
    {
        private TcpListener? _listener;
        private bool _isRunning;
        private CancellationTokenSource? _cts;


    private readonly ConnectionManager _connectionManager = new();
    private readonly UserService _userService = new();
    private readonly RoomManager _roomManager = new();
    private readonly MatchManager _matchManager = new();
    private readonly MessageHandler _messageHandler;

    public TcpServer()
    {
        _messageHandler = new MessageHandler(_userService, _roomManager, _matchManager);
    }

        // [THÊM MỚI] Khai báo MatchManager để xử lý logic Thắng/Thua/Luật chơi
        private readonly MatchManager _matchManager = new();

        public ConnectionManager ConnectionManager => _connectionManager;

        // [THÊM MỚI] Getter cho MatchManager (sau này dùng cho RoomManager móc nối qua)
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
                        OnClientDisconnected,
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

            
            if (message.Type == MessageType.Move && message is MoveMessage moveMsg)
            {
                string playerId = session.SessionId.ToString();

                // 1. Tìm trận đấu mà Client này đang ngồi
                var currentMatch = _matchManager.FindPlayerMatch(playerId);

                // TEST: NẾU CHƯA CÓ TRẬN ĐẤU 
                // TEST: NẾU CHƯA CÓ TRẬN ĐẤU 
                if (currentMatch == null)
                {
                    // 1. Phát tin đồng bộ tên GIẢ LẬP để test BƯỚC 3
                    var syncMsg = new GameSyncMessage
                    {
                        PlayerXName = "Nam123",
                        PlayerOName = "Minh456",
                        MySymbol = "X",             // Giả lập Client test này đang cầm cờ X
                        CurrentTurnName = "Nam123"  // Lượt đầu tiên là của X
                    };
                    await session.SendAsync(syncMsg);

                    // 2. Trả thẳng nước đi về cho Client tự vẽ X
                    await session.SendAsync(moveMsg);
                    return;
                }

                if (currentMatch != null)
                {
                    // 2. Thẩm định nước đi (Hàm MakeMove tự động kiểm tra lượt, vị trí, ô trống)
                    bool isMoveValid = _matchManager.MakeMove(currentMatch.MatchId, playerId, moveMsg.Row, moveMsg.Col);

                    if (isMoveValid)
                    {
                        // 3. Nếu hợp lệ -> Lấy Session của 2 người chơi để thông báo
                        Guid sessionX = Guid.Parse(currentMatch.PlayerX.PlayerId);
                        Guid sessionO = Guid.Parse(currentMatch.PlayerO.PlayerId);

                        ClientSession? clientX = _connectionManager.Get(sessionX);
                        ClientSession? clientO = _connectionManager.Get(sessionO);

                        // Gửi thẳng đối tượng MoveMessage (mạng sẽ tự Pack lại thành JSON/Bytes)
                        if (clientX != null) await clientX.SendAsync(moveMsg);

                        // Không gửi 2 lần nếu test 1 mình (sessionX == sessionO)
                        if (clientO != null && sessionX != sessionO) await clientO.SendAsync(moveMsg);

                        // 4. Kiểm tra xem nước đi vừa rồi có làm trận đấu kết thúc không
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
                                // Xác định ai là người chiến thắng
                                var winner = currentMatch.WinnerId == currentMatch.PlayerX.PlayerId
                                             ? currentMatch.PlayerX
                                             : currentMatch.PlayerO;
                                gameOverMsg.WinnerName = winner.PlayerName;
                            }

                            // Gửi thông báo kết thúc cho cả phòng
                            if (clientX != null) await clientX.SendAsync(gameOverMsg);
                            if (clientO != null && sessionX != sessionO) await clientO.SendAsync(gameOverMsg);
                        }
                    }
                    else
                    {
                        Logger.Warn($"[Game] Client {playerId} gửi nước đi không hợp lệ!");
                    }
                }

                // Xử lý xong MoveMessage thì thoát hàm, không chạy xuống phần phản hồi mẫu nữa
                return;
            }

           
            ResponseMessage response = new ResponseMessage
            {
                SenderId = "Server",
                Success = true,
                ErrorMessage = string.Empty,
                Data = $"Server đã nhận {message.Type} thành công lúc {DateTime.Now:HH:mm:ss}"
            };

            await session.SendAsync(response);
        }

        private void OnClientDisconnected(ClientSession session)
        {
            _connectionManager.Remove(session.SessionId);
        }
        // Chuyển gói tin sang Router (MessageHandler) để xử lý logic
        await _messageHandler.ProcessMessageAsync(session, message);
    }



        public void Stop()
        {
            _isRunning = false;
            _cts?.Cancel();

            _connectionManager.ClearAll();
            _listener?.Stop();

            Logger.Warn("Server đã dừng hoạt động.");
        }
    }
}