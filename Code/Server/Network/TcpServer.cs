using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;
using Server.Config;
using Server.Managers;

namespace Server.Network;

public class TcpServer
{
    private TcpListener? _listener;

    private bool _isRunning;

    private readonly ConnectionManager _connectionManager = new();

    public void Start(ServerConfig config)
    {
        IPAddress ip = IPAddress.Parse(config.Ip);

        _listener = new TcpListener(ip, config.Port);

        _listener.Start();

        _isRunning = true;

        Console.WriteLine("------------------------------------");
        Console.WriteLine("Server started");
        Console.WriteLine($"IP   : {config.Ip}");
        Console.WriteLine($"Port : {config.Port}");
        Console.WriteLine("------------------------------------");

        ListenAsync();
    }

    private async void ListenAsync()
    {
        while (_isRunning)
        {
            try
            {
                TcpClient client = await _listener!.AcceptTcpClientAsync();

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
            }
            catch (SocketException)
            {
                if (!_isRunning)
                    break;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public void Stop()
    {
        _isRunning = false;

        _listener?.Stop();

        Console.WriteLine();
        Console.WriteLine("Server stopped.");
    }
}