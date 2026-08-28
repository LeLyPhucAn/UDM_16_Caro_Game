using CaroGame.Protocol.Messages;
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
        /// Định danh duy nhất cho từng message, dùng để:
        /// - PacketParser validate packet (Task 2 - Kiểm tra MessageId).
        /// - Đối chiếu Request/Response (ResponseMessage.RequestMessageId).
        /// - Log/debug khi cần truy vết một message cụ thể.
        /// Được sinh tự động khi tạo message, không cần set thủ công.
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
