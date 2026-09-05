using System;

namespace CaroGame.Protocol.Messages.System
{
    public class PingMessage : BaseMessage
    {
        public PingMessage()
        {
            Type = MessageType.Ping;
        }
    }
}
