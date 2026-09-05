namespace CaroGame.Protocol.Messages
{
    public class GameSyncMessage : BaseMessage
    {
        public string PlayerXName { get; set; } = string.Empty;
        public string PlayerOName { get; set; } = string.Empty;
        public string MySymbol { get; set; } = string.Empty;
        public string CurrentTurnName { get; set; } = string.Empty;

        public GameSyncMessage()
        {
            Type = MessageType.GameSync;
        }
    }
}