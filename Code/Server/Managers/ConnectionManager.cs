using System;
using System.Collections.Generic;
using System.Text;

using Server.Network;

namespace Server.Managers;

public class ConnectionManager
{
    private readonly List<ClientSession> _clients = new();

    public void Add(ClientSession session)
    {
        _clients.Add(session);

        Console.WriteLine($"[+] Client connected ({_clients.Count})");
    }

    public void Remove(ClientSession session)
    {
        _clients.Remove(session);

        Console.WriteLine($"[-] Client disconnected ({_clients.Count})");
    }

    public IReadOnlyList<ClientSession> GetAll()
    {
        return _clients.AsReadOnly();
    }

    public int Count => _clients.Count;
}