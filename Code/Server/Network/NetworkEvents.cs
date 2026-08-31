using System;

namespace Server.Network
{
    public static class NetworkEvents
    {
        public static event Action<ClientSession, int>? OnPacketSent;
        public static event Action<ClientSession, int>? OnPacketReceived;
        public static event Action<ClientSession, Exception>? OnPacketError;
        public static event Action<ClientSession>? OnClientDisconnected;

        public static void RaisePacketSent(ClientSession session, int byteCount)
        {
            OnPacketSent?.Invoke(session, byteCount);
        }

        public static void RaisePacketReceived(ClientSession session, int byteCount)
        {
            OnPacketReceived?.Invoke(session, byteCount);
        }

        public static void RaisePacketError(ClientSession session, Exception ex)
        {
            OnPacketError?.Invoke(session, ex);
        }

        public static void RaiseClientDisconnected(ClientSession session)
        {
            OnClientDisconnected?.Invoke(session);
        }
    }
}
