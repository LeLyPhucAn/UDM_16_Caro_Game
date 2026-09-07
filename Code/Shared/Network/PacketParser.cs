using System;
using System.Text;
using Shared.Messages;
using Shared.Utils;

namespace Shared.Network
{
    /// <summary>
    /// Đóng gói (Pack) và giải mã (Unpack) BaseMessage theo đúng cấu trúc
    /// NetworkMessage: [4 byte Type] [4 byte độ dài Body] [Body JSON UTF-8].
    ///
    /// Mỗi bước kiểm tra trong đề bài được tách thành một hàm riêng
    /// (ValidateHeader, ValidateMessageType, ValidatePayload, ValidateJson,
    /// ValidateMessageId, DeserializePacket) để dễ đối chiếu và dễ viết test
    /// cho từng mục.
    /// </summary>
    public static class PacketParser
    {
        /// <summary>Đóng gói một BaseMessage thành byte[] để gửi qua socket.</summary>
        public static byte[] Pack(BaseMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            string json = JsonSerializer.Serialize(message);
            byte[] bodyBytes = Encoding.UTF8.GetBytes(json);

            NetworkMessage netMessage = new NetworkMessage
            {
                RawType = (int)message.Type,
                BodyLength = bodyBytes.Length,
                Body = bodyBytes
            };

            return Encode(netMessage);
        }

        /// <summary>Ghép Header (Type + Length) và Body của NetworkMessage thành 1 mảng byte.</summary>
        private static byte[] Encode(NetworkMessage netMessage)
        {
            byte[] typeBytes = BitConverter.GetBytes(netMessage.RawType);
            byte[] lengthBytes = BitConverter.GetBytes(netMessage.BodyLength);

            byte[] packet = new byte[NetworkMessage.HEADER_SIZE + netMessage.Body.Length];
            Buffer.BlockCopy(typeBytes, 0, packet, 0, NetworkMessage.TYPE_SIZE);
            Buffer.BlockCopy(lengthBytes, 0, packet, NetworkMessage.TYPE_SIZE, NetworkMessage.LENGTH_SIZE);
            Buffer.BlockCopy(netMessage.Body, 0, packet, NetworkMessage.HEADER_SIZE, netMessage.Body.Length);

            return packet;
        }

        /// <summary>
        /// Giải mã một packet, chạy tuần tự qua đúng các bước Validate theo
        /// yêu cầu đề bài. Bất kỳ bước nào lỗi cũng ném PacketException kèm
        /// ErrorType tương ứng để MessageHandler xử lý riêng từng trường hợp.
        /// </summary>
        public static BaseMessage Unpack(byte[] data)
        {
            ValidateHeader(data);

            NetworkMessage netMessage = ReadHeader(data);

            ValidateMessageType(netMessage.RawType);
            ValidatePayload(data, netMessage.BodyLength);

            netMessage.Body = ReadBody(data, netMessage.BodyLength);
            string json = Encoding.UTF8.GetString(netMessage.Body);
            MessageType type = (MessageType)netMessage.RawType;

            ValidateJson(json, type);
            ValidateMessageId(json, type);

            return DeserializePacket(json, type);
        }

        /// <summary>
        /// Phiên bản an toàn của Unpack(): không ném exception ra ngoài mà
        /// trả về true/false kèm thông báo lỗi (đảm bảo Packet lỗi không làm
        /// Client/Server crash).
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

        // ===================== Rà soát Header =====================
        /// <summary>Kiểm tra data có đủ byte để đọc Header (Type + Length) hay không.</summary>
        private static void ValidateHeader(byte[] data)
        {
            if (data == null || data.Length < NetworkMessage.HEADER_SIZE)
            {
                throw new PacketException(
                    PacketErrorType.MissingData,
                    "Dữ liệu không đủ để đọc Header (cần tối thiểu " + NetworkMessage.HEADER_SIZE + " byte).");
            }
        }

        /// <summary>Đọc Type và độ dài Body từ Header thành 1 NetworkMessage (chưa đọc Body).</summary>
        private static NetworkMessage ReadHeader(byte[] data)
        {
            return new NetworkMessage
            {
                RawType = BitConverter.ToInt32(data, 0),
                BodyLength = BitConverter.ToInt32(data, NetworkMessage.TYPE_SIZE)
            };
        }

        // ===================== Kiểm tra MessageType =====================
        /// <summary>Kiểm tra rawType đọc từ Header có tồn tại trong enum MessageType hay không.</summary>
        private static void ValidateMessageType(int rawType)
        {
            if (!Enum.IsDefined(typeof(MessageType), rawType))
            {
                throw new PacketException(
                    PacketErrorType.InvalidMessageType,
                    "MessageType không hợp lệ hoặc không được hỗ trợ: " + rawType);
            }
        }

        // ===================== Rà soát Payload =====================
        /// <summary>
        /// Kiểm tra độ dài Body hợp lệ: không âm, không rỗng, không vượt giới
        /// hạn, và data có đủ byte tương ứng.
        /// Lưu ý: kiểm tra "bodyLength &gt; MAX_BODY_SIZE" được thực hiện TRƯỚC
        /// phép cộng "HEADER_SIZE + bodyLength" để tránh trường hợp bodyLength
        /// là số nguyên rất lớn gây tràn số (integer overflow) khi cộng, làm
        /// bước kiểm tra độ dài data bị bỏ qua sai lệch.
        /// </summary>
        private static void ValidatePayload(byte[] data, int bodyLength)
        {
            if (bodyLength < 0 || bodyLength > NetworkMessage.MAX_BODY_SIZE)
            {
                throw new PacketException(
                    PacketErrorType.MissingData,
                    "Payload có độ dài không hợp lệ hoặc vượt quá kích thước cho phép (" +
                    bodyLength + " byte, tối đa " + NetworkMessage.MAX_BODY_SIZE + " byte).");
            }

            if (bodyLength == 0)
            {
                throw new PacketException(
                    PacketErrorType.MissingData,
                    "Payload rỗng, không có dữ liệu để giải mã.");
            }

            if (data.Length < NetworkMessage.HEADER_SIZE + bodyLength)
            {
                throw new PacketException(
                    PacketErrorType.MissingData,
                    "Dữ liệu không đủ để đọc Body (cần " + bodyLength + " byte).");
            }
        }

        /// <summary>Đọc phần Body (bodyLength byte) từ sau Header.</summary>
        private static byte[] ReadBody(byte[] data, int bodyLength)
        {
            byte[] body = new byte[bodyLength];
            Buffer.BlockCopy(data, NetworkMessage.HEADER_SIZE, body, 0, bodyLength);
            return body;
        }

        // ===================== Kiểm tra JSON =====================
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

        // ===================== Kiểm tra MessageId =====================
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
