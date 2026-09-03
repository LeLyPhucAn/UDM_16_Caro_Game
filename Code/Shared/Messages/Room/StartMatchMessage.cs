using System;

namespace CaroGame.Protocol.Messages.Room
{
    public class StartMatchMessage : BaseMessage
    {
        public string RoomId { get; set; } = string.Empty;

        public StartMatchMessage()
        {
            Type = MessageType.StartMatch;
        }
    }
}
