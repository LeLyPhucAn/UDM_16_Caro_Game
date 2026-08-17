using System;
using System.Text;

namespace CaroGame.Protocol
{
    /// <summary>
    /// Chuẩn hóa cấu trúc dữ liệu gửi/nhận qua Socket, gồm 3 phần:
    /// [4 byte Type] [4 byte độ dài Body] [Body JSON dạng UTF-8]
    /// Cách đóng gói này giúp SocketServer luôn biết chính xác cần đọc
    /// bao nhiêu byte cho một message hoàn chỉnh.
    /// </summary>
    public static class PacketParser
    {
        // Header gồm 2 phần: 4 byte lưu Type, 4 byte lưu độ dài Body
        private const int TYPE_SIZE = 4;
        private const int LENGTH_SIZE = 4;
        private const int HEADER_SIZE = TYPE_SIZE + LENGTH_SIZE;

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
        /// </summary>
        public static BaseMessage Unpack(byte[] data)
        {
            if (data == null || data.Length < HEADER_SIZE)
            {
                throw new PacketException("Dữ liệu không đủ để đọc Header (cần tối thiểu " + HEADER_SIZE + " byte).");
            }

            MessageType type = (MessageType)BitConverter.ToInt32(data, 0);
            int bodyLength = BitConverter.ToInt32(data, TYPE_SIZE);

            if (bodyLength < 0 || data.Length < HEADER_SIZE + bodyLength)
            {
                throw new PacketException("Dữ liệu không đủ để đọc Body (cần " + bodyLength + " byte).");
            }

            string json = Encoding.UTF8.GetString(data, HEADER_SIZE, bodyLength);

            try
            {
                return JsonSerializer.Deserialize(json, type);
            }
            catch (Exception ex)
            {
                throw new PacketException("Không thể giải mã message loại " + type + ": " + ex.Message, ex);
            }
        }
    }

    /// <summary>
    /// Exception riêng cho lỗi đóng gói/giải mã packet.
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
