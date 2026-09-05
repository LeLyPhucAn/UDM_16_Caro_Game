using CaroGame.Protocol.Messages;
using System;
using System.Text;
using CaroGame.Protocol.Messages;
using CaroGame.Protocol.Utils;

namespace CaroGame.Protocol.Network
{
    /// <summary>
    /// Chuẩn hóa cấu trúc dữ liệu gửi/nhận qua Socket, gồm 3 phần:
    /// [4 byte Type] [4 byte độ dài Body] [Body JSON dạng UTF-8]
    /// Cách đóng gói này giúp SocketServer luôn biết chính xác cần đọc
    /// bao nhiêu byte cho một message hoàn chỉnh.
    ///
    /// Task 2 - Packet Routing &amp; Validation: Unpack() thực hiện tuần tự
    /// các bước kiểm tra Header -> MessageType -> Payload -> Validate JSON
    /// -> kiểm tra MessageId -> Deserialize, mọi lỗi ở bất kỳ bước nào đều
    /// được gói lại thành PacketException để SocketServer xử lý mà không
    /// làm crash chương trình. Nên dùng TryUnpack() ở vòng lặp đọc socket
    /// để không cần try/catch thủ công ở nơi gọi.
    /// </summary>
    public static class PacketParser
    {
        // Header gồm 2 phần: 4 byte lưu Type, 4 byte lưu độ dài Body
        private const int TYPE_SIZE = 4;
        private const int LENGTH_SIZE = 4;
        private const int HEADER_SIZE = TYPE_SIZE + LENGTH_SIZE;

        /// <summary>
        /// Giới hạn kích thước Body tối đa (1 MB) để tránh trường hợp packet
        /// bị lỗi/giả mạo khai báo độ dài quá lớn làm tràn bộ nhớ khi đọc.
        /// </summary>
        private const int MAX_BODY_SIZE = 1024 * 1024;

        /// <summary>
        /// Đóng gói một BaseMessage thành byte[] để gửi qua socket.
        /// </summary>
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
        /// Giải mã một packet hoàn chỉnh (đã đủ Header + Body) thành BaseMessage.
        /// Nếu dữ liệu không hợp lệ ở bất kỳ bước nào (thiếu byte, sai
        /// MessageType, JSON hỏng, thiếu MessageId, ...) sẽ ném PacketException
        /// để nơi gọi (SocketServer) tự xử lý, thay vì làm crash chương trình.
        /// </summary>
        public static BaseMessage Unpack(byte[] data)
        {
            // ===== Bước 1: Kiểm tra Header =====
            if (data == null || data.Length < HEADER_SIZE)
            {
                throw new PacketException(
                    "Dữ liệu không đủ để đọc Header (cần tối thiểu " + HEADER_SIZE + " byte).");
            }

            int rawType = BitConverter.ToInt32(data, 0);
            int bodyLength = BitConverter.ToInt32(data, TYPE_SIZE);

            // ===== Bước 2: Kiểm tra MessageType =====
            // rawType phải ứng với một giá trị đã khai báo trong enum MessageType,
            // tránh trường hợp client cũ/mới lệch phiên bản gửi type không tồn tại.
            if (!Enum.IsDefined(typeof(MessageType), rawType))
            {
                throw new PacketException("MessageType không hợp lệ hoặc không được hỗ trợ: " + rawType);
            }

            MessageType type = (MessageType)rawType;

            // ===== Bước 3: Kiểm tra Payload =====
            if (bodyLength < 0 || data.Length < HEADER_SIZE + bodyLength)
            {
                throw new PacketException("Dữ liệu không đủ để đọc Body (cần " + bodyLength + " byte).");
            }

            if (bodyLength == 0)
            {
                throw new PacketException("Payload rỗng, không có dữ liệu để giải mã.");
            }

            if (bodyLength > MAX_BODY_SIZE)
            {
                throw new PacketException(
                    "Payload vượt quá kích thước cho phép (" + bodyLength + " > " + MAX_BODY_SIZE + " byte).");
            }

            string json = Encoding.UTF8.GetString(data, HEADER_SIZE, bodyLength);

            // ===== Bước 4: Validate JSON =====
            // Kiểm tra cú pháp JSON trước, để phân biệt rõ lỗi "JSON hỏng"
            // với lỗi "JSON đúng nhưng thiếu field" xảy ra ở các bước sau.
            if (!JsonSerializer.IsValidJson(json))
            {
                throw new PacketException("Payload không phải JSON hợp lệ cho message loại " + type + ".");
            }

            // ===== Bước 5: Kiểm tra MessageId =====
            // Phải kiểm tra trên chuỗi JSON thô (trước khi Deserialize), vì object
            // BaseMessage luôn tự sinh MessageId mặc định trong constructor -
            // nếu kiểm tra sau Deserialize, packet thiếu MessageId vẫn "lọt qua"
            // một cách sai lệch (xem giải thích chi tiết tại JsonSerializer.HasValidMessageId).
            if (!JsonSerializer.HasValidMessageId(json))
            {
                throw new PacketException("Message loại " + type + " thiếu MessageId hoặc MessageId rỗng.");
            }

            // ===== Bước 6: Deserialize =====
            BaseMessage message;
            try
            {
                message = JsonSerializer.Deserialize(json, type);
            }
            catch (Exception ex)
            {
                throw new PacketException("Không thể giải mã message loại " + type + ": " + ex.Message, ex);
            }

            if (message == null)
            {
                throw new PacketException("Giải mã message loại " + type + " trả về null.");
            }

            return message;
        }

        /// <summary>
        /// Phiên bản an toàn của Unpack(): không ném exception ra ngoài mà trả
        /// về true/false kèm thông báo lỗi, giúp vòng lặp đọc dữ liệu của
        /// SocketServer xử lý packet lỗi một cách gọn gàng mà không bao giờ
        /// làm crash chương trình (Task 2 - Không để Packet lỗi làm crash).
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
                // Bắt luôn các lỗi phát sinh ngoài dự kiến (ví dụ lỗi hệ thống khi
                // Reflection/Deserialize nội bộ) để tuyệt đối không làm sập server.
                message = null;
                error = "Lỗi không xác định khi giải mã packet: " + ex.Message;
                return false;
            }
        }
    }

    /// <summary>
    /// Exception riêng cho lỗi đóng gói/giải mã packet, giúp phân biệt với các
    /// exception hệ thống khác khi SocketServer bắt lỗi.
    /// </summary>
    public class PacketException : Exception
    {
        public PacketException(string message) : base(message)
        {
        }

        public PacketException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
