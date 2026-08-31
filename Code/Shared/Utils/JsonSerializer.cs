using System;
using System.Text.Json;
using CaroGame.Protocol.Messages;


namespace CaroGame.Protocol
{
    /// <summary>
    /// Lớp tiện ích chuyển đổi giữa Object (Message) và chuỗi JSON.
    /// </summary>
    public static class JsonSerializer
    {
        private static readonly JsonSerializerOptions OPTIONS = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public static string Serialize(BaseMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return System.Text.Json.JsonSerializer.Serialize(message, message.GetType(), OPTIONS);
        }

        public static BaseMessage Deserialize(string json, MessageType type)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new ArgumentException("Chuỗi JSON không được rỗng.", nameof(json));
            }

            switch (type)
            {
                case MessageType.Login:
                    return System.Text.Json.JsonSerializer.Deserialize<LoginMessage>(json, OPTIONS) ?? throw new InvalidOperationException("Deserialize LoginMessage returned null.");

                case MessageType.Invite:
                    return System.Text.Json.JsonSerializer.Deserialize<InviteMessage>(json, OPTIONS) ?? throw new InvalidOperationException("Deserialize InviteMessage returned null.");

                case MessageType.Response:
                    return System.Text.Json.JsonSerializer.Deserialize<ResponseMessage>(json, OPTIONS) ?? throw new InvalidOperationException("Deserialize ResponseMessage returned null.");

                case MessageType.Error:
                    return System.Text.Json.JsonSerializer.Deserialize<ErrorMessage>(json, OPTIONS) ?? throw new InvalidOperationException("Deserialize ErrorMessage returned null.");

                case MessageType.Request:
                    return System.Text.Json.JsonSerializer.Deserialize<RequestMessage>(json, OPTIONS);
                default:
                    throw new NotSupportedException("Chưa hỗ trợ deserialize cho MessageType: " + type);
            }
        }
    }
}
