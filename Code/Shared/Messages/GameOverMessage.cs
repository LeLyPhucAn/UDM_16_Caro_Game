using System;
using System.Collections.Generic;

namespace CaroGame.Protocol
{
    // DTO siêu nhỏ để lưu tọa độ không phụ thuộc vào System.Drawing
    public class Coordinate
    {
        public int Row { get; set; }
        public int Col { get; set; }
    }

    public class GameOverMessage : BaseMessage
    {
        // Tên người thắng (Nếu để rỗng hoặc null nghĩa là HÒA)
        public string WinnerName { get; set; } = string.Empty;

        // Loại kết quả: "Win", "Draw", hoặc "OpponentLeft" (Đối thủ thoát ngang)
        public string ResultType { get; set; } = string.Empty;

        // Danh sách 5 tọa độ để Client làm hiệu ứng chớp sáng/đổi màu
        public List<Coordinate> WinningLine { get; set; } = new List<Coordinate>();

        public GameOverMessage()
        {
            Type = MessageType.GameOver;
        }
    }
}