namespace Shared.Messages
{
    /// <summary>
    /// Liệt kê tất cả các loại Message được trao đổi giữa Client và Server.
    /// Khi thêm chức năng mới, chỉ cần bổ sung thêm giá trị vào đây,
    /// đồng thời cập nhật JsonSerializer.Deserialize để ánh xạ đúng class.
    /// </summary>
    public enum MessageType
    {
        Login,

        // ===== Room / Lobby Messages =====
        CreateRoom,
        JoinRoom,
        LeaveRoom,
        Invite,

        // ===== Game Messages =====
        Move,
        Turn,
        GameState,
        GameResult,

        // ===== Response Messages =====
        Response,
        Error,
    }
}
