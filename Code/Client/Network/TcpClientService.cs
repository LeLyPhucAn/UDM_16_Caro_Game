using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Client.Network
{
    public class TcpClientService
    {
        private TcpClient? _client;
        private NetworkStream? _stream;

        public bool IsConnected => _client?.Connected ?? false;

        public async Task<bool> ConnectAsync(string ip, int port)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(ip, port);
                _stream = _client.GetStream();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kết nối thất bại: {ex.Message}");
                return false;
            }
        }

        public void Disconnect()
        {
            _stream?.Close();
            _client?.Close();
            _stream = null;
            _client = null;
        }
    }
}
