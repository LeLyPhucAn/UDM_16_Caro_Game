using System;
using System.Threading.Tasks;

namespace Client.Network
{
    public class ClientConnection
    {
        private readonly TcpClientService _networkService;

        // Các event để giao tiếp với tầng UI / Game Logic
        public event Action<string>? OnMessageReceived;
        public event Action? OnConnectionLost;
        public event Action<Exception>? OnError;

        public bool IsConnected => _networkService.IsConnected;

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
            OnMessageReceived?.Invoke(data);
        }

        private void HandleDisconnected()
        {
            OnConnectionLost?.Invoke();
        }

        private void HandleError(Exception ex)
        {
            Console.WriteLine($"[ClientConnection Error]: {ex.Message}");
            OnError?.Invoke(ex);
        }
    }
}
