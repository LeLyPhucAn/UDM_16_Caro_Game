namespace CaroGame.Protocol.Messages.Room
{
    /// <summary>
    /// Client gửi lên server để xin vào một phòng đã tồn tại.
    /// Server phản hồi bằng ResponseMessage; nếu thành công, Data có thể chứa
    /// GameStateMessage hiện tại của phòng để client đồng bộ trạng thái.
    /// </summary>
    public class JoinRoomMessage : BaseMessage
    {
        public string RoomId { get; set; }
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }

        /// <summary>Mật khẩu phòng, chỉ cần điền khi phòng ở chế độ riêng tư.</summary>
        public string Password { get; set; }

        public JoinRoomMessage()
        {
            Type = MessageType.JoinRoom;
        }
    }
}
