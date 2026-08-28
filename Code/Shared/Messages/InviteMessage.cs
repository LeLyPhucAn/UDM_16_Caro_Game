using CaroGame.Protocol.Messages;
namespace CaroGame.Protocol.Messages.Room
{
    /// <summary>
    /// Message dùng khi một người chơi mời người chơi khác vào phòng đấu cờ caro.
    /// </summary>
    public class InviteMessage : BaseMessage
    {
        public string TargetPlayerId { get; set; }
        public string RoomId { get; set; }

        public InviteMessage()
        {
            Type = MessageType.Invite;
        }
    }
}
