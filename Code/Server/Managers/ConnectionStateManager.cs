using System;
using System.Linq;
using System.Threading;
using CaroGame.Protocol.Messages.System;
using Server.Network;
using Server.Utils;

namespace Server.Managers
{
    public class ConnectionStateManager
    {
        private readonly ConnectionManager _connectionManager;
        private readonly Timer _timer;
        private readonly int _pingIntervalSeconds = 5;
        private readonly int _timeoutSeconds = 15;

        public ConnectionStateManager(ConnectionManager connectionManager)
        {
            _connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));
            
            // Chạy kiểm tra mỗi 5 giây
            _timer = new Timer(CheckHeartbeat, null, TimeSpan.FromSeconds(_pingIntervalSeconds), TimeSpan.FromSeconds(_pingIntervalSeconds));
        }

        private async void CheckHeartbeat(object? state)
        {
            var clients = _connectionManager.GetAll().ToList();
            var now = DateTime.Now;

            foreach (var client in clients)
            {
                // Kiểm tra timeout
                if ((now - client.LastPongTime).TotalSeconds >= _timeoutSeconds)
                {
                    Logger.Warn($"[ConnectionState] Client {client.SessionId} bị timeout (không phản hồi Pong quá {_timeoutSeconds}s). Đang ngắt kết nối...");
                    NetworkEvents.RaiseClientDisconnected(client);
                    continue;
                }

                // Gửi Ping
                if ((now - client.LastPingTime).TotalSeconds >= _pingIntervalSeconds)
                {
                    try
                    {
                        await client.SendAsync(new PingMessage());
                        client.LastPingTime = now;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"[ConnectionState] Lỗi khi gửi Ping tới {client.SessionId}", ex);
                    }
                }
            }
        }
    }
}
