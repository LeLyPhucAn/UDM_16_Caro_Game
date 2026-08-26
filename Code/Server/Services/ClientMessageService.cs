using System;
using System.Text.Json;

namespace Client.Services
{
    public class ClientMessageService
    {
        public void ProcessMessage(string jsonPayload)
        {
            try
            {
                // TODO: Chuyển đổi chuỗi JSON thành Object (Deserialize)
                // Chú ý: Bạn cần thay 'YourPacketModel' bằng class Packet mà nhóm quy định
                // var response = JsonSerializer.Deserialize<YourPacketModel>(jsonPayload);

                // Giả lập logic kiểm tra Response
                /*
                if (response.IsError)
                {
                    // Xử lý Error Response
                    Console.WriteLine($"Server Error: {response.Message}");
                }
                else
                {
                    // Xử lý Response hợp lệ
                    Console.WriteLine($"Success: {response.Data}");
                }
                */

                // Tạm thời in ra màn hình để test
                Console.WriteLine($"[ClientMessageService] Đã xử lý gói tin: {jsonPayload}");
            }
            catch (JsonException ex)
            {
                // Xử lý dữ liệu nhận không hợp lệ (Tránh làm ứng dụng crash)
                Console.WriteLine($"[Lỗi Deserialize] Dữ liệu rác hoặc sai định dạng: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi không xác định]: {ex.Message}");
            }
        }
    }
}