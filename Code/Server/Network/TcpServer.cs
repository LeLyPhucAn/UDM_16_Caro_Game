using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using CaroGame.Protocol;
using Server.Config;
using Server.Managers;

namespace Server.Network;

public class TcpServer
{
    private TcpListener? _listener;
    private bool _isRunning;
    private CancellationTokenSource? _cts;

    private readonly ConnectionManager _connectionManager = new();

    public ConnectionManager ConnectionManager => _connectionManager;

    public void Start(ServerConfig config)
    {
        IPAddress ip = IPAddress.Parse(config.Ip);

        _listener = new TcpListener(ip, config.Port);
        _listener.Start();

        _isRunning = true;
        _cts = new CancellationTokenSource();

        Console.WriteLine("------------------------------------");
        Console.WriteLine("Server started");
        Console.WriteLine($"IP   : {config.Ip}");
        Console.WriteLine($"Port : {config.Port}");
        Console.WriteLine("------------------------------------");

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

                Console.WriteLine();
                Console.WriteLine("====================================");
                Console.WriteLine("New Client Connected");
                Console.WriteLine($"Session ID : {session.SessionId}");
                Console.WriteLine($"Remote IP  : {session.RemoteEndPoint}");
                Console.WriteLine($"Connected  : {session.ConnectedTime}");
                Console.WriteLine($"Online     : {_connectionManager.Count}");
                Console.WriteLine("====================================");
                Console.WriteLine();

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
                if (!_isRunning)
                    break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ServerError] Error accepting client: {ex.Message}");
            }
        }
    }

    private async Task OnMessageReceivedAsync(ClientSession session, BaseMessage message)
    {
        Console.WriteLine($"[Recv] Từ Client {session.SessionId} ({session.RemoteEndPoint}): Type={message.Type}, Sender={message.SenderId}");

        // Phản hồi mẫu ResponseMessage lại cho Client
        ResponseMessage response = new ResponseMessage
        {
            SenderId = "Server",
            Success = true,
            ErrorMessage = string.Empty,
            Data = $"Server received {message.Type} message successfully"
        };

        await session.SendAsync(response);
    }

    private void OnClientDisconnected(ClientSession session)
    {
        _connectionManager.Remove(session.SessionId);
        Console.WriteLine($"[Disconnected] Client {session.SessionId} ({session.RemoteEndPoint}) đã ngắt kết nối. Online còn: {_connectionManager.Count}");
    }

    public void Stop()
    {
        _isRunning = false;
        _cts?.Cancel();

        _connectionManager.ClearAll();
        _listener?.Stop();

        Console.WriteLine();
        Console.WriteLine("Server stopped.");
    }
}
