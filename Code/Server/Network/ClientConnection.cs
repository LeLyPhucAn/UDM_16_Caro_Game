using System;
using System.Threading.Tasks;
using Client.Network;
using Client.Managers; // Thêm dòng này để gọi thư mục Managers

namespace Client.Network

{
    public class ClientConnection
    {
        private TcpClientService _networkService;

        // Thêm StateManager
        public ConnectionStateManager StateManager { get; private set; }

        public event Action OnMessageReceived;
        public event Action OnConnectionLost;

        public ClientConnection()
        {
            _networkService = new TcpClientService();
            StateManager = new ConnectionStateManager(); // Khởi tạo Manager

            _networkService.OnDataReceived += HandleDataReceived;
            _networkService.OnDisconnected += HandleDisconnected;
            _networkService.OnError += HandleError;
        }

        public async Task ConnectToServer(string ip, int port)
        {
            try
            {
                // 1. Xử lý Connecting
                StateManager.ChangeState(ConnectionState.Connecting);

                await _networkService.ConnectAsync(ip, port);

                // 2. Xử lý Connected
                StateManager.ChangeState(ConnectionState.Connected);
            }
            catch (Exception ex)
            {
                // 3. Xử lý Connection Failed (Lỗi ngay khi cố gắng kết nối)
                StateManager.ChangeState(ConnectionState.ConnectionFailed);
                Console.WriteLine($"[Connect Failed]: {ex.Message}");
            }
        }

        public void Disconnect()
        {
            // Cho phép Client thực hiện Disconnect đúng cách
            _networkService.Disconnect();

            // Xử lý Disconnected (Chủ động ngắt)
            StateManager.ChangeState(ConnectionState.Disconnected);
        }

        public async Task SendMessage(string message)
        {
            await _networkService.SendDataAsync(message);
        }

        private void HandleDataReceived(string data)
        {
            OnMessageReceived?.Invoke(data);
        }

        private void HandleDisconnected()
        {
            // Bị ngắt kết nối thụ động (Server sập hoặc mất mạng)
            // 4. Phát hiện Server Offline
            if (StateManager.CurrentState != ConnectionState.Disconnected)
            {
                StateManager.ChangeState(ConnectionState.ServerOffline);
            }

            OnConnectionLost?.Invoke();
        }

        private void HandleError(Exception ex)
        {
            // Xử lý Send/Receive Error
            Console.WriteLine($"[ClientConnection Error]: {ex.Message}");
        }
    }
}