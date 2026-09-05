using System.Collections.Generic;

namespace CaroGame.Protocol.Messages.History
{
    public class HistoryResponseMessage : BaseMessage
    {
        public List<MatchHistoryItem> Matches { get; set; }

        public HistoryResponseMessage()
        {
            Type = MessageType.HistoryResponse;
            Matches = new List<MatchHistoryItem>();
        }
    }
}
