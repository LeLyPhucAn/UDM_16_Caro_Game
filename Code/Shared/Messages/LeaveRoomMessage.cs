namespace CaroGame.Protocol
{
    /// <summary>
    /// Message client gửi lên server để yêu cầu rời phòng.
    /// </summary>
    public class LeaveRoomMessage : BaseMessage
    {
        public string RoomId { get; set; } = string.Empty;

        public LeaveRoomMessage()
        {
            Type = MessageType.LeaveRoom;
        }
    }
}
