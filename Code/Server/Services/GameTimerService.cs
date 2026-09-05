using System;
using System.Collections.Concurrent;

namespace Server.Services
{
    /// <summary>
    /// Quản lý Timer cho từng Match.
    ///
    /// Mỗi lượt chơi có một khoảng thời gian giới hạn.
    /// Mặc định: 30 giây.
    /// </summary>
    public sealed class GameTimerService : IDisposable
    {
        /// <summary>
        /// Thông tin Timer của một Match.
        /// </summary>
        private sealed class TimerEntry
        {
            public Timer Timer { get; }

            public DateTime ExpiresAt { get; }

            public TimerEntry(
                Timer timer,
                DateTime expiresAt)
            {
                Timer = timer;

                ExpiresAt = expiresAt;
            }
        }

        /// <summary>
        /// Danh sách Timer đang chạy.
        ///
        /// Key = MatchId.
        /// </summary>
        private readonly ConcurrentDictionary<
            string,
            TimerEntry> timers;

        /// <summary>
        /// Thời gian của mỗi lượt.
        /// </summary>
        private readonly TimeSpan turnDuration;

        /// <summary>
        /// Callback được gọi khi hết thời gian.
        /// </summary>
        private readonly Action<string> timeoutCallback;

        private bool disposed;

        /// <summary>
        /// Lấy thời gian của một lượt.
        /// </summary>
        public TimeSpan TurnDuration
        {
            get
            {
                return turnDuration;
            }
        }

        /// <summary>
        /// Constructor.
        ///
        /// Nếu không truyền thời gian:
        /// mặc định 30 giây.
        /// </summary>
        public GameTimerService(
            TimeSpan? turnDuration = null,
            Action<string>? timeoutCallback = null)
        {
            timers =
                new ConcurrentDictionary<
                    string,
                    TimerEntry>();

            this.turnDuration =
                turnDuration ??
                TimeSpan.FromSeconds(30);

            if (this.turnDuration <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(turnDuration),
                    "Turn duration must be greater than zero.");
            }

            this.timeoutCallback =
                timeoutCallback ??
                (_ => { });
        }

        /// <summary>
        /// Bắt đầu Timer hoặc Reset Timer của Match.
        ///
        /// Dùng khi:
        /// - Match bắt đầu.
        /// - Người chơi đánh xong.
        /// - Chuyển sang lượt tiếp theo.
        /// </summary>
        public bool StartOrReset(
            string matchId)
        {
            if (disposed)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(matchId))
            {
                return false;
            }

            // Xóa Timer cũ.
            Stop(matchId);

            DateTime expiresAt =
                DateTime.UtcNow.Add(
                    turnDuration);

            Timer? timer = null;

            timer =
                new Timer(
                    _ =>
                        OnTimeout(
                            matchId,
                            timer!),
                    null,
                    turnDuration,
                    Timeout.InfiniteTimeSpan);

            TimerEntry entry =
                new TimerEntry(
                    timer,
                    expiresAt);

            if (!timers.TryAdd(
                    matchId,
                    entry))
            {
                timer.Dispose();

                return false;
            }

            return true;
        }

        /// <summary>
        /// Dừng Timer của Match.
        /// </summary>
        public bool Stop(
            string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
            {
                return false;
            }

            if (timers.TryRemove(
                    matchId,
                    out TimerEntry? entry))
            {
                entry.Timer.Dispose();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Lấy số giây còn lại.
        /// </summary>
        public int GetRemainingSeconds(
            string matchId)
        {
            if (!timers.TryGetValue(
                    matchId,
                    out TimerEntry? entry))
            {
                return 0;
            }

            TimeSpan remaining =
                entry.ExpiresAt -
                DateTime.UtcNow;

            if (remaining <= TimeSpan.Zero)
            {
                return 0;
            }

            return (int)Math.Ceiling(
                remaining.TotalSeconds);
        }

        /// <summary>
        /// Kiểm tra Timer có đang chạy hay không.
        /// </summary>
        public bool IsRunning(
            string matchId)
        {
            if (string.IsNullOrWhiteSpace(
                    matchId))
            {
                return false;
            }

            return timers.ContainsKey(matchId);
        }

        /// <summary>
        /// Xử lý khi Timer hết hạn.
        /// </summary>
        private void OnTimeout(
            string matchId,
            Timer timer)
        {
            if (!timers.TryGetValue(
                    matchId,
                    out TimerEntry? current))
            {
                timer.Dispose();

                return;
            }

            // Đảm bảo Timer cũ không xử lý timeout
            // sau khi Timer mới đã được reset.
            if (!ReferenceEquals(
                    current.Timer,
                    timer))
            {
                timer.Dispose();

                return;
            }

            if (!timers.TryRemove(
                    matchId,
                    out TimerEntry? entry))
            {
                timer.Dispose();

                return;
            }

            entry.Timer.Dispose();

            if (!disposed)
            {
                timeoutCallback(matchId);
            }
        }

        /// <summary>
        /// Giải phóng toàn bộ Timer.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            foreach (
                KeyValuePair<
                    string,
                    TimerEntry> item
                in timers)
            {
                item.Value.Timer.Dispose();
            }

            timers.Clear();
        }
    }
}