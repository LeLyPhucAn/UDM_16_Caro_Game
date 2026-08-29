using System;
using System.Threading.Tasks;
using Client.Managers;

namespace Client.Network
{
    public class ClientConnection
    {
        private TcpClientService _networkService;
        public ConnectionStateManager StateManager { get; private set; }

        public event Action<string> OnMessageReceived;

        // Sự kiện báo UI khi Server Disconnect
        public event Action OnServerDisconnected;

        public ClientConnection()
        {
            _networkService = new TcpClientService();
            StateManager = new ConnectionStateManager();

            _networkService.OnDataReceived += HandleDataReceived;
            _networkService.OnDisconnected += HandleDisconnected;
            _networkService.OnError += HandleError;
        }

        public async Task ConnectToServer(string ip, int port)
        {
            try
            {
                // Quản lý Connecting
                StateManager.ChangeState(ConnectionState.Connecting);

                await _networkService.ConnectAsync(ip, port);

                // Quản lý Connected
                StateManager.ChangeState(ConnectionState.Connected);
            }
            catch (Exception)
            {
                // Xử lý Connection Failed
                StateManager.ChangeState(ConnectionState.ConnectionFailed);
            }
        }

        public void Disconnect()
        {
            _networkService.Disconnect();

            // Quản lý Disconnected (Client chủ động)
            StateManager.ChangeState(ConnectionState.Disconnected);
        }

        public async Task SendMessage(string message)
        {
            await _networkService.SendDataAsync(message);
        }

        private void HandleDataReceived(string data)
        {
            // Phát Event khi nhận Message
            OnMessageReceived?.Invoke(data);
        }

        private void HandleDisconnected()
        {
            // Xử lý Server Offline (Mất mạng đột ngột hoặc Server sập)
            if (StateManager.CurrentState != ConnectionState.Disconnected)
            {
                StateManager.ChangeState(ConnectionState.ServerOffline);
                OnServerDisconnected?.Invoke();
            }
        }

        private void HandleError(Exception ex)
        {
            // Bắt log Send/Receive Error ngầm, không báo ra giao diện gây hoang mang
            Console.WriteLine($"[Network Error]: {ex.Message}");
        }
    }
}