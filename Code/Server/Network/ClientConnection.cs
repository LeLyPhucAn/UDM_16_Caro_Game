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
        public event Action OnConnectionLost;

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
                StateManager.ChangeState(ConnectionState.Connecting);
                await _networkService.ConnectAsync(ip, port);
                StateManager.ChangeState(ConnectionState.Connected);
            }
            catch (Exception ex)
            {
                StateManager.ChangeState(ConnectionState.ConnectionFailed);
                Console.WriteLine($"[Connect Failed]: {ex.Message}");
            }
        }

        public void Disconnect()
        {
            _networkService.Disconnect();
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
            if (StateManager.CurrentState != ConnectionState.Disconnected)
            {
                StateManager.ChangeState(ConnectionState.ServerOffline);
            }
            OnConnectionLost?.Invoke();
        }

        private void HandleError(Exception ex)
        {
            Console.WriteLine($"[ClientConnection Error]: {ex.Message}");
        }
    }
}