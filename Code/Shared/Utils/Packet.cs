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
    public static class Packet
    {
        public static byte[] Pack(BaseMessage message)
        {
            string json = JsonSerializer.Serialize(message);
            byte[] typeBytes = BitConverter.GetBytes((int)message.Type);
            byte[] bodyBytes = Encoding.UTF8.GetBytes(json);
            byte[] lengthBytes = BitConverter.GetBytes(bodyBytes.Length);

            byte[] packet = new byte[typeBytes.Length + lengthBytes.Length + bodyBytes.Length];
            Buffer.BlockCopy(typeBytes, 0, packet, 0, typeBytes.Length);
            Buffer.BlockCopy(lengthBytes, 0, packet, typeBytes.Length, lengthBytes.Length);
            Buffer.BlockCopy(bodyBytes, 0, packet, typeBytes.Length + lengthBytes.Length, bodyBytes.Length);

            return packet;
        }

        public static BaseMessage Unpack(byte[] data)
        {
            MessageType type = (MessageType)BitConverter.ToInt32(data, 0);
            int length = BitConverter.ToInt32(data, 4);
            string json = Encoding.UTF8.GetString(data, 8, length);

            return JsonSerializer.Deserialize(json, type);
        }
    }
}
