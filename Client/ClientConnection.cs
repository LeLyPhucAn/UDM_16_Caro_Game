using System;
using System.Text.Json; // Bổ sung thư viện này để dùng Serialize
using System.Threading.Tasks;
using Client.Network;
using Client.Services; // Thêm namespace của ClientMessageService

namespace Client
{
    public class ClientConnection
    {
        private TcpClientService _networkService;
        private ClientMessageService _messageService; // Khai báo thêm MessageService

        // Các event để giao tiếp với tầng UI / Game Logic
        // Bạn có thể đổi Action<string> thành Action<YourPacketModel> sau này
        public event Action<string> OnMessageReceived;
        public event Action OnConnectionLost;

        public ClientConnection()
        {
            _networkService = new TcpClientService();
            _messageService = new ClientMessageService(); // Khởi tạo MessageService

            // Lắng nghe các sự kiện từ tầng Network (TcpClientService)
            _networkService.OnDataReceived += HandleDataReceived;
            _networkService.OnDisconnected += HandleDisconnected;
            _networkService.OnError += HandleError;
        }

        public async Task ConnectToServer(string ip, int port)
        {
            await _networkService.ConnectAsync(ip, port);
        }

        public void Disconnect()
        {
            _networkService.Disconnect();
        }

        // SỬA HÀM NÀY: Đổi tham số từ 'string' sang 'object' để nhận mọi loại Message từ UI
        public async Task SendMessage(object messageObj)
        {
            // Yêu cầu: Serialize Message
            string jsonPayload = JsonSerializer.Serialize(messageObj);

            // Yêu cầu: Gửi Message qua TCP
            await _networkService.SendDataAsync(jsonPayload);
        }

        // SỬA HÀM NÀY: Xử lý dữ liệu nhận được trước khi đẩy lên UI
        private void HandleDataReceived(string data)
        {
            // Yêu cầu: Chuyển Message đến tầng xử lý phù hợp
            _messageService.ProcessMessage(data);

            // Vẫn giữ event để UI có thể cập nhật nếu cần
            OnMessageReceived?.Invoke(data);
        }

        private void HandleDisconnected()
        {
            OnConnectionLost?.Invoke();
        }

        private void HandleError(Exception ex)
        {
            Console.WriteLine($"[ClientConnection Error]: {ex.Message}");
        }
    }
}