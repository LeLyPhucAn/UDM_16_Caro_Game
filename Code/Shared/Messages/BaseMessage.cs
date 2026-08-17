using System;

namespace CaroGame.Protocol
{
    /// <summary>
    /// Lớp cơ sở cho tất cả các Message. Mọi message cụ thể (LoginMessage,
    /// InviteMessage, ResponseMessage, ...) đều kế thừa từ đây.
    /// </summary>
    public class BaseMessage
    {
        public MessageType Type { get; set; }
        public string SenderId { get; set; }
        public long Timestamp { get; set; }

        public BaseMessage()
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}
