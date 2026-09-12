using System.Collections.Generic;
using Shared.Models;

namespace GameLogic.Models
{
    public class Room
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int HostPlayerId { get; set; }

        public List<Player> Players { get; set; }

        public string Status { get; set; }

        public List<string> SpectatorSessionIds { get; set; } = new List<string>();
        public Room()
        {
            Name = string.Empty;
            Status = "Waiting";
            Players = new List<Player>();
        }

        public Room(
            int id,
            string name,
            int hostPlayerId)
        {
            Id = id;
            Name = name;
            HostPlayerId = hostPlayerId;

            Status = "Waiting";

            Players = new List<Player>();
        }
    }
}