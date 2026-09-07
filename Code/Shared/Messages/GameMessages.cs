namespace Shared.Messages.Game
{
    /// <summary>
    /// Client gửi lên server khi người chơi đánh một nước cờ (đặt quân X/O
    /// tại một ô trên bàn cờ). Server validate nước đi hợp lệ rồi broadcast
    /// GameStateMessage/TurnMessage mới cho cả 2 người chơi trong phòng.
    /// </summary>
    public class MoveMessage : Shared.Messages.BaseMessage
    {
        public string RoomId { get; set; }
        public string PlayerId { get; set; }

        /// <summary>Chỉ số hàng trên bàn cờ, bắt đầu từ 0.</summary>
        public int Row { get; set; }

        /// <summary>Chỉ số cột trên bàn cờ, bắt đầu từ 0.</summary>
        public int Column { get; set; }

        /// <summary>Ký hiệu quân cờ của người chơi, ví dụ "X" hoặc "O".</summary>
        public string Symbol { get; set; }

        public MoveMessage()
        {
            Type = Shared.Messages.MessageType.Move;
        }
    }

    /// <summary>
    /// Server gửi cho client để thông báo lượt chơi hiện tại là của ai.
    /// Thường được gửi ngay sau khi ván đấu bắt đầu, hoặc sau mỗi MoveMessage
    /// hợp lệ để chuyển lượt sang người chơi tiếp theo.
    /// </summary>
    public class TurnMessage : Shared.Messages.BaseMessage
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
            Type = Shared.Messages.MessageType.Turn;
        }
    }

    /// <summary>
    /// Server gửi cho client để đồng bộ toàn bộ trạng thái ván đấu hiện tại.
    /// Dùng khi: người chơi vừa Join Room giữa chừng, sau mỗi nước đi hợp lệ,
    /// hoặc khi client yêu cầu đồng bộ lại (ví dụ sau khi reconnect).
    /// </summary>
    public class GameStateMessage : Shared.Messages.BaseMessage
    {
        public string RoomId { get; set; }

        /// <summary>
        /// Trạng thái bàn cờ dạng chuỗi phẳng, mỗi ký tự đại diện một ô
        /// (ví dụ '-' là ô trống, 'X'/'O' là quân cờ), độ dài = BoardSize * BoardSize.
        /// Dùng dạng chuỗi để dễ serialize JSON, không cần mảng 2 chiều.
        /// </summary>
        public string BoardState { get; set; }

        public int BoardSize { get; set; }

        /// <summary>Id người chơi đang tới lượt.</summary>
        public string CurrentPlayerId { get; set; }

        /// <summary>Trạng thái ván đấu: "Waiting", "Playing", "Finished".</summary>
        public string Status { get; set; }

        public GameStateMessage()
        {
            Type = Shared.Messages.MessageType.GameState;
        }
    }

    /// <summary>
    /// Server gửi cho client khi ván đấu kết thúc: có người thắng, hòa,
    /// hoặc một bên đầu hàng/mất kết nối quá thời gian quy định.
    /// </summary>
    public class GameResultMessage : Shared.Messages.BaseMessage
    {
        public string RoomId { get; set; }

        /// <summary>Id người thắng cuộc, để trống nếu hòa.</summary>
        public string WinnerId { get; set; }

        /// <summary>Loại kết quả: "Win", "Draw", "Surrender", "Disconnect".</summary>
        public string ResultType { get; set; }

        /// <summary>
        /// Danh sách toạ độ (dạng "row,col") tạo thành hàng 5 quân thắng cuộc,
        /// dùng để client vẽ highlight trên bàn cờ. Để trống nếu hòa/đầu hàng.
        /// </summary>
        public string[] WinningLine { get; set; }

        public GameResultMessage()
        {
            Type = Shared.Messages.MessageType.GameResult;
        }
    }
}
