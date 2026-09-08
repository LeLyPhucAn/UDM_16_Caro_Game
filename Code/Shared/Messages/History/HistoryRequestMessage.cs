namespace CaroGame.Protocol.Messages.History
{
    public class HistoryRequestMessage : BaseMessage
    {
        public int UserId { get; set; }

        public HistoryRequestMessage()
        {
            Type = MessageType.HistoryRequest;
        }
    }
}
