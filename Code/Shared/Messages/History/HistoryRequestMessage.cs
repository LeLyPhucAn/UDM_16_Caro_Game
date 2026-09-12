namespace CaroGame.Protocol.Messages.History
{
    public class HistoryRequestMessage : BaseMessage
    {
        public string Username { get; set; } = string.Empty;

        public HistoryRequestMessage()
        {
            Type = MessageType.HistoryRequest;
        }
    }
}
