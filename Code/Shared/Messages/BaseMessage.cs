using System;

namespace CaroGame.Protocol.Messages
{
    /// <summary>
    /// Lớp cơ sở cho tất cả các Message. Mọi message cụ thể (LoginMessage,
    /// RoomMessages, GameMessages, ResponseMessages, ...) đều kế thừa từ đây.
    /// </summary>
    public class BaseMessage
    {
        public MessageType Type { get; set; }

        /// <summary>
        /// Định danh duy nhất cho từng message (Guid).
        /// </summary>
        public string MessageId { get; set; }

        public string SenderId { get; set; }
        public long Timestamp { get; set; }

        public BaseMessage()
        {
            MessageId = Guid.NewGuid().ToString();
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}
