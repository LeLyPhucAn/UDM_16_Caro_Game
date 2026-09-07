using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Server.Database;

namespace Server.Repositories;

public class MatchRepository
{
    // 1. Tạo Match mới (Lưu Player, StartTime, Status)
    public int CreateMatch(int player1Id, int player2Id, DateTime startTime)
    {
        string sql = @"
            INSERT INTO Matches (Player1Id, Player2Id, StartTime, Status)
            OUTPUT INSERTED.MatchId
            VALUES (@Player1Id, @Player2Id, @StartTime, 'IN_PROGRESS');";

        SqlParameter[] parameters = {
            new SqlParameter("@Player1Id", player1Id),
            new SqlParameter("@Player2Id", player2Id),
            new SqlParameter("@StartTime", startTime)
        };

        try
        {
            object? result = DatabaseHelper.ExecuteScalar(sql, parameters);
            return result != null ? Convert.ToInt32(result) : -1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MatchRepository Error - CreateMatch]: {ex.Message}");
            return -1;
        }
    }

    // 2. Lưu thông tin khi kết thúc (Winner, Result, EndTime, Status)
    public bool EndMatch(int matchId, int? winnerId, string result, DateTime endTime)
    {
        string sql = @"
            UPDATE Matches
            SET EndTime = @EndTime,
                WinnerId = @WinnerId,
                Result = @Result,
                Status = 'COMPLETED'
            WHERE MatchId = @MatchId;";

        SqlParameter[] parameters = {
            new SqlParameter("@MatchId", matchId),
            new SqlParameter("@EndTime", endTime),
            new SqlParameter("@WinnerId", (object?)winnerId ?? DBNull.Value),
            new SqlParameter("@Result", (object?)result ?? DBNull.Value)
        };

        try
        {
            return DatabaseHelper.ExecuteNonQuery(sql, parameters) > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MatchRepository Error - EndMatch]: {ex.Message}");
            return false;
        }
    }

    // 3. Truy vấn Match theo ID
    public DataTable GetMatchById(int matchId)
    {
        string sql = "SELECT * FROM Matches WHERE MatchId = @MatchId;";
        SqlParameter[] parameters = { new SqlParameter("@MatchId", matchId) };

        try
        {
            return DatabaseHelper.ExecuteQuery(sql, parameters);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MatchRepository Error - GetMatchById]: {ex.Message}");
            return new DataTable();
        }
    }
}