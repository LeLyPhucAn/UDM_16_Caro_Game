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

        // Request chung
        Request,

        // ===== Room / Lobby Messages =====
        CreateRoom,
        JoinRoom,
        LeaveRoom,
        Invite,
        StartMatch,
        Ready,

        // ===== Game Messages =====
        Move,
        GameState,
        Chat,
        GameOver,

        // ===== History Messages =====
        HistoryRequest,
        HistoryResponse,

        // ===== Response Messages =====
        Response,
        Error
    }
}