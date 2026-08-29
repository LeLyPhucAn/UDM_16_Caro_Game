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

        // Lưu trữ Context của UI Thread để đồng bộ hóa
        private readonly SynchronizationContext _uiContext;

        public ConnectionStateManager()
        {
            // Yêu cầu: Khởi tạo class này trên Main Thread (Form_Load hoặc Constructor của Form)
            _uiContext = SynchronizationContext.Current;
        }

        public void ChangeState(ConnectionState newState)
        {
            if (CurrentState == newState) return;

            CurrentState = newState;

            // Đảm bảo Event được bắn ra trên đúng UI Thread
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