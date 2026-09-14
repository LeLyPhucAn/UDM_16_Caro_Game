using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Server.Utils;

namespace Server.Managers;

/// <summary>
/// Quan ly grace period (90 giay) cho tinh nang ket noi lai (Reconnect).
/// Khi mot Player disconnect giua tran, Manager giu trang thai tam dung trong
/// GracePeriodSeconds giay. Neu Player khong ket noi lai kip, event OnGraceExpired se duoc kich hoat.
/// </summary>
public class ReconnectManager
{
    /// <summary>
    /// Thoi gian cho reconnect tinh bang giay (90 giay).
    /// </summary>
    public static readonly int GracePeriodSeconds = 60;

    // Key: PlayerId (Session GUID dang string)
    // Value: CancellationTokenSource de huy grace period khi Player reconnect kip
    private readonly Dictionary<string, CancellationTokenSource> _pendingGraces = new();
    private readonly object _lock = new();

    /// <summary>
    /// Duoc kich hoat khi grace period het han ma Player khong reconnect.
    /// Arg: playerId (Session ID cu cua Player da disconnect).
    /// </summary>
    public event Action<string>? OnGraceExpired;

    /// <summary>
    /// Bat dau dem nguoc grace period cho Player vua disconnect.
    /// Neu Player da co grace period dang chay, reset lai tu dau.
    /// </summary>
    public void StartGrace(string playerId)
    {
        CancelGrace(playerId);

        var cts = new CancellationTokenSource();
        lock (_lock)
        {
            _pendingGraces[playerId] = cts;
        }

        Logger.Info($"[Reconnect] Grace period {GracePeriodSeconds}s bat dau cho Player {playerId}");

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(GracePeriodSeconds), cts.Token);

                lock (_lock)
                {
                    _pendingGraces.Remove(playerId);
                }

                Logger.Warn($"[Reconnect] Grace period het han cho Player {playerId}. Ket thuc tran dau.");
                OnGraceExpired?.Invoke(playerId);
            }
            catch (TaskCanceledException)
            {
                Logger.Info($"[Reconnect] Grace period da bi huy (Player {playerId} da reconnect).");
            }
        });
    }

    /// <summary>
    /// Huy grace period - goi khi Player reconnect thanh cong trong thoi gian cho phep.
    /// </summary>
    public bool CancelGrace(string playerId)
    {
        lock (_lock)
        {
            if (_pendingGraces.TryGetValue(playerId, out var cts))
            {
                cts.Cancel();
                cts.Dispose();
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
