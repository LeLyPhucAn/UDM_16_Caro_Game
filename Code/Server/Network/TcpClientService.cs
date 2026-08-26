using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Network
{
    public class TcpClientService
    {
        // Sử dụng hằng số để tránh hard-code giá trị cấu hình
        private const int ReceiveBufferSize = 4096;

        private TcpClient _client;
        private NetworkStream _stream;
        private CancellationTokenSource _cancellationTokenSource;

        public bool IsConnected => _client != null && _client.Connected;

        public event Action<string> OnDataReceived;
        public event Action OnDisconnected;
        public event Action<Exception> OnError;

        public async Task ConnectAsync(string host, int port)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(host, port);
                _stream = _client.GetStream();
                _cancellationTokenSource = new CancellationTokenSource();

                // Khởi chạy vòng lặp nhận dữ liệu ngầm sau khi kết nối thành công
                _ = ReceiveDataAsync(_cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                HandleError(ex);
                throw;
            }
        }

        public void Disconnect()
        {
            if (!IsConnected) return;

            try
            {
                // Hủy token để dừng vòng lặp nhận dữ liệu bất đồng bộ một cách an toàn
                _cancellationTokenSource?.Cancel();
                _stream?.Close();
                _client?.Close();
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
            finally
            {
                OnDisconnected?.Invoke();
            }
        }

        public async Task SendDataAsync(string data)
        {
            if (!IsConnected) return;

            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                await _stream.WriteAsync(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                HandleError(ex);
                Disconnect();
            }
        }

        private async Task ReceiveDataAsync(CancellationToken token)
        {
            byte[] buffer = new byte[ReceiveBufferSize];

            try
            {
                // Lắng nghe dữ liệu liên tục cho đến khi bị hủy hoặc mất kết nối
                while (IsConnected && !token.IsCancellationRequested)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, token);

                    if (bytesRead > 0)
                    {
                        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        OnDataReceived?.Invoke(data);
                    }
                    else
                    {
                        // bytesRead = 0 báo hiệu server đã chủ động ngắt kết nối
                        Disconnect();
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Bỏ qua ngoại lệ này vì nó sinh ra khi người dùng chủ động gọi Disconnect
            }
            catch (Exception ex)
            {
                HandleError(ex);
                Disconnect();
            }
        }

        private void HandleError(Exception ex)
        {
            OnError?.Invoke(ex);
        }
    }
}