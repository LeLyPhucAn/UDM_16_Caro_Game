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

        CreateRoom = 4,
        JoinRoom = 5,
        LeaveRoom = 6,
        PlayMove = 7,
    }
}
