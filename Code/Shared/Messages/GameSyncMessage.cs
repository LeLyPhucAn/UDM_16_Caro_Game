using CaroGame.Protocol.Messages;
using System;

namespace CaroGame.Protocol
{
    public class GameSyncMessage : BaseMessage
    {
        public string PlayerXName { get; set; } = string.Empty;
        public string PlayerOName { get; set; } = string.Empty;

        // Cho biết người dùng hiện tại đang cầm cờ gì ("X" hay "O")
        public string MySymbol { get; set; } = string.Empty;

        // Tên của người đang đến lượt đánh (Để Client đối chiếu mở/khóa bàn cờ)
        public string CurrentTurnName { get; set; } = string.Empty;

        public GameSyncMessage()
        {
            Type = MessageType.GameSync;
        }
    }
}
