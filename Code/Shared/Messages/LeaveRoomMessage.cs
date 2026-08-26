namespace CaroGame.Protocol
{
    /// <summary>
    /// Client gửi lên server khi người chơi rời phòng (thoát tự nguyện,
    /// mất kết nối, hoặc bị kick). Server sau khi xử lý thường sẽ broadcast
    /// lại GameStateMessage/ResponseMessage cho người chơi còn lại trong phòng.
    /// </summary>
    public class LeaveRoomMessage : BaseMessage
    {
        public string RoomId { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;

        /// <summary>Lý do rời phòng, ví dụ: "voluntary", "disconnect", "kicked".</summary>
        public string Reason { get; set; } = string.Empty;
        
        public LeaveRoomMessage()
        {
            Type = MessageType.LeaveRoom;
        }
    }
}
