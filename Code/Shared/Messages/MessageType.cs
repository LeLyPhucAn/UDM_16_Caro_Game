namespace CaroGame.Protocol.Messages
{
    /// <summary>
    /// Liệt kê tất cả các loại Message được trao đổi giữa Client và Server.
    /// Khi thêm chức năng mới, chỉ cần bổ sung thêm giá trị vào đây,
    /// đồng thời cập nhật JsonSerializer.Deserialize để ánh xạ đúng class.
    /// </summary>
public enum MessageType
    {
        Login, // Giữ nguyên các type cũ ở trên


        // Chuẩn bị mở rộng cho các tuần tiếp theo, ví dụ:
        // Move,
        // GameState,
        // Chat,
        GameSync,   // Đồng bộ thông tin ván đấu (Tên người chơi, Ký hiệu X/O)
        Move,       // Gửi/Nhận tọa độ nước đi
        GameOver,   // Thông báo kết thúc ván (Kèm danh sách 5 ô chiến thắng)

        // ===== Room / Lobby Messages (Task 1) =====
        CreateRoom = 4,
        JoinRoom = 5,
        LeaveRoom = 6,
        Invite = 7,

        // ===== Game Messages (Task 1) =====
        Turn = 9,
        GameState = 10,
        GameResult = 11,

        // ===== Response Messages (Task 1) =====
        Response = 12,
        Error = 13

    }
}
