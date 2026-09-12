namespace CaroGame.Protocol.Messages
{
    public class GameOverMessage : BaseMessage
    {
        public string RoomId { get; set; } = string.Empty;
        public string ResultType { get; set; } = string.Empty; // "Win", "Draw", "Lose", "Disconnect", "Timeout", "Surrender"
        public string WinnerId { get; set; } = string.Empty;
        public string WinnerName { get; set; } = string.Empty;
        public string[] WinningLine { get; set; } = new string[0];

        public GameOverMessage()
        {
            Type = MessageType.GameOver;
        }
    }
}
