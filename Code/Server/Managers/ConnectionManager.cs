using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Server.Network;

namespace Server.Managers;

public class ConnectionManager
{
    // Dùng ConcurrentDictionary thay cho List để chống crash đụng độ luồng (Thread-Safety)
    private readonly ConcurrentDictionary<Guid, ClientSession> _clients = new();

    public void Add(ClientSession session)
    {
        if (_clients.TryAdd(session.SessionId, session))
        {
            Console.WriteLine($"[+] Client connected ({_clients.Count})");
        }
    }

    public void Remove(Guid sessionId)
    {
        if (_clients.TryRemove(sessionId, out var session))
        {
            session.Close();
            Console.WriteLine($"[-] Client disconnected ({_clients.Count})");
        }
    }

    public IReadOnlyList<ClientSession> GetAll()
    {
        return _clients.Values.ToList().AsReadOnly();
    }

    public void ClearAll()
    {
        foreach (var session in _clients.Values)
        {
            session.Close();
        }
        _clients.Clear();
    }

    public int Count => _clients.Count;
}
