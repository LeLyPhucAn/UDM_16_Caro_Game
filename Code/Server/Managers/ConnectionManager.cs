using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaroGame.Protocol;
using Server.Network;
using Server.Utils;

namespace Server.Managers;

public class ConnectionManager
{
    // Sử dụng ConcurrentDictionary để đảm bảo an toàn đa luồng (Thread-Safety)
    private readonly ConcurrentDictionary<Guid, ClientSession> _clients = new();

    /// <summary>
    /// Số lượng Client đang trực tuyến
    /// </summary>
    public int Count => _clients.Count;

    /// <summary>
    /// Thêm một Client Session mới vào danh sách quản lý
    /// </summary>
    public bool Add(ClientSession session)
    {
        if (_clients.TryAdd(session.SessionId, session))
        {
            Logger.Info($"[+] Client kết nối: SessionId={session.SessionId}, IP={session.RemoteEndPoint} (Online: {_clients.Count})");
            return true;
        }

        Logger.Warn($"Không thể thêm Client {session.SessionId} (Session đã tồn tại)");
        return false;
    }

    /// <summary>
    /// Xóa và đóng kết nối của một Client Session
    /// </summary>
    public bool Remove(Guid sessionId)
    {
        if (_clients.TryRemove(sessionId, out var session))
        {
            session.Close();
            Logger.Info($"[-] Client ngắt kết nối: SessionId={sessionId}, IP={session.RemoteEndPoint} (Online: {_clients.Count})");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Tìm kiếm ClientSession theo SessionId
    /// </summary>
    public ClientSession? Get(Guid sessionId)
    {
        _clients.TryGetValue(sessionId, out var session);
        return session;
    }

    /// <summary>
    /// Lấy danh sách toàn bộ ClientSession đang hoạt động
    /// </summary>
    public IReadOnlyList<ClientSession> GetAll()
    {
        return _clients.Values.ToList().AsReadOnly();
    }

    /// <summary>
    /// Gửi thông điệp (Broadcast) tới toàn bộ Client đang kết nối
    /// </summary>
    public async Task BroadcastAsync(BaseMessage message)
    {
        var tasks = _clients.Values.Select(client => client.SendAsync(message));
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Gửi thông điệp tới tất cả Client ngoại trừ một Client chỉ định (hữu ích cho Chat/Game Room)
    /// </summary>
    public async Task BroadcastExceptAsync(Guid excludeSessionId, BaseMessage message)
    {
        var tasks = _clients.Values
            .Where(client => client.SessionId != excludeSessionId)
            .Select(client => client.SendAsync(message));

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Ngắt và dọn dẹp toàn bộ kết nối khi dừng Server
    /// </summary>
    public void ClearAll()
    {
        foreach (var session in _clients.Values)
        {
            session.Close();
        }
        _clients.Clear();
        Logger.Info("Đã dọn dẹp và ngắt toàn bộ kết nối Client.");
    }

    /// <summary>
    /// Đóng gói trạng thái Sảnh và gửi cho toàn bộ Client
    /// </summary>
    public async Task BroadcastLobbyStateAsync()
    {
        // Lấy danh sách tên người chơi đang online (Giả sử ClientSession có property Username)
        var players = _clients.Values
            .Select(c => c.SessionId.ToString()) // Tạm lấy SessionId làm tên nếu chưa lưu Username
            .ToList();

        var lobbyData = new LobbyStateDto
        {
            OnlineCount = _clients.Count,
            OnlinePlayers = players,
            Rooms = new List<RoomInfo>() // Sau này quản lý phòng thì điền vào đây
        };

        var response = new ResponseMessage
        {
            Success = true,
            Data = System.Text.Json.JsonSerializer.Serialize(lobbyData)
        };

        await BroadcastAsync(response);
    }

    public List<string> GetAllPlayerNames()
    {
        List<string> playerNames = new List<string>();

        foreach (var session in _clients.Values) // Thay _clients bằng tên biến Dictionary thực tế của bạn
        {
            if (!string.IsNullOrWhiteSpace(session.PlayerName))
            {
                playerNames.Add(session.PlayerName);
            }
            else
            {
                // Dự phòng khi vừa kết nối mà chưa kịp gửi tên
                playerNames.Add("Khách_" + session.SessionId.ToString().Substring(0, 4));
            }
        }

        return playerNames;
    }
}
