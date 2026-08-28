using CaroGame.Protocol.Messages;
namespace CaroGame.Protocol.Messages.Game
{
    /// <summary>
    /// Server gửi cho client để thông báo lượt chơi hiện tại là của ai.
    /// Thường được gửi ngay sau khi ván đấu bắt đầu, hoặc sau mỗi MoveMessage
    /// hợp lệ để chuyển lượt sang người chơi tiếp theo.
    /// </summary>
    public class TurnMessage : BaseMessage
    {
        public string RoomId { get; set; }

        /// <summary>Id của người chơi được phép đánh nước tiếp theo.</summary>
        public string CurrentPlayerId { get; set; }

        /// <summary>Số thứ tự lượt đi, tăng dần từ 1, dùng để đồng bộ và debug.</summary>
        public int TurnNumber { get; set; }

        /// <summary>Thời gian tối đa (giây) cho lượt đi này, 0 nếu không giới hạn.</summary>
        public int TimeLimitSeconds { get; set; }

        public TurnMessage()
        {
            Type = MessageType.Turn;
        }
    }
}
