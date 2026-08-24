namespace CaroGame.Protocol
{
    /// <summary>
    /// Message client gửi lên server để yêu cầu tạo phòng mới.
    /// </summary>
    public class CreateRoomMessage : BaseMessage
    {
        public string RoomName { get; set; } = string.Empty;

        public CreateRoomMessage()
        {
            Type = MessageType.CreateRoom;
        }
    }
}
