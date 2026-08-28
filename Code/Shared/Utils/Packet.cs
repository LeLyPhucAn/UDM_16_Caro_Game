using CaroGame.Protocol.Messages;
using CaroGame.Protocol.Messages.Game;
using CaroGame.Protocol.Utils;
using System;
using System.Text;
// LƯU Ý: Không thêm 'using System.Text.Json;' ở trên cùng 
// để tránh đụng độ tên với class JsonSerializer tùy chỉnh của bạn.

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
            // SỬA QUAN TRỌNG 1: Ép Type đa hình (Polymorphism)
            // Dùng thư viện chuẩn của .NET và message.GetType() để đảm bảo
            // các biến của lớp con (Row, Col, Symbol) 100% không bị rớt mất khi chuyển thành JSON.
            string json = System.Text.Json.JsonSerializer.Serialize(message, message.GetType());

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

            // SỬA QUAN TRỌNG 2: Phân luồng dịch JSON
            switch (type)
            {
                // Tự tay dịch các gói tin Game Caro bằng thư viện chuẩn của C#
                case MessageType.Move:
                    return System.Text.Json.JsonSerializer.Deserialize<MoveMessage>(json)!;

                case MessageType.GameSync:
                    return System.Text.Json.JsonSerializer.Deserialize<GameSyncMessage>(json)!;

                case MessageType.GameOver:
                    return System.Text.Json.JsonSerializer.Deserialize<GameOverMessage>(json)!;

                // Mặc định (Login, Lobby, Response...): 
                // Trả về hàm Deserialize trong class tùy chỉnh của bạn để giữ nguyên tính ổn định cũ
                default:
                    return Utils.JsonSerializer.Deserialize(json, type);
            }
        }
    }
}
