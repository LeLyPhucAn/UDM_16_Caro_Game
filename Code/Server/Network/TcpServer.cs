using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using CaroGame.Protocol;
using CaroGame.Protocol.Messages;
using CaroGame.Protocol.Messages.Game;
using Shared.Models;
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
    private readonly ConnectionStateManager _connectionStateManager;
    private readonly NetworkDiagnostics _networkDiagnostics;

    public TcpServer()
    {
        _messageHandler = new MessageHandler(_userService, _roomManager, _matchManager, _connectionManager);
        
        // Khởi tạo các Manager cho Diagnostics và Heartbeat
        _connectionStateManager = new ConnectionStateManager(_connectionManager);
        _networkDiagnostics = new NetworkDiagnostics();

        // Đăng ký sự kiện xử thua do hết giờ từ MatchManager
        _matchManager.OnMatchTimeout += HandleMatchTimeout;
        
        // Lắng nghe sự kiện ngắt kết nối chủ động từ Heartbeat
        NetworkEvents.OnClientDisconnected += OnClientDisconnected;
    }

    private void HandleMatchTimeout(Match match, string winnerId, string winnerName)
    {
        var timeoutMsg = new GameResultMessage
        {
            RoomId = match.MatchId,
            ResultType = "Timeout",
            WinnerId = winnerId,
            WinnerName = winnerName,
            WinningLine = new string[0]
        };

        // Gửi thông báo xử thua cho cả 2 người chơi
        if (match.PlayerX != null)
        {
            _ = _connectionManager.SendMessageToClientAsync(match.PlayerX.Id, timeoutMsg);
        }
        if (match.PlayerO != null)
        {
            _ = _connectionManager.SendMessageToClientAsync(match.PlayerO.Id, timeoutMsg);
        }
        
        Logger.Info($"[Timeout] Trận đấu {match.MatchId} kết thúc do một người quá giờ. Người thắng: {winnerId}");
    }

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
            
            // Chuyển gói tin sang Router (MessageHandler) để xử lý logic
            await _messageHandler.ProcessMessageAsync(session, message);
        }

        private void OnClientDisconnected(ClientSession session)
        {
            string sessionId = session.SessionId.ToString();
            
            // Tìm xem người chơi này có đang trong phòng nào không
            var room = _roomManager.FindPlayerRoom(sessionId);
            if (room != null)
            {
                Logger.Info($"[Disconnect] Player {sessionId} ngắt kết nối đột ngột khi đang trong phòng {room.RoomId}.");
                
                // Nếu trận đấu đang diễn ra, hủy trận
                var match = _matchManager.GetMatch(room.RoomId);
                if (match != null && match.State == MatchState.Playing)
                {
                    _matchManager.CancelMatch(match.MatchId, "Opponent disconnected.");
                    
                    // Thông báo Game Result (Hủy) cho người còn lại
                    var cancelMsg = new GameResultMessage
                    {
                        RoomId = match.MatchId,
                        ResultType = "Cancel",
                        WinnerId = string.Empty,
                        WinningLine = new string[0]
                    };
                    
                    // Gửi tới toàn bộ client trong Room (thực ra chỉ còn lại người kia vì người này disconnect rồi)
                    _ = _connectionManager.BroadcastExceptAsync(session.SessionId, cancelMsg);
                }

                // Xóa người chơi khỏi phòng
                _roomManager.LeaveRoom(room.RoomId, sessionId);
            }

            _connectionManager.Remove(session.SessionId);
            
            // Broadcast LobbyState mới
            _ = _connectionManager.BroadcastLobbyStateAsync();
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