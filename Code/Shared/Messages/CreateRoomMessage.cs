namespace CaroGame.Protocol.Messages.Room
{
    /// <summary>
    /// Client gửi lên server để yêu cầu tạo một phòng chơi Caro mới.
    /// Server sẽ phản hồi lại bằng ResponseMessage, trong đó Data chứa RoomId
    /// vừa được tạo (nếu Success = true).
    /// </summary>
    public class CreateRoomMessage : BaseMessage
    {
        /// <summary>Tên phòng hiển thị trong danh sách phòng (Lobby).</summary>
        public string RoomName { get; set; } = string.Empty;

        /// <summary>Id của người tạo phòng, mặc định sẽ là chủ phòng (host).</summary>
        public string HostId { get; set; } = string.Empty;

        /// <summary>Số người chơi tối đa được phép vào phòng (Caro thường là 2).</summary>
        public int MaxPlayers { get; set; }

        /// <summary>Kích thước bàn cờ, ví dụ 15 nghĩa là bàn 15x15.</summary>
        public int BoardSize { get; set; }

        /// <summary>Phòng có mật khẩu hay không.</summary>
        public bool IsPrivate { get; set; }

        /// <summary>Mật khẩu phòng, chỉ có ý nghĩa khi IsPrivate = true.</summary>
        public string Password { get; set; } = string.Empty;
        
        public CreateRoomMessage()
        {
            Type = MessageType.CreateRoom;
        }
    }
}
