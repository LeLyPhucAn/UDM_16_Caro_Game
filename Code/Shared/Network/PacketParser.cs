using System;
using System.Text;
using CaroGame.Protocol.Messages;
using CaroGame.Protocol.Utils;

namespace CaroGame.Protocol.Network
{
    /// <summary>
    /// Phân loại lỗi packet, dùng để MessageHandler (Task 2) biết chính xác
    /// cần xử lý theo hướng nào:
    /// - MissingData          -> ứng với mục "Xử lý Packet thiếu dữ liệu"
    /// - InvalidMessageType   -> ứng với mục "Xử lý MessageType không hợp lệ"
    /// - CorruptedPacket      -> ứng với mục "Xử lý Packet bị lỗi" (JSON hỏng,
    ///   thiếu MessageId, hoặc deserialize thất bại)
    /// </summary>
    public enum PacketErrorType
    {
        MissingData,
        InvalidMessageType,
        CorruptedPacket
    }

    /// <summary>
    /// Exception riêng cho lỗi đóng gói/giải mã packet. Có thêm ErrorType để
    /// MessageHandler không cần đọc chuỗi Message mà vẫn biết chính xác đây
    /// là loại lỗi nào trong 3 loại mà Task 2 yêu cầu xử lý riêng.
    /// </summary>
    public class PacketException : Exception
    {
        public PacketErrorType ErrorType { get; }

        public PacketException(PacketErrorType errorType, string message) : base(message)
        {
            ErrorType = errorType;
        }

        public PacketException(PacketErrorType errorType, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorType = errorType;
        }
    }

    /// <summary>
    /// Chuẩn hóa cấu trúc dữ liệu gửi/nhận qua Socket:
    /// [4 byte Type] [4 byte độ dài Body] [Body JSON dạng UTF-8]
    ///
    /// Task 2 - mỗi bước kiểm tra trong đề bài được tách thành một hàm
    /// riêng (ValidateHeader, ValidateMessageType, ValidatePayload,
    /// ValidateJson, ValidateMessageId, DeserializePacket) để dễ đối chiếu
    /// và dễ viết test cho từng mục.
    /// </summary>
    public static class PacketParser
    {
        private const int TYPE_SIZE = 4;
        private const int LENGTH_SIZE = 4;
        private const int HEADER_SIZE = TYPE_SIZE + LENGTH_SIZE;

        /// <summary>Giới hạn Body tối đa (1 MB) để tránh packet giả mạo khai báo độ dài quá lớn.</summary>
        private const int MAX_BODY_SIZE = 1024 * 1024;

        /// <summary>Đóng gói một BaseMessage thành byte[] để gửi qua socket.</summary>
        public static byte[] Pack(BaseMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            string json = JsonSerializer.Serialize(message);
            byte[] typeBytes = BitConverter.GetBytes((int)message.Type);
            byte[] bodyBytes = Encoding.UTF8.GetBytes(json);
            byte[] lengthBytes = BitConverter.GetBytes(bodyBytes.Length);

            byte[] packet = new byte[HEADER_SIZE + bodyBytes.Length];
            Buffer.BlockCopy(typeBytes, 0, packet, 0, TYPE_SIZE);
            Buffer.BlockCopy(lengthBytes, 0, packet, TYPE_SIZE, LENGTH_SIZE);
            Buffer.BlockCopy(bodyBytes, 0, packet, HEADER_SIZE, bodyBytes.Length);

            return packet;
        }

        /// <summary>
        /// Giải mã một packet, chạy tuần tự qua đúng các bước Validate theo
        /// yêu cầu Task 2. Bất kỳ bước nào lỗi cũng ném PacketException kèm
        /// ErrorType tương ứng để MessageHandler xử lý riêng từng trường hợp.
        /// </summary>
        public static BaseMessage Unpack(byte[] data)
        {
            ValidateHeader(data);

            int rawType = BitConverter.ToInt32(data, 0);
            int bodyLength = BitConverter.ToInt32(data, TYPE_SIZE);

            MessageType type = ValidateMessageType(rawType);

            ValidatePayload(data, bodyLength);

            string json = Encoding.UTF8.GetString(data, HEADER_SIZE, bodyLength);

            ValidateJson(json, type);
            ValidateMessageId(json, type);

            return DeserializePacket(json, type);
        }

