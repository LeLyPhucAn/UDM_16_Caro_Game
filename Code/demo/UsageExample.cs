using System;
using CaroGame.Protocol.Messages;
using CaroGame.Protocol.Network;

namespace CaroGame.Protocol
{
    public class UsageExample
    {
        public static void Run()
        {
            // 1. Client tạo message đăng nhập
            LoginMessage login = new LoginMessage
            {
                SenderId = "player_001",
                Username = "vankhuyen",
                Password = "123456"
            };

            // 2. Đóng gói thành byte[] để gửi qua socket
            byte[] packet = PacketParser.Pack(login);

            // 3. Server nhận byte[] và giải mã ngược lại
            BaseMessage received = PacketParser.Unpack(packet);

            if (received is LoginMessage loginReceived)
            {
                Console.WriteLine("Server nhận đăng nhập từ: " + loginReceived.Username);
            }

            // 4. Server phản hồi lại cho client
            ResponseMessage response = new ResponseMessage
            {
                SenderId = "server",
                Success = true,
                Data = "Đăng nhập thành công"
            };

            byte[] responsePacket = PacketParser.Pack(response);
            BaseMessage receivedResponse = PacketParser.Unpack(responsePacket);

            if (receivedResponse is ResponseMessage responseReceived)
            {
                Console.WriteLine("Client nhận phản hồi: " + responseReceived.Data);
            }

            // 5. Ví dụ server gửi ErrorMessage khi có lỗi ở tầng giao thức
            ErrorMessage error = new ErrorMessage
            {
                SenderId = "server",
                ErrorCode = "UNKNOWN_TYPE",
                Description = "Không hỗ trợ loại message này"
            };

            byte[] errorPacket = PacketParser.Pack(error);
            BaseMessage receivedError = PacketParser.Unpack(errorPacket);

            if (receivedError is ErrorMessage errorReceived)
            {
                Console.WriteLine("Client nhận lỗi [" + errorReceived.ErrorCode + "]: " + errorReceived.Description);
            }

            // 6. Ví dụ Packet Validation: dữ liệu bị thiếu byte (giả lập lỗi khi truyền qua mạng)
            byte[] corruptedPacket = new byte[5]; // ít hơn 8 byte Header cần thiết
            try
            {
                PacketParser.Unpack(corruptedPacket);
            }
            catch (PacketException ex)
            {
                Console.WriteLine("Packet không hợp lệ: " + ex.Message);
            }
        }
    }
}
