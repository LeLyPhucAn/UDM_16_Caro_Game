namespace CaroGame.Protocol
{
    /// <summary>
    /// Message client gửi lên server để yêu cầu tham gia phòng.
    /// </summary>
    public class JoinRoomMessage : BaseMessage
    {
        public string RoomId { get; set; } = string.Empty;

        public JoinRoomMessage()
        {
            Type = MessageType.JoinRoom;
        }
    }
}
