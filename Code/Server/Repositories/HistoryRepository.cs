using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Server.Database;

namespace Server.Repositories;

// DTO chứa thông tin từng nước đi trên bàn cờ
public class MoveDto
{
    public int PlayerId { get; set; }
    public int MoveX { get; set; }
    public int MoveY { get; set; }
    public int StepOrder { get; set; }
    public DateTime MoveTime { get; set; }
}

public class HistoryRepository
{
    /// <summary>
    /// 1. Lấy danh sách các trận đấu của User theo UserId (đã viết ở câu trên)
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
            Console.WriteLine($"[HistoryRepository Error - GetMatchHistoryByUserId]: {ex.Message}");
            return new DataTable();
        }
    }

    /// <summary>
    /// 2. Lưu toàn bộ các nước đi của bàn cờ vào CSDL khi kết thúc trận (dùng Transaction)
    /// </summary>
    public void InsertHistoryList(int matchId, List<MoveDto> moves, SqlConnection conn, SqlTransaction trans)
    {
        if (moves == null || moves.Count == 0) return;

        string sql = @"INSERT INTO History (MatchId, PlayerId, MoveX, MoveY, StepOrder, MoveTime)
                       VALUES (@MatchId, @PlayerId, @MoveX, @MoveY, @StepOrder, @MoveTime);";

        foreach (var move in moves)
        {
            using var cmd = new SqlCommand(sql, conn, trans);
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            cmd.Parameters.AddWithValue("@PlayerId", move.PlayerId);
            cmd.Parameters.AddWithValue("@MoveX", move.MoveX);
            cmd.Parameters.AddWithValue("@MoveY", move.MoveY);
            cmd.Parameters.AddWithValue("@StepOrder", move.StepOrder);
            cmd.Parameters.AddWithValue("@MoveTime", move.MoveTime);
            cmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// 3. Lấy danh sách nước đi của 1 trận đấu cụ thể (dùng cho tính năng Xem lại / Replay)
    /// </summary>
    public List<MoveDto> GetHistoryByMatchId(int matchId)
    {
        var list = new List<MoveDto>();
        string sql = @"SELECT PlayerId, MoveX, MoveY, StepOrder, MoveTime 
                       FROM History 
                       WHERE MatchId = @MatchId 
                       ORDER BY StepOrder ASC;";

        SqlParameter[] parameters = { new SqlParameter("@MatchId", matchId) };
        DataTable dt = DatabaseHelper.ExecuteQuery(sql, parameters);

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new MoveDto
            {
                PlayerId = Convert.ToInt32(row["PlayerId"]),
                MoveX = Convert.ToInt32(row["MoveX"]),
                MoveY = Convert.ToInt32(row["MoveY"]),
                StepOrder = Convert.ToInt32(row["StepOrder"]),
                MoveTime = Convert.ToDateTime(row["MoveTime"])
            });
        }
        return list;
    }
}