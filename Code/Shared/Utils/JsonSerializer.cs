using System;
using CaroGame.Protocol.Messages;

namespace CaroGame.Protocol.Utils
{
    /// <summary>
    /// Lớp tiện ích chuyển đổi giữa Object (Message) và chuỗi JSON.
    /// Dùng System.Text.Json phía dưới, được gọi đầy đủ để tránh trùng tên
    /// với chính class này.
    /// </summary>
    public static class JsonSerializer
    {
        private static readonly System.Text.Json.JsonSerializerOptions OPTIONS = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public static string Serialize(BaseMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            // Serialize theo đúng kiểu cụ thể (LoginMessage, InviteMessage, ...)
            // để không bị mất các field riêng của từng loại message.
            return System.Text.Json.JsonSerializer.Serialize(message, message.GetType(), OPTIONS);
        }

        public static BaseMessage Deserialize(string json, MessageType type)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new ArgumentException("Chuỗi JSON không được rỗng.", nameof(json));
            }

            // Dựa vào Type để biết cần chuyển JSON về đúng class con nào
            switch (type)
            {
                case MessageType.Login:
                    return System.Text.Json.JsonSerializer.Deserialize<LoginMessage>(json, OPTIONS);

                case MessageType.Invite:
                    return System.Text.Json.JsonSerializer.Deserialize<InviteMessage>(json, OPTIONS);

                case MessageType.Response:
                    return System.Text.Json.JsonSerializer.Deserialize<ResponseMessage>(json, OPTIONS);

                case MessageType.Error:
                    return System.Text.Json.JsonSerializer.Deserialize<ErrorMessage>(json, OPTIONS);

                default:
                    throw new NotSupportedException("Chưa hỗ trợ deserialize cho MessageType: " + type);
            }
        }
    }
}
