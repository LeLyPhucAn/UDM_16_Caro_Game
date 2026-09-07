using System;
using System.Threading;
using Server.Utils;

namespace Server.Network
{
    public class NetworkDiagnostics
    {
        private long _bytesSent;
        private long _bytesReceived;
        private long _errorCount;

        private readonly Timer _timer;

        public NetworkDiagnostics()
        {
            NetworkEvents.OnPacketSent += HandlePacketSent;
            NetworkEvents.OnPacketReceived += HandlePacketReceived;
            NetworkEvents.OnPacketError += HandlePacketError;

            // In log mỗi 10 giây
            _timer = new Timer(PrintDiagnostics, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
        }

        private void HandlePacketSent(ClientSession session, int byteCount)
        {
            Interlocked.Add(ref _bytesSent, byteCount);
        }

        private void HandlePacketReceived(ClientSession session, int byteCount)
        {
            Interlocked.Add(ref _bytesReceived, byteCount);
        }

        private void HandlePacketError(ClientSession session, Exception ex)
        {
            Interlocked.Increment(ref _errorCount);
        }

        private void PrintDiagnostics(object? state)
        {
            long sent = Interlocked.Exchange(ref _bytesSent, 0);
            long received = Interlocked.Exchange(ref _bytesReceived, 0);
            long errors = Interlocked.Exchange(ref _errorCount, 0);

            // Log ra Console
            Logger.Info($"[Diagnostics] 10s passed - Sent: {sent} bytes, Received: {received} bytes, Errors: {errors}");
        }
    }
}
