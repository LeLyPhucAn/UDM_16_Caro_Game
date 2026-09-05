using System;
using System.Collections.Generic;
using System.Data;
using Server.Repositories;

namespace Server.Services;

public class MatchService
{
    private readonly MatchRepository _matchRepository;
    private readonly HistoryRepository _historyRepository;
    private readonly UserRepository _userRepository;

    public MatchService()
    {
        _matchRepository = new MatchRepository();
        _historyRepository = new HistoryRepository();
        _userRepository = new UserRepository();
    }

    /// <summary>
    /// 1. Tạo trận đấu mới
    /// </summary>
    public int StartNewMatch(int player1Id, int player2Id)
    {
        if (player1Id <= 0 || player2Id <= 0 || player1Id == player2Id)
        {
            Console.WriteLine("[MatchService Warning]: ID người chơi không hợp lệ.");
            return -1;
        }

        try
        {
            return _matchRepository.CreateMatch(player1Id, player2Id, DateTime.Now);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MatchService Error - StartNewMatch]: {ex.Message}");
            return -1;
        }
    }

    /// <summary>
    /// 2. Hủy trận đấu (Khi thoát game / mất mạng)
    /// </summary>
    public bool CancelMatch(int matchId, string reason)
    {
        if (matchId <= 0) return false;

        try
        {
            return _matchRepository.EndMatch(matchId, null, $"CANCELLED: {reason}", DateTime.Now);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MatchService Error - CancelMatch]: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 3. Lấy lịch sử đấu của một User
    /// </summary>
    public DataTable GetUserMatchHistory(int userId)
    {
        if (userId <= 0) return new DataTable();

        try
        {
            return _historyRepository.GetMatchHistoryByUserId(userId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MatchService Error - GetUserMatchHistory]: {ex.Message}");
            return new DataTable();
        }
    }
    /// <summary>
    /// Lưu kết quả trận đấu khi kết thúc (được gọi từ MatchManager)
    /// </summary>
    public bool SaveMatchResult(int matchId, int? winnerId, string result)
    {
        return SaveMatchResult(matchId, winnerId, result, DateTime.Now);
    }

    public bool SaveMatchResult(int matchId, int? winnerId, string result, DateTime endTime)
    {
        if (matchId <= 0) return false;

        try
        {
            return _matchRepository.EndMatch(matchId, winnerId, result, endTime);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MatchService Error - SaveMatchResult]: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 4. Lấy danh sách nước đi để Replay trận đấu
    /// </summary>
    public List<MoveDto> GetMatchReplayMoves(int matchId)
    {
        if (matchId <= 0) return new List<MoveDto>();

        try
        {
            return _historyRepository.GetHistoryByMatchId(matchId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MatchService Error - GetMatchReplayMoves]: {ex.Message}");
            return new List<MoveDto>();
        }
    }
}