        /// <summary>
        /// Phiên bản an toàn của Unpack(): không ném exception ra ngoài mà
        /// trả về true/false kèm thông báo lỗi (Task 2 - Đảm bảo Packet lỗi
        /// không làm Client/Server crash).
        /// </summary>
        public static bool TryUnpack(byte[] data, out BaseMessage message, out string error)
        {
            try
            {
                message = Unpack(data);
                error = null;
                return true;
            }
            catch (PacketException ex)
            {
                message = null;
                error = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                message = null;
                error = "Lỗi không xác định khi giải mã packet: " + ex.Message;
                return false;
            }
        }

        // ===================== Validate Header =====================
        /// <summary>Kiểm tra data có đủ byte để đọc 8 byte Header (Type + Length) hay không.</summary>
        private static void ValidateHeader(byte[] data)
        {
            if (data == null || data.Length < HEADER_SIZE)
            {
                throw new PacketException(
                    PacketErrorType.MissingData,
                    "Dữ liệu không đủ để đọc Header (cần tối thiểu " + HEADER_SIZE + " byte).");
            }
        }

        // ===================== Validate MessageType =====================
        /// <summary>Kiểm tra rawType đọc từ Header có tồn tại trong enum MessageType hay không.</summary>
        private static MessageType ValidateMessageType(int rawType)
        {
            if (!Enum.IsDefined(typeof(MessageType), rawType))
            {
                throw new PacketException(
                    PacketErrorType.InvalidMessageType,
                    "MessageType không hợp lệ hoặc không được hỗ trợ: " + rawType);
            }

            return (MessageType)rawType;
        }

        // ===================== Validate Payload =====================
        /// <summary>Kiểm tra độ dài Body hợp lệ: đủ byte, không rỗng, không vượt giới hạn.</summary>
        private static void ValidatePayload(byte[] data, int bodyLength)
        {
            if (bodyLength < 0 || data.Length < HEADER_SIZE + bodyLength)
            {
                throw new PacketException(
                    PacketErrorType.MissingData,
                    "Dữ liệu không đủ để đọc Body (cần " + bodyLength + " byte).");
            }

            if (bodyLength == 0)
            {
                throw new PacketException(
                    PacketErrorType.MissingData,
                    "Payload rỗng, không có dữ liệu để giải mã.");
            }

            if (bodyLength > MAX_BODY_SIZE)
            {
                throw new PacketException(
                    PacketErrorType.MissingData,
                    "Payload vượt quá kích thước cho phép (" + bodyLength + " > " + MAX_BODY_SIZE + " byte).");
            }
        }

        // ===================== Validate JSON =====================
        /// <summary>Kiểm tra Body có đúng cú pháp JSON hay không.</summary>
        private static void ValidateJson(string json, MessageType type)
        {
            if (!JsonSerializer.IsValidJson(json))
            {
                throw new PacketException(
                    PacketErrorType.CorruptedPacket,
                    "Payload không phải JSON hợp lệ cho message loại " + type + ".");
            }
        }

        // ===================== Validate MessageId =====================
        /// <summary>
        /// Kiểm tra JSON có field MessageId hợp lệ hay không. Phải kiểm tra
        /// trên chuỗi JSON thô (trước khi Deserialize) vì BaseMessage luôn tự
        /// sinh MessageId mặc định trong constructor.
        /// </summary>
        private static void ValidateMessageId(string json, MessageType type)
        {
            if (!JsonSerializer.HasValidMessageId(json))
            {
                throw new PacketException(
                    PacketErrorType.CorruptedPacket,
                    "Message loại " + type + " thiếu MessageId hoặc MessageId rỗng.");
            }
        }

        // ===================== Deserialize Packet =====================
        /// <summary>Chuyển JSON thành đúng class con tương ứng với MessageType.</summary>
        private static BaseMessage DeserializePacket(string json, MessageType type)
        {
            BaseMessage message;
            try
            {
                message = JsonSerializer.Deserialize(json, type);
            }
            catch (Exception ex)
            {
                throw new PacketException(
                    PacketErrorType.CorruptedPacket,
                    "Không thể giải mã message loại " + type + ": " + ex.Message, ex);
            }

            if (message == null)
            {
                throw new PacketException(
                    PacketErrorType.CorruptedPacket,
                    "Giải mã message loại " + type + " trả về null.");
            }

            return message;
        }
    }
}
