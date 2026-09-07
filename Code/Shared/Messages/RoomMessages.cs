namespace Shared.Messages.Room
{
    /// <summary>
    /// Client gửi lên server để yêu cầu tạo một phòng chơi Caro mới.
    /// Server sẽ phản hồi lại bằng ResponseMessage, trong đó Data chứa RoomId
    /// vừa được tạo (nếu Success = true).
    /// </summary>
    public class CreateRoomMessage : Shared.Messages.BaseMessage
    {
        /// <summary>Tên phòng hiển thị trong danh sách phòng (Lobby).</summary>
        public string RoomName { get; set; }

        /// <summary>Id của người tạo phòng, mặc định sẽ là chủ phòng (host).</summary>
        public string HostId { get; set; }

        /// <summary>Số người chơi tối đa được phép vào phòng (Caro thường là 2).</summary>
        public int MaxPlayers { get; set; }

        /// <summary>Kích thước bàn cờ, ví dụ 15 nghĩa là bàn 15x15.</summary>
        public int BoardSize { get; set; }

        /// <summary>Phòng có mật khẩu hay không.</summary>
        public bool IsPrivate { get; set; }

        /// <summary>Mật khẩu phòng, chỉ có ý nghĩa khi IsPrivate = true.</summary>
        public string Password { get; set; }

        public CreateRoomMessage()
        {
            Type = Shared.Messages.MessageType.CreateRoom;
        }
    }

    /// <summary>
    /// Client gửi lên server để xin vào một phòng đã tồn tại.
    /// Server phản hồi bằng ResponseMessage; nếu thành công, Data có thể chứa
    /// GameStateMessage hiện tại của phòng để client đồng bộ trạng thái.
    /// </summary>
    public class JoinRoomMessage : Shared.Messages.BaseMessage
    {
        public string RoomId { get; set; }
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }

        /// <summary>Mật khẩu phòng, chỉ cần điền khi phòng ở chế độ riêng tư.</summary>
        public string Password { get; set; }

        public JoinRoomMessage()
        {
            Type = Shared.Messages.MessageType.JoinRoom;
        }
    }

    /// <summary>
    /// Client gửi lên server khi người chơi rời phòng (thoát tự nguyện,
    /// mất kết nối, hoặc bị kick). Server sau khi xử lý thường sẽ broadcast
    /// lại GameStateMessage/ResponseMessage cho người chơi còn lại trong phòng.
    /// </summary>
    public class LeaveRoomMessage : Shared.Messages.BaseMessage
    {
        public string RoomId { get; set; }
        public string PlayerId { get; set; }

        /// <summary>Lý do rời phòng, ví dụ: "voluntary", "disconnect", "kicked".</summary>
        public string Reason { get; set; }

        public LeaveRoomMessage()
        {
            Type = Shared.Messages.MessageType.LeaveRoom;
        }
    }

    /// <summary>
    /// Message dùng khi một người chơi mời người chơi khác vào phòng đấu cờ caro.
    /// </summary>
    public class InviteMessage : Shared.Messages.BaseMessage
    {
        public string TargetPlayerId { get; set; }
        public string RoomId { get; set; }

        public InviteMessage()
        {
            Type = Shared.Messages.MessageType.Invite;
        }
    }
}
