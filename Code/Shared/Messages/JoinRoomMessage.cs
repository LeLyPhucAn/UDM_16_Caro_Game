namespace CaroGame.Protocol
{
    /// <summary>
    /// Client gửi lên server để xin vào một phòng đã tồn tại.
    /// Server phản hồi bằng ResponseMessage; nếu thành công, Data có thể chứa
    /// GameStateMessage hiện tại của phòng để client đồng bộ trạng thái.
    /// </summary>
    public class JoinRoomMessage : BaseMessage
    {
        public string RoomId { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;

        /// <summary>Mật khẩu phòng, chỉ cần điền khi phòng ở chế độ riêng tư.</summary>
        public string Password { get; set; } = string.Empty;
        
        public JoinRoomMessage()
        {
            Type = MessageType.JoinRoom;
        }
    }
}
