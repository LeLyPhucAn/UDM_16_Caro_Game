using System;

namespace CaroGame.Protocol.Messages.History
{
    public class MatchHistoryItem
    {
        public int MatchId { get; set; }
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? WinnerId { get; set; }
        public string Result { get; set; }
        public string Status { get; set; }
    }
}
