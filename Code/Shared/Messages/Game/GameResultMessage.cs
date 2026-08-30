namespace CaroGame.Protocol.Messages.Game
{
    /// <summary>
    /// Server gửi cho client khi ván đấu kết thúc: có người thắng, hòa,
    /// hoặc một bên đầu hàng/mất kết nối quá thời gian quy định.
    /// </summary>
    public class GameResultMessage : BaseMessage
    {
        public string RoomId { get; set; }

        /// <summary>Id người thắng cuộc, để trống nếu hòa.</summary>
        public string WinnerId { get; set; }

        public string WinnerName { get; set; }

        /// <summary>Loại kết quả: "Win", "Draw", "Surrender", "Disconnect".</summary>
        public string ResultType { get; set; }

        /// <summary>
        /// Danh sách toạ độ (dạng "row,col") tạo thành hàng 5 quân thắng cuộc,
        /// dùng để client vẽ highlight trên bàn cờ. Để trống nếu hòa/đầu hàng.
        /// </summary>
        public string[] WinningLine { get; set; }

        public GameResultMessage()
        {
            Type = MessageType.GameResult;
        }
    }
}
