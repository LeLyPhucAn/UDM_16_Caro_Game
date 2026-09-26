using Server.Utils;
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

    // Khởi tạo các repository mặc định
    public MatchService()
        : this(new MatchRepository(), new HistoryRepository(), new UserRepository())
    {
    }

    public MatchService(MatchRepository matchRepository, HistoryRepository historyRepository, UserRepository userRepository)
    {
        _matchRepository = matchRepository ?? throw new ArgumentNullException(nameof(matchRepository));
        _historyRepository = historyRepository ?? throw new ArgumentNullException(nameof(historyRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    // Tạo trận đấu mới và lưu vào CSDL
    public int StartNewMatch(int player1Id, int player2Id)
    {
        if (player1Id <= 0 || player2Id <= 0 || player1Id == player2Id)
        {
            Logger.Warn("[MatchService]: ID người chơi không hợp lệ.");
            return -1;
        }

        try
        {
            return _matchRepository.CreateMatch(player1Id, player2Id, DateTime.Now);
        }
        catch (Exception ex)
        {
            Logger.Error("[MatchService Error - StartNewMatch]", ex);
            return -1;
        }
    }

    // Lấy lịch sử đấu của một người chơi
    public DataTable GetUserMatchHistory(int userId)
    {
        if (userId <= 0) return new DataTable();

        try
        {
            return _historyRepository.GetMatchHistoryByUserId(userId);
        }
        catch (Exception ex)
        {
            Logger.Error("[MatchService Error - GetUserMatchHistory]", ex);
            return new DataTable();
        }
    }

    /// <summary>
    /// Ghi nhận chi tiết từng nước đi vào bảng History
    /// </summary>
    public bool RecordMove(int matchId, int playerId, int row, int col, int stepOrder)
    {
        if (matchId <= 0 || playerId <= 0) return false;

        try
        {
            return _historyRepository.InsertMoveHistory(matchId, playerId, row, col, stepOrder);
        }
        catch (Exception ex)
        {
            Logger.Error($"[MatchService Error - RecordMove] Match: {matchId}", ex);
            return false;
        }
    }

    // Lưu kết quả trận đấu và cập nhật thống kê người chơi
    public bool SaveMatchResult(int matchId, int? winnerId, string result)
    {
        return SaveMatchResult(matchId, winnerId, result, DateTime.Now);
    }

    public bool SaveMatchResult(int matchId, int? winnerId, string result, DateTime endTime)
    {
        if (matchId <= 0)
        {
            Logger.Warn("[MatchService]: MatchID không hợp lệ.");
            return false;
        }

        try
        {
            Logger.Info($"[MatchService]: Cập nhật kết quả Match #{matchId} (WinnerId: {winnerId?.ToString() ?? "Hòa/Hủy"}, Result: {result})");

            // Cập nhật thống kê người chơi (thắng / thua / hòa)
            DataTable matchInfo = _matchRepository.GetMatchById(matchId);
            if (matchInfo.Rows.Count > 0)
            {
                int p1 = Convert.ToInt32(matchInfo.Rows[0]["Player1Id"]);
                int p2 = Convert.ToInt32(matchInfo.Rows[0]["Player2Id"]);

                if (string.Equals(result, "DRAW", StringComparison.OrdinalIgnoreCase))
                {
                    _userRepository.UpdateUserStats(p1, false, true);  // hòa
                    _userRepository.UpdateUserStats(p2, false, true);  // hòa
                }
                else if (winnerId.HasValue)
                {
                    int loserId = (winnerId.Value == p1) ? p2 : p1;
                    _userRepository.UpdateUserStats(winnerId.Value, true, false);  // Thắng
                    _userRepository.UpdateUserStats(loserId, false, false);         // Thua
                }
            }

            return _matchRepository.EndMatch(matchId, winnerId, result, endTime);
        }
        catch (Exception ex)
        {
            Logger.Error("[MatchService DB Exception - SaveMatchResult]", ex);
            return false;
        }
    }
}