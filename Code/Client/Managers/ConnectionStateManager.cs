using System;

namespace Client.Managers
{
    // Định nghĩa các trạng thái kết quả cần đạt
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
        private ConnectionState _currentState = ConnectionState.Disconnected;

        // Event để UI đăng ký lắng nghe trạng thái thay đổi
        public event Action<ConnectionState> OnStateChanged;

        public ConnectionState CurrentState
        {
            get => _currentState;
            private set
            {
                if (_currentState != value)
                {
                    _currentState = value;
                    OnStateChanged?.Invoke(_currentState); // Thông báo trạng thái cho UI
                }
            }
        }

        public void ChangeState(ConnectionState newState)
        {
            CurrentState = newState;
            Console.WriteLine($"[State Updated]: {newState}");
        }
    }
}