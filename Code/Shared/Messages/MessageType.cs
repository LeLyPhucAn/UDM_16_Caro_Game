namespace CaroGame.Protocol.Messages
{
    /// <summary>
    /// Liệt kê tất cả các loại Message được trao đổi giữa Client và Server.
    /// Khi thêm chức năng mới, chỉ cần bổ sung thêm giá trị vào đây,
    /// đồng thời cập nhật JsonSerializer.Deserialize để ánh xạ đúng class.
    /// </summary>
    public enum MessageType
    {
        Login,
        Register,

        // [TASK 1] Bổ sung Type Generic cho các Request chung (StartGame, LeaveRoom...)
        Request,

        // ===== Room / Lobby Messages (Cấu trúc mới của Team) =====
        CreateRoom,
        JoinRoom,
        LeaveRoom,
        Invite,
        StartMatch,
        Ready,

        // ===== Game Messages =====
        Move,
        Turn,
        GameState,

        Timer,
        Chat,

        // [TASK 1] Bổ sung Type cho Bàn cờ (GameForm)
        GameSync,
        GameOver,

        // ===== History Messages =====
        HistoryRequest,
        HistoryResponse,

        // ===== Response Messages =====
        Response,
        Error,

        // ===== System / Network Messages =====
        Ping,
        Pong,
    }
}