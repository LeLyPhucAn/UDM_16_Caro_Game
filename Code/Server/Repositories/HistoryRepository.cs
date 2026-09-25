using Server.Utils;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Server.Database;

namespace Server.Repositories;

public class HistoryRepository
{
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