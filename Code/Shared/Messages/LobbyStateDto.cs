using System.Collections.Generic;

namespace CaroGame.Protocol
{
    public class RoomInfo
    {
        public string RoomId { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; } = 2;
        public bool IsPlaying { get; set; }
    }

    public class LobbyStateDto
    {
        public int OnlineCount { get; set; }
        public List<string> OnlinePlayers { get; set; } = new List<string>();
        public List<RoomInfo> Rooms { get; set; } = new List<RoomInfo>();
    }
}