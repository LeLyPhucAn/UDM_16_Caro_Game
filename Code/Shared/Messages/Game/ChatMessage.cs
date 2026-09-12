namespace CaroGame.Protocol.Messages
{
    public class ChatMessage : BaseMessage
    {
        public string RoomId { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public ChatMessage()
        {
            Type = MessageType.Chat;
        }
    }
}
