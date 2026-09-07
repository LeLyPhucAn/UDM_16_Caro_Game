using System;
using CaroGame.Protocol;
using CaroGame.Protocol.Messages;

namespace CaroGame.Protocol.Messages
{
    public class RequestMessage : BaseMessage
    {
        public string Action { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;

        public RequestMessage()
        {
            Type = MessageType.Request;
        }

        public RequestMessage(string action, string data = "")
        {
            Type = MessageType.Request;
            Action = action;
            Data = data;
        }
    }
}