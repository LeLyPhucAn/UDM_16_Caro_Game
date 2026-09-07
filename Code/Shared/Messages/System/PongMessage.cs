using System;

namespace CaroGame.Protocol.Messages.System
{
    public class PongMessage : BaseMessage
    {
        public PongMessage()
        {
            Type = MessageType.Pong;
        }
    }
}
