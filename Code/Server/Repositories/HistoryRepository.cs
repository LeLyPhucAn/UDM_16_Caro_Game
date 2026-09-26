using Server.Utils;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Server.Database;

namespace Server.Repositories;

public class HistoryRepository
{
    /// <summary>
    /// Lưu chi tiết từng nước đi vào bảng History
    /// </summary>
    public bool InsertMoveHistory(int matchId, int playerId, int moveX, int moveY, int stepOrder)
    {
        if (matchId <= 0 || playerId <= 0) return false;

        string sql = @"
            INSERT INTO History (MatchId, PlayerId, MoveX, MoveY, StepOrder, MoveTime)
            VALUES (@MatchId, @PlayerId, @MoveX, @MoveY, @StepOrder, GETDATE());";

        SqlParameter[] parameters = {
            new SqlParameter("@MatchId", matchId),
            new SqlParameter("@PlayerId", playerId),
            new SqlParameter("@MoveX", moveX),
            new SqlParameter("@MoveY", moveY),
            new SqlParameter("@StepOrder", stepOrder)
        };

        try
        {
            return DatabaseHelper.ExecuteNonQuery(sql, parameters) > 0;
        }
        catch (Exception ex)
        {
            Logger.Error($"[HistoryRepository Error - InsertMoveHistory] Match: {matchId}, Step: {stepOrder}", ex);
            return false;
        }
    }

    /// <summary>
    /// Lấy danh sách lịch sử các trận đấu của người chơi theo UserId
    /// </summary>
    public DataTable GetMatchHistoryByUserId(int userId)
    {
        string sql = @"
            SELECT MatchId, Player1Id, Player2Id, StartTime, EndTime, WinnerId, Result, Status
            FROM Matches
            WHERE Player1Id = @UserId OR Player2Id = @UserId
            ORDER BY StartTime DESC;";

        SqlParameter[] parameters = { new SqlParameter("@UserId", userId) };

        try
        {
            return DatabaseHelper.ExecuteQuery(sql, parameters);
        }
        catch (Exception ex)
        {
            Logger.Error("[HistoryRepository Error - GetMatchHistoryByUserId]", ex);
            return new DataTable();
        }
    }
}