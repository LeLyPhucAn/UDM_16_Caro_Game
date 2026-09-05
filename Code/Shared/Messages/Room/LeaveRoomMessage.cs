namespace CaroGame.Protocol.Messages.Room
{
    /// <summary>
    /// Client gửi lên server khi người chơi rời phòng (thoát tự nguyện,
    /// mất kết nối, hoặc bị kick). Server sau khi xử lý thường sẽ broadcast
    /// lại GameStateMessage/ResponseMessage cho người chơi còn lại trong phòng.
    /// </summary>
    public class LeaveRoomMessage : BaseMessage
    {
        public string RoomId { get; set; }
        public string PlayerId { get; set; }

        /// <summary>Lý do rời phòng, ví dụ: "voluntary", "disconnect", "kicked".</summary>
        public string Reason { get; set; }

        public LeaveRoomMessage()
        {
            Type = MessageType.LeaveRoom;
        }
    }
}
