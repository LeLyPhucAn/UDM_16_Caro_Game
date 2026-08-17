namespace CaroGame.Protocol
{
    /// <summary>
    /// Liệt kê tất cả các loại Message được trao đổi giữa Client và Server.
    /// Khi thêm chức năng mới, chỉ cần bổ sung thêm giá trị vào đây.
    /// </summary>
    public enum MessageType
    {
        Login,
        Invite,
        Response,

        // Chuẩn bị mở rộng cho các tuần tiếp theo, ví dụ:
        // Move,
        // GameState,
        // Chat,
    }
}
