using System.Collections.Generic;

namespace Shared.Models
{
    public class RoomInfo
    {
        public string RoomId { get; set; }
        public string RoomName { get; set; }
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public bool IsPlaying { get; set; }
    }

    public class LobbyStateDto
    {
        public int OnlineCount { get; set; }
        public List<string> OnlinePlayers { get; set; }
        public List<RoomInfo> Rooms { get; set; }

        public LobbyStateDto()
        {
            OnlinePlayers = new List<string>();
            Rooms = new List<RoomInfo>();
        }
    }
}
