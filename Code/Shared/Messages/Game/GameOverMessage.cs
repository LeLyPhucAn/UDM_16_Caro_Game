namespace CaroGame.Protocol.Messages
{
    public class GameOverMessage : BaseMessage
    {
        public string ResultType { get; set; } = string.Empty; // "Win", "Draw", "Lose"
        public string WinnerName { get; set; } = string.Empty;

        public GameOverMessage()
        {
            Type = MessageType.GameOver;
        }
    }
}
