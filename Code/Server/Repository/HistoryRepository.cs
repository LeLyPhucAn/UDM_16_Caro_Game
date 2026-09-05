using System;
using System.Data;
using System.Data.SqlClient;

namespace Server.Repository
{
    public class HistoryRepository
    {
        /// <summary>
        /// Truy vấn lịch sử đấu của User theo UserId
        /// </summary>
        public DataTable GetMatchHistoryByUserId(int userId)
        {
            DataTable dt = new DataTable();
            string sql = @"
                SELECT MatchId, Player1Id, Player2Id, StartTime, EndTime, WinnerId, Result, Status
                FROM Matches
                WHERE Player1Id = @UserId OR Player2Id = @UserId
                ORDER BY StartTime DESC;";

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HistoryRepository Error - GetMatchHistoryByUserId]: {ex.Message}");
            }

            return dt;
        }
    }
}