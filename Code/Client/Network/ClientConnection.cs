using System;
using System.Threading.Tasks;
using CaroGame.Protocol;

namespace Client.Network
{
    public class ClientConnection
    {
        private readonly TcpClientService _networkService;

        // Sự kiện gửi Message chuẩn lên tầng UI Form
        public event Action<BaseMessage>? OnMessageReceived;
        public event Action? OnConnectionLost;
        public event Action<Exception>? OnError;

        public bool IsConnected => _networkService.IsConnected;

        public ClientConnection()
        {
            _networkService = new TcpClientService();

            _networkService.OnMessageReceived += msg => OnMessageReceived?.Invoke(msg);
            _networkService.OnDisconnected += () => OnConnectionLost?.Invoke();
            _networkService.OnError += ex => OnError?.Invoke(ex);
        }

        public async Task ConnectToServer(string ip, int port)
        {
            await _networkService.ConnectAsync(ip, port);
        }

        public void Disconnect()
        {
            _networkService.Disconnect();
        }

        public async Task SendMessageAsync(BaseMessage message)
        {
            await _networkService.SendAsync(message);
        }
    }
}
