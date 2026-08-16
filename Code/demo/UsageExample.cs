using System;

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
            byte[] packet = Packet.Pack(login);

            // 3. Server nhận byte[] và giải mã ngược lại
            BaseMessage received = Packet.Unpack(packet);

            if (received is LoginMessage loginReceived)
            {
                Console.WriteLine($"Server nhận đăng nhập từ: {loginReceived.Username}");
            }

            // 4. Server phản hồi lại cho client
            ResponseMessage response = new ResponseMessage
            {
                SenderId = "server",
                Success = true,
                Data = "Đăng nhập thành công"
            };

            byte[] responsePacket = Packet.Pack(response);
            BaseMessage receivedResponse = Packet.Unpack(responsePacket);

            if (receivedResponse is ResponseMessage responseReceived)
            {
                Console.WriteLine($"Client nhận phản hồi: {responseReceived.Data}");
            }
        }
    }
}
