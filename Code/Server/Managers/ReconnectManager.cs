using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Server.Utils;

namespace Server.Managers;

/// <summary>
/// Loại grace period (Chờ lần đầu hay Chờ thêm)
/// </summary>
public enum GraceType
{
    Initial,
    Extended
}

/// <summary>
/// Quan ly grace period cho tinh nang ket noi lai (Reconnect).
/// </summary>
public class ReconnectManager
{
    /// <summary>
    /// Thoi gian cho reconnect lan dau (60 giay).
    /// </summary>
    public static readonly int InitialGraceSeconds = 90;

    /// <summary>
    /// Thoi gian cho them khi doi thu dong y (120 giay).
    /// </summary>
    public static readonly int ExtendedGraceSeconds = 120;

    // Key: PlayerId (Session GUID dang string)
    // Value: (CancellationTokenSource, GraceType) de huy grace period va xac dinh loai
    private readonly Dictionary<string, (CancellationTokenSource Cts, GraceType Type)> _pendingGraces = new();
    private readonly object _lock = new();

    /// <summary>
    /// Duoc kich hoat khi grace period het han ma Player khong reconnect.
    /// Arg1: playerId, Arg2: loai grace period vua het han.
    /// </summary>
    public event Action<string, GraceType>? OnGraceExpired;

    /// <summary>
    /// Bat dau dem nguoc grace period cho Player vua disconnect.
    /// </summary>
    public void StartGrace(string playerId, GraceType type, int seconds)
    {
        CancelGrace(playerId);

        var cts = new CancellationTokenSource();
        lock (_lock)
        {
            _pendingGraces[playerId] = (cts, type);
        }

        Logger.Info($"[Reconnect] Grace period ({type}) {seconds}s bat dau cho Player {playerId}");

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(seconds), cts.Token);

                lock (_lock)
                {
                    _pendingGraces.Remove(playerId);
                }

                Logger.Warn($"[Reconnect] Grace period ({type}) het han cho Player {playerId}.");
                OnGraceExpired?.Invoke(playerId, type);
            }
            catch (TaskCanceledException)
            {
                Logger.Info($"[Reconnect] Grace period da bi huy (Player {playerId} da reconnect hoac duoc gia han).");
            }
        });
    }

    /// <summary>
    /// Huy grace period - goi khi Player reconnect thanh cong.
    /// </summary>
    public bool CancelGrace(string playerId)
    {
        lock (_lock)
        {
            if (_pendingGraces.TryGetValue(playerId, out var tuple))
            {
                tuple.Cts.Cancel();
                tuple.Cts.Dispose();
                _pendingGraces.Remove(playerId);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Kiem tra xem Player co dang trong grace period (cho reconnect) khong.
    /// </summary>
    public bool HasPendingGrace(string playerId)
    {
        lock (_lock)
        {
            return _pendingGraces.ContainsKey(playerId);
        }
    }

    /// <summary>
    /// Tra ve danh sach tat ca Player dang trong grace period.
    /// </summary>
    public IReadOnlyList<string> GetAllPendingPlayers()
    {
        lock (_lock)
        {
            return new List<string>(_pendingGraces.Keys).AsReadOnly();
        }
    }
}
