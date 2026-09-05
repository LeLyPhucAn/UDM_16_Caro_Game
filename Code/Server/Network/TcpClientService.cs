using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Network
{
    public class TcpClientService
    {
        private const int ReceiveBufferSize = 4096;
        private TcpClient _client;
        private NetworkStream _stream;
        private CancellationTokenSource _cancellationTokenSource;

        // Khóa luồng an toàn (Thread-safe) chống đụng độ khi gửi dữ liệu liên tục
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);
        private bool _isReceiving = false;

        public bool IsConnected => _client != null && _client.Connected;

        public event Action<string> OnDataReceived;
        public event Action OnDisconnected;
        public event Action<Exception> OnError;

        public async Task ConnectAsync(string host, int port, int timeoutMs = 5000)
        {
            try
            {
                _client = new TcpClient();

                // Xử lý Connection Timeout
                using (var timeoutCts = new CancellationTokenSource(timeoutMs))
                {
                    var connectTask = _client.ConnectAsync(host, port);
                    if (await Task.WhenAny(connectTask, Task.Delay(timeoutMs, timeoutCts.Token)) != connectTask)
                    {
                        throw new TimeoutException("Connection Timeout.");
                    }
                    await connectTask;
                }

                _stream = _client.GetStream();
                _cancellationTokenSource = new CancellationTokenSource();

                // Đảm bảo không tạo nhiều Receive Loop
                if (!_isReceiving)
                {
                    _isReceiving = true;
                    _ = ReceiveDataAsync(_cancellationTokenSource.Token);
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
                throw;
            }
        }

        public void Disconnect()
        {
            if (!IsConnected) return;

            try
            {
                _cancellationTokenSource?.Cancel();
                _isReceiving = false;
                _stream?.Close();
                _client?.Close();
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
            }
            finally
            {
                OnDisconnected?.Invoke();
            }
        }

        public async Task SendDataAsync(string data)
        {
            if (!IsConnected) return;

            await _sendLock.WaitAsync(); // Bắt đầu khóa Thread
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data + "\n");
                await _stream.WriteAsync(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
                Disconnect();
            }
            finally
            {
                _sendLock.Release(); // Mở khóa Thread
            }
        }

        private async Task ReceiveDataAsync(CancellationToken token)
        {
            byte[] buffer = new byte[ReceiveBufferSize];

            try
            {
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
                        Disconnect();
                        break;
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
                Disconnect();
            }
            finally
            {
                _isReceiving = false;
            }
        }
    }
}