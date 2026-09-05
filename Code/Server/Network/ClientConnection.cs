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
        public event Action OnServerDisconnected;

        // Lưu thông tin để Reconnect
        private string _lastIp;
        private int _lastPort;

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
                _lastIp = ip;
                _lastPort = port;
                StateManager.ChangeState(ConnectionState.Connecting);

                // Timeout 5 giây tránh treo UI
                await _networkService.ConnectAsync(ip, port, 5000);

                StateManager.ChangeState(ConnectionState.Connected);
            }
            catch (Exception)
            {
                StateManager.ChangeState(ConnectionState.ConnectionFailed);
            }
        }

        public async Task ReconnectToServer()
        {
            if (!string.IsNullOrEmpty(_lastIp) && _lastPort > 0)
            {
                await ConnectToServer(_lastIp, _lastPort);
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
                OnServerDisconnected?.Invoke();
            }
        }

        private void HandleError(Exception ex)
        {
            Console.WriteLine($"[Network Error]: {ex.Message}");
        }
    }
}