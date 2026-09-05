using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using CaroGame.Protocol;
using CaroGame.Protocol.Messages;
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
        _messageHandler = new MessageHandler(_userService, _roomManager, _matchManager, _connectionManager);
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
            _connectionManager.Remove(session.SessionId);
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