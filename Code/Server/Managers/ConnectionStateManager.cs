using System;
using System.Threading;

namespace Client.Managers
{
    public enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected,
        ConnectionFailed,
        ServerOffline
    }

    public class ConnectionStateManager
    {
        public ConnectionState CurrentState { get; private set; } = ConnectionState.Disconnected;

        public event Action<ConnectionState> OnStateChanged;

        private readonly SynchronizationContext _uiContext;

        public ConnectionStateManager()
        {
            _uiContext = SynchronizationContext.Current;
        }

        public void ChangeState(ConnectionState newState)
        {
            if (CurrentState == newState) return;

            CurrentState = newState;

            if (_uiContext != null)
            {
                _uiContext.Post(_ => OnStateChanged?.Invoke(CurrentState), null);
            }
            else
            {
                OnStateChanged?.Invoke(CurrentState);
            }
        }
    }
}