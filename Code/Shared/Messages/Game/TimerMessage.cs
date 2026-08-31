namespace CaroGame.Protocol.Messages.Game
{
    public class TimerMessage : BaseMessage
    {
        public string RoomId { get; set; }
        public int RemainingSeconds { get; set; }

        public TimerMessage()
        {
            Type = MessageType.Timer;
        }
    }
}
