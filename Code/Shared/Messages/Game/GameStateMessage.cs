namespace CaroGame.Protocol.Messages.Game
{
    /// <summary>
    /// Server gửi cho client để đồng bộ toàn bộ trạng thái ván đấu hiện tại.
    /// Dùng khi: người chơi vừa Join Room giữa chừng, sau mỗi nước đi hợp lệ,
    /// hoặc khi client yêu cầu đồng bộ lại (ví dụ sau khi reconnect).
    /// </summary>
    public class GameStateMessage : BaseMessage
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
            Type = MessageType.GameState;
        }
    }
}
