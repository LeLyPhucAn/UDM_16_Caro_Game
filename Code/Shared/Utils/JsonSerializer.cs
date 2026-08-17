using System;
using System.Text.Json;

namespace CaroGame.Protocol
{
    /// <summary>
    /// Lớp tiện ích chuyển đổi giữa Object (Message) và chuỗi JSON.
    /// Dùng System.Text.Json phía dưới, được gọi đầy đủ để tránh trùng tên
    /// với chính class này.
    /// </summary>
    public static class JsonSerializer
    {
        private static readonly JsonSerializerOptions OPTIONS = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public static string Serialize(BaseMessage message)
        {
            // Serialize theo đúng kiểu cụ thể (LoginMessage, InviteMessage, ...)
            // để không bị mất các field riêng của từng loại message.
            return System.Text.Json.JsonSerializer.Serialize(message, message.GetType(), OPTIONS);
        }

        public static BaseMessage Deserialize(string json, MessageType type)
        {
            return type switch
            {
                MessageType.Login => System.Text.Json.JsonSerializer.Deserialize<LoginMessage>(json, OPTIONS),
                MessageType.Invite => System.Text.Json.JsonSerializer.Deserialize<InviteMessage>(json, OPTIONS),
                MessageType.Response => System.Text.Json.JsonSerializer.Deserialize<ResponseMessage>(json, OPTIONS),
                _ => throw new NotSupportedException($"Chưa hỗ trợ deserialize cho MessageType: {type}")
            };
        }
    }
}
