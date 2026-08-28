using CaroGame.Protocol.Messages;
using System;

namespace CaroGame.Protocol.Messages.Game
{
    /// <summary>
    /// Client gửi lên server khi người chơi đánh một nước cờ (đặt quân X/O
    /// tại một ô trên bàn cờ). Server validate nước đi hợp lệ rồi broadcast
    /// GameStateMessage/TurnMessage mới cho cả 2 người chơi trong phòng.
    /// </summary>
    public class MoveMessage : BaseMessage
    {
        /// <summary>Mã phòng của ván đấu hiện tại.</summary>
        public string RoomId { get; set; } = string.Empty;

        /// <summary>ID của người chơi thực hiện nước đi.</summary>
        public string PlayerId { get; set; } = string.Empty;

        /// <summary>Chỉ số hàng trên bàn cờ, bắt đầu từ 0.</summary>
        public int Row { get; set; }

        /// <summary>Chỉ số cột trên bàn cờ, bắt đầu từ 0.</summary>
        public int Column { get; set; }

        /// <summary>Ký hiệu quân cờ của người chơi, ví dụ "X" hoặc "O".</summary>
        public string Symbol { get; set; } = string.Empty;

        public MoveMessage()
        {
            Type = MessageType.Move;
        }
    }
}
