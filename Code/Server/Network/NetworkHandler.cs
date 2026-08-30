using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using CaroGame.Protocol;
using CaroGame.Protocol.Messages;

namespace Server.Network;

public class NetworkHandler
{
    /// <summary>
    /// Đọc chính xác đủ count bytes từ NetworkStream.
    /// Trả về false nếu stream bị ngắt kết nối giữa chừng.
    /// </summary>
    public static async Task<bool> ReadExactAsync(NetworkStream stream, byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
    {
        int totalRead = 0;
        while (totalRead < count)
        {
            int read = await stream.ReadAsync(buffer.AsMemory(offset + totalRead, count - totalRead), cancellationToken);
            if (read == 0)
            {
                return false; // Client ngắt kết nối
            }
            totalRead += read;
        }
        return true;
    }

    /// <summary>
    /// Vòng lặp nhận dữ liệu (Message) bất đồng bộ từ Client.
    /// Tự động bắt Exception ngắt kết nối để Server không bị crash.
    /// </summary>
    public static async Task ListenForMessagesAsync(
        ClientSession session,
        Func<ClientSession, BaseMessage, Task> onMessageReceived,
        Action<ClientSession> onDisconnected,
        CancellationToken cancellationToken = default)
    {
        byte[] headerBuffer = new byte[8]; // [4 bytes Type] [4 bytes Body Length]

        try
        {
            while (session.IsConnected && !cancellationToken.IsCancellationRequested)
            {
                // 1. Đọc 8 bytes Header
                bool success = await ReadExactAsync(session.Stream, headerBuffer, 0, 8, cancellationToken);
                if (!success)
                {
                    break; // Client ngắt kết nối bình thường
                }

                MessageType type = (MessageType)BitConverter.ToInt32(headerBuffer, 0);
                int bodyLength = BitConverter.ToInt32(headerBuffer, 4);

                if (bodyLength < 0 || bodyLength > 10 * 1024 * 1024) // Giới hạn 10MB
                {
                    Console.WriteLine($"[NetworkError] Kích thước gói tin không hợp lệ ({bodyLength} bytes) từ {session.RemoteEndPoint}");
                    break;
                }

                // 2. Đọc Body
                byte[] bodyBuffer = new byte[bodyLength];
                if (bodyLength > 0)
                {
                    success = await ReadExactAsync(session.Stream, bodyBuffer, 0, bodyLength, cancellationToken);
                    if (!success)
                    {
                        break;
                    }
                }

                // 3. Ghép Header + Body để Unpack thành BaseMessage
                byte[] fullPacket = new byte[8 + bodyLength];
                Buffer.BlockCopy(headerBuffer, 0, fullPacket, 0, 8);
                if (bodyLength > 0)
                {
                    Buffer.BlockCopy(bodyBuffer, 0, fullPacket, 8, bodyLength);
                }

                BaseMessage message = Packet.Unpack(fullPacket);

                // 4. Gọi Callback xử lý Message
                if (onMessageReceived != null)
                {
                    await onMessageReceived(session, message);
                }
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"[Network] Client {session.RemoteEndPoint} ngắt socket: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"[Network] Client {session.RemoteEndPoint} lỗi I/O: {ex.Message}");
        }
        catch (ObjectDisposedException)
        {
            // Stream/Client đã đóng
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NetworkError] Ngoại lệ khi nhận dữ liệu từ {session.RemoteEndPoint}: {ex.Message}");
        }
        finally
        {
            session.Close();
            onDisconnected?.Invoke(session);
        }
    }

    /// <summary>
    /// Gửi một BaseMessage tới Client.
    /// </summary>
    public static async Task<bool> SendAsync(ClientSession session, BaseMessage message)
    {
        if (!session.IsConnected) return false;

        try
        {
            byte[] packetBytes = Packet.Pack(message);
            await session.Stream.WriteAsync(packetBytes.AsMemory());
            await session.Stream.FlushAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NetworkError] Lỗi khi gửi dữ liệu tới {session.RemoteEndPoint}: {ex.Message}");
            session.Close();
            return false;
        }
    }
}
