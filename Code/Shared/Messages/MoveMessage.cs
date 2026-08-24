using System;

namespace CaroGame.Protocol
{
    public class MoveMessage : BaseMessage
    {
        public int Row { get; set; }
        public int Col { get; set; }

        // Chứa "X" hoặc "O"
        public string Symbol { get; set; } = string.Empty;

        public MoveMessage()
        {
            // Tự động gán nhãn loại tin nhắn khi khởi tạo
            Type = MessageType.Move;
        }
    }
}