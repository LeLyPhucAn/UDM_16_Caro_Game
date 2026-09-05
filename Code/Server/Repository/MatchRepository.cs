using System;
using System.Data;
using System.Data.SqlClient;

namespace Server.Repository
{
    public class MatchRepository
    {
        // 1. Tạo Match mới (Lưu Player, StartTime, Status)
        public int CreateMatch(int player1Id, int player2Id, DateTime startTime)
        {
            string sql = @"
                INSERT INTO Matches (Player1Id, Player2Id, StartTime, Status)
                OUTPUT INSERTED.MatchId
                VALUES (@Player1Id, @Player2Id, @StartTime, 'IN_PROGRESS');";

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Player1Id", player1Id);
                        cmd.Parameters.AddWithValue("@Player2Id", player2Id);
                        cmd.Parameters.AddWithValue("@StartTime", startTime);
                        return (int)cmd.ExecuteScalar();
                    }
                }
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

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchId", matchId);
                        cmd.Parameters.AddWithValue("@EndTime", endTime);
                        cmd.Parameters.AddWithValue("@WinnerId", (object)winnerId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Result", (object)result ?? DBNull.Value);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MatchRepository Error - EndMatch]: {ex.Message}");
                return false;
            }
        }

        // 3. Truy vấn Match theo ID (YÊU CẦU BỔ SUNG)
        public DataTable GetMatchById(int matchId)
        {
            DataTable dt = new DataTable();
            string sql = "SELECT * FROM Matches WHERE MatchId = @MatchId;";

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchId", matchId);
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MatchRepository Error - GetMatchById]: {ex.Message}");
            }

            return dt;
        }
    }
}