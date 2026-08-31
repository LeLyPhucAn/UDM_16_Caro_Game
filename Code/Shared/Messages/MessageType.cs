namespace CaroGame.Protocol
{
    /// <summary>
    /// Liệt kê tất cả các loại Message được trao đổi giữa Client và Server.
    /// Khi thêm chức năng mới, chỉ cần bổ sung thêm giá trị vào đây.
    /// </summary>
    public enum MessageType
    {
        Login = 0,
        Invite = 1,
        Response = 2,
        Error = 3,

        // Chuẩn bị mở rộng cho các tuần tiếp theo, ví dụ:
        // Move,
        // GameState,
        // Chat,
        GameSync,   // Đồng bộ thông tin ván đấu (Tên người chơi, Ký hiệu X/O)
        Move,       // Gửi/Nhận tọa độ nước đi
        GameOver,    // Thông báo kết thúc ván (Kèm danh sách 5 ô chiến thắng)
        Request
    }
}
