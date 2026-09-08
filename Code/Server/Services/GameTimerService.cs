using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Server.Services
{
    /// <summary>
    /// Quản lý Timer cho từng Match.
    ///
    /// Mặc định: 30 giây cho mỗi lượt.
    /// </summary>
    public sealed class GameTimerService : IDisposable
    {
        private sealed class TimerEntry
        {
            public Timer Timer { get; }

            public DateTime ExpiresAtUtc { get; }

            public TimerEntry(
                Timer timer,
                DateTime expiresAtUtc)
            {
                Timer = timer;
                ExpiresAtUtc = expiresAtUtc;
            }
        }

        private readonly ConcurrentDictionary<
            string,
            TimerEntry> timers = new();

        private readonly TimeSpan turnDuration;

        private readonly Action<string> timeoutCallback;

        private int disposed;

        public TimeSpan TurnDuration =>
            turnDuration;

        public GameTimerService(
            TimeSpan? turnDuration = null,
            Action<string>? timeoutCallback = null)
        {
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
                timeoutCallback ?? (_ => { });
        }

        /// <summary>
        /// Start hoặc Reset Timer.
        /// </summary>
        public bool StartOrReset(
            string matchId)
        {
            if (IsDisposed())
                return false;

            if (string.IsNullOrWhiteSpace(matchId))
                return false;

            matchId = matchId.Trim();

            DateTime expiresAtUtc =
                DateTime.UtcNow.Add(turnDuration);

            Timer? newTimer = null;

            newTimer =
                new Timer(
                    _ =>
                        OnTimeout(
                            matchId,
                            newTimer!),
                    null,
                    turnDuration,
                    Timeout.InfiniteTimeSpan);

            TimerEntry newEntry =
                new TimerEntry(
                    newTimer,
                    expiresAtUtc);

            timers.AddOrUpdate(
                matchId,
                newEntry,
                (_, oldEntry) =>
                {
                    oldEntry.Timer.Dispose();

                    return newEntry;
                });

            return true;
        }

        /// <summary>
        /// Dừng Timer.
        /// </summary>
        public bool Stop(string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return false;

            matchId = matchId.Trim();

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
        /// Lấy thời gian còn lại.
        /// </summary>
        public int GetRemainingSeconds(
            string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return 0;

            if (!timers.TryGetValue(
                    matchId.Trim(),
                    out TimerEntry? entry))
            {
                return 0;
            }

            TimeSpan remaining =
                entry.ExpiresAtUtc -
                DateTime.UtcNow;

            if (remaining <= TimeSpan.Zero)
                return 0;

            return (int)Math.Ceiling(
                remaining.TotalSeconds);
        }

        public bool IsRunning(
            string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return false;

            return timers.ContainsKey(
                matchId.Trim());
        }

        private void OnTimeout(
            string matchId,
            Timer timer)
        {
            if (IsDisposed())
            {
                timer.Dispose();
                return;
            }

            if (!timers.TryGetValue(
                    matchId,
                    out TimerEntry? current))
            {
                timer.Dispose();
                return;
            }

            // Timer cũ -> bỏ qua.
            if (!ReferenceEquals(
                    current.Timer,
                    timer))
            {
                timer.Dispose();
                return;
            }

            // Chỉ Timer hiện tại mới được xử lý.
            if (!timers.TryRemove(
                    new KeyValuePair<string, TimerEntry>(
                        matchId,
                        current)))
            {
                timer.Dispose();
                return;
            }

            current.Timer.Dispose();

            if (!IsDisposed())
            {
                try
                {
                    timeoutCallback(matchId);
                }
                catch
                {
                    // Không để exception từ callback
                    // làm crash Timer thread.
                }
            }
        }

        private bool IsDisposed()
        {
            return Volatile.Read(
                ref disposed) != 0;
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(
                    ref disposed,
                    1) != 0)
            {
                return;
            }

            foreach (
                KeyValuePair<string, TimerEntry> item
                in timers)
            {
                item.Value.Timer.Dispose();
            }

            timers.Clear();
        }
    }
}