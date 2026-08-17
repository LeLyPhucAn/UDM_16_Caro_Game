using System;
using System.Threading.Tasks;
using Client.Network;

namespace Client
{
    public class ClientConnection
    {
        private TcpClientService _networkService;

        // Các event để giao tiếp với tầng UI / Game Logic
        public event Action OnMessageReceived;
        public event Action OnConnectionLost;

        public ClientConnection()
        {
            _networkService = new TcpClientService();

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

        public async Task SendMessage(string message)
        {
            await _networkService.SendDataAsync(message);
        }

        // Xử lý dữ liệu nhận được trước khi đẩy lên UI
        private void HandleDataReceived(string data)
        {
            // Sau này bạn có thể thêm logic giải mã JSON hoặc chia gói tin ở đây
            OnMessageReceived?.Invoke(data);
        }

        private void HandleDisconnected()
        {
            OnConnectionLost?.Invoke();
        }

        private void HandleError(Exception ex)
        {
            // In log lỗi ra console hoặc xử lý tùy logic game
            Console.WriteLine($"[ClientConnection Error]: {ex.Message}");
        }
    }
}