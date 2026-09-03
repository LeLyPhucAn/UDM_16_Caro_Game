using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using CaroGame.Protocol;
using CaroGame.Protocol.Messages;

namespace Server.Network;

public class ClientSession
{
    public Guid SessionId { get; }

    public TcpClient Client { get; }

    public string PlayerName { get; set; } = string.Empty;

    public NetworkStream Stream { get; }

    public IPEndPoint? RemoteEndPoint { get; }

    public DateTime ConnectedTime { get; }

    public DateTime LastPingTime { get; set; }

    public DateTime LastPongTime { get; set; }

    public bool IsConnected => Client.Connected;

    public ClientSession(TcpClient client)
    {
        SessionId = Guid.NewGuid();

        Client = client;

        Stream = client.GetStream();

        RemoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;

        ConnectedTime = DateTime.Now;
        LastPingTime = DateTime.Now;
        LastPongTime = DateTime.Now;
    }

    public async Task<bool> SendAsync(BaseMessage message)
    {
        return await NetworkHandler.SendAsync(this, message);
    }

    public void Close()
    {
        try
        {
            Stream?.Close();
            Client?.Close();
        }
        catch { }
    }
}
