using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Server.Repository
{
    public class HistoryRepository
    {
        private readonly string _connectionString;

        public HistoryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqlConnection GetConnection() => new SqlConnection(_connectionString);

        // Truy vấn lịch sử trận đấu của 1 Player
        public DataTable GetMatchHistoryByUserId(int userId)
        {
            string query = @"SELECT m.Id AS MatchId, 
                                    u1.Username AS Player1, 
                                    u2.Username AS Player2, 
                                    w.Username AS Winner, 
                                    m.Result, m.StartTime, m.EndTime
                             FROM Matches m
                             JOIN Users u1 ON m.Player1Id = u1.Id
                             JOIN Users u2 ON m.Player2Id = u2.Id
                             LEFT JOIN Users w ON m.WinnerId = w.Id
                             WHERE m.Player1Id = @UserId OR m.Player2Id = @UserId
                             ORDER BY m.StartTime DESC";

            using (var connection = GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);

                try
                {
                    connection.Open();
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Database Error (GetMatchHistoryByUserId): {ex.Message}");
                    throw;
                }
            }
        }
    }
}