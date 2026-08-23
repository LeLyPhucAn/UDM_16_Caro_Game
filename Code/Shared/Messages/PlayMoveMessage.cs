namespace CaroGame.Protocol
{
    /// <summary>
    /// Message client gửi lên server để thực hiện đánh một nước cờ.
    /// </summary>
    public class PlayMoveMessage : BaseMessage
    {
        public string MatchId { get; set; } = string.Empty;
        public int Row { get; set; }
        public int Column { get; set; }

        public PlayMoveMessage()
        {
            Type = MessageType.PlayMove;
        }
    }
}
