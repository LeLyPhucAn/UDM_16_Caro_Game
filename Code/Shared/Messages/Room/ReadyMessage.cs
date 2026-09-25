namespace CaroGame.Protocol.Messages.Room
{
    public class ReadyMessage : BaseMessage
    {
        public string RoomId { get; set; }
        public bool IsReady { get; set; }

        public ReadyMessage()
        {
            Type = MessageType.Ready;
        }
    }
}
