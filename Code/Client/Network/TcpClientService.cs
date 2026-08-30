using CaroGame.Protocol.Messages;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using CaroGame.Protocol;
using CaroGame.Protocol.Network;

namespace Client.Network
{
    public class TcpClientService
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private CancellationTokenSource? _cancellationTokenSource;

        public bool IsConnected => _client != null && _client.Connected;

        // Sự kiện bắn ra khi nhận được một Message hoàn chỉnh từ Server
        public event Action<BaseMessage>? OnMessageReceived;
        public event Action? OnDisconnected;
        public event Action<Exception>? OnError;

        public async Task ConnectAsync(string host, int port)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(host, port);
                _stream = _client.GetStream();
                _cancellationTokenSource = new CancellationTokenSource();

                // Khởi chạy vòng lặp nhận gói tin ngầm
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

        /// <summary>
        /// Gửi gói tin chuẩn định dạng (Header 8 byte + Body JSON)
        /// </summary>
        public async Task SendAsync(BaseMessage message)
        {
            if (!IsConnected || _stream == null) return;

            try
            {
                byte[] packet = PacketParser.Pack(message);
                await _stream.WriteAsync(packet.AsMemory());
                await _stream.FlushAsync();
            }
            catch (Exception ex)
            {
                HandleError(ex);
                Disconnect();
            }
        }

        /// <summary>
        /// Đọc chính xác đủ số lượng byte từ Stream
        /// </summary>
        private static async Task<bool> ReadExactAsync(NetworkStream stream, byte[] buffer, int count, CancellationToken token)
        {
            int totalRead = 0;
            while (totalRead < count)
            {
                int read = await stream.ReadAsync(buffer.AsMemory(totalRead, count - totalRead), token);
                if (read == 0) return false;
                totalRead += read;
            }
            return true;
        }

        private async Task ReceiveDataAsync(CancellationToken token)
        {
            byte[] headerBuffer = new byte[8];

            try
            {
                while (IsConnected && !token.IsCancellationRequested && _stream != null)
                {
                    // 1. Đọc 8 bytes Header
                    bool readHeaderSuccess = await ReadExactAsync(_stream, headerBuffer, 8, token);
                    if (!readHeaderSuccess) break;

                    int bodyLength = BitConverter.ToInt32(headerBuffer, 4);

                    // 2. Đọc Body
                    byte[] bodyBuffer = new byte[bodyLength];
                    if (bodyLength > 0)
                    {
                        bool readBodySuccess = await ReadExactAsync(_stream, bodyBuffer, bodyLength, token);
                        if (!readBodySuccess) break;
                    }

                    // 3. Ghép Header + Body để Unpack thành BaseMessage
                    byte[] messageBytes = new byte[8 + bodyLength];
                    Buffer.BlockCopy(headerBuffer, 0, messageBytes, 0, 8);
                    if (bodyLength > 0)
                    {
                        Buffer.BlockCopy(bodyBuffer, 0, messageBytes, 8, bodyLength);
                    }

                    BaseMessage message = PacketParser.Unpack(messageBytes);
                    OnMessageReceived?.Invoke(message);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                HandleError(ex);
            }                                                                                                                                                                                                                                                                                       
            finally
            {
                Disconnect();
            }
        }

        private void HandleError(Exception ex)
        {
            OnError?.Invoke(ex);
        }
    }
}
