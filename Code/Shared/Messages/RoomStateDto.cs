namespace CaroGame.Protocol.Messages // Đổi namespace cho khớp với dự án của bạn
{
    public class RoomStateDto
    {
        public string RoomId { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;

        // Tên của người chơi cầm quân X (Chủ phòng)
        public string PlayerX { get; set; } = string.Empty;

        // Tên của người chơi cầm quân O (Khách)
        public string PlayerO { get; set; } = string.Empty;
    }
}