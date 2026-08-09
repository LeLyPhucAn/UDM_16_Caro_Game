using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace Server.Network;

public class ClientSession
{
    public Guid SessionId { get; }

    public TcpClient Client { get; }

    public NetworkStream Stream { get; }

    public IPEndPoint? RemoteEndPoint { get; }

    public DateTime ConnectedTime { get; }

    public bool IsConnected => Client.Connected;

    public ClientSession(TcpClient client)
    {
        SessionId = Guid.NewGuid();

        Client = client;

        Stream = client.GetStream();

        RemoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;

        ConnectedTime = DateTime.Now;
    }
}