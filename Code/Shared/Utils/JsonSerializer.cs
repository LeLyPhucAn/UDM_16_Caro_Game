using System;
using CaroGame.Protocol.Messages;
using CaroGame.Protocol.Messages.Room;
using CaroGame.Protocol.Messages.Game;
using CaroGame.Protocol.Messages.History;
using CaroGame.Protocol.Messages.Response;
using CaroGame.Protocol.Messages.System;

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

            // Serialize theo đúng kiểu cụ thể (LoginMessage, MoveMessage, ...)
            // để không bị mất các field riêng của từng loại message.
            return System.Text.Json.JsonSerializer.Serialize(message, message.GetType(), OPTIONS);
        }

        /// <summary>
        /// Kiểm tra một chuỗi có phải JSON hợp lệ về mặt cú pháp hay không,
        /// dùng để PacketParser tách riêng lỗi "JSON sai cú pháp" khỏi lỗi
        /// "JSON đúng cú pháp nhưng thiếu/sai field" (Task 2 - Validate JSON).
        /// </summary>
        public static bool IsValidJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            try
            {
                using (System.Text.Json.JsonDocument.Parse(json))
                {
                    return true;
                }
            }
            catch (System.Text.Json.JsonException)
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra chuỗi JSON có chứa field "MessageId" dạng chuỗi và không rỗng
        /// hay không (Task 2 - Kiểm tra MessageId).
        ///
        /// Bắt buộc phải kiểm tra trên chuỗi JSON THÔ thay vì trên object sau khi
        /// Deserialize: BaseMessage tự sinh MessageId bằng Guid.NewGuid() ngay
        /// trong constructor, và System.Text.Json chỉ ghi đè property nào có mặt
        /// trong JSON - nếu người gửi không gửi MessageId, object sau deserialize
        /// vẫn "có" MessageId (do constructor gán), khiến việc kiểm tra trên object
        /// luôn pass một cách sai lệch dù packet gốc không hề mang MessageId.
        /// </summary>
        public static bool HasValidMessageId(string json)
        {
            try
            {
                using (System.Text.Json.JsonDocument document = System.Text.Json.JsonDocument.Parse(json))
                {
                    if (!document.RootElement.TryGetProperty("MessageId", out System.Text.Json.JsonElement idElement) &&
                        !document.RootElement.TryGetProperty("messageId", out idElement))
                    {
                        return false;
                    }

                    if (idElement.ValueKind != System.Text.Json.JsonValueKind.String)
                    {
                        return false;
                    }

                    string idValue = idElement.GetString();
                    return !string.IsNullOrWhiteSpace(idValue);
                }
            }
            catch (System.Text.Json.JsonException)
            {
                return false;
            }
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

                // ===== Room Messages =====
                case MessageType.CreateRoom:
                    return System.Text.Json.JsonSerializer.Deserialize<CreateRoomMessage>(json, OPTIONS);

                case MessageType.JoinRoom:
                    return System.Text.Json.JsonSerializer.Deserialize<JoinRoomMessage>(json, OPTIONS);

                case MessageType.LeaveRoom:
                    return System.Text.Json.JsonSerializer.Deserialize<LeaveRoomMessage>(json, OPTIONS);

                case MessageType.Invite:
                    return System.Text.Json.JsonSerializer.Deserialize<InviteMessage>(json, OPTIONS);

                // ===== Game Messages =====
                case MessageType.Move:
                    return System.Text.Json.JsonSerializer.Deserialize<MoveMessage>(json, OPTIONS);

                case MessageType.Turn:
                    return System.Text.Json.JsonSerializer.Deserialize<TurnMessage>(json, OPTIONS);

                case MessageType.GameState:
                    return System.Text.Json.JsonSerializer.Deserialize<GameStateMessage>(json, OPTIONS);

                case MessageType.GameResult:
                    return System.Text.Json.JsonSerializer.Deserialize<GameResultMessage>(json, OPTIONS);

                case MessageType.Timer:
                    return System.Text.Json.JsonSerializer.Deserialize<TimerMessage>(json, OPTIONS);

                // ===== History Messages =====
                case MessageType.HistoryRequest:
                    return System.Text.Json.JsonSerializer.Deserialize<HistoryRequestMessage>(json, OPTIONS);

                case MessageType.HistoryResponse:
                    return System.Text.Json.JsonSerializer.Deserialize<HistoryResponseMessage>(json, OPTIONS);

                // ===== Response Messages =====
                case MessageType.Response:
                    return System.Text.Json.JsonSerializer.Deserialize<ResponseMessage>(json, OPTIONS);

                case MessageType.Error:
                    return System.Text.Json.JsonSerializer.Deserialize<ErrorMessage>(json, OPTIONS);

                // ===== System Messages =====
                case MessageType.Ping:
                    return System.Text.Json.JsonSerializer.Deserialize<PingMessage>(json, OPTIONS);

                case MessageType.Pong:
                    return System.Text.Json.JsonSerializer.Deserialize<PongMessage>(json, OPTIONS);

                default:
                    throw new NotSupportedException("Chưa hỗ trợ deserialize cho MessageType: " + type);
            }
        }
    }
}
