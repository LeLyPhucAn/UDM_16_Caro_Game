using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Server.Repository
{
    public class MatchRepository
    {
        private readonly string _connectionString;

        public MatchRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqlConnection GetConnection() => new SqlConnection(_connectionString);

        // Tạo Match Record mới và trả về MatchId vừa tạo
        public int CreateMatch(int player1Id, int player2Id, DateTime startTime)
        {
            string query = @"INSERT INTO Matches (Player1Id, Player2Id, StartTime, Status) 
                            OUTPUT INSERTED.Id 
                            VALUES (@Player1Id, @Player2Id, @StartTime, 'Ongoing')";

            using (var connection = GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Player1Id", player1Id);
                command.Parameters.AddWithValue("@Player2Id", player2Id);
                command.Parameters.AddWithValue("@StartTime", startTime);

                try
                {
                    connection.Open();
                    return (int)command.ExecuteScalar();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Database Error (CreateMatch): {ex.Message}");
                    throw;
                }
            }
        }

        // Cập nhật kết quả khi Match kết thúc
        public bool EndMatch(int matchId, int? winnerId, string result, DateTime endTime)
        {
            string query = @"UPDATE Matches 
                            SET WinnerId = @WinnerId, Result = @Result, EndTime = @EndTime, Status = 'Completed' 
                            WHERE Id = @MatchId";

            using (var connection = GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@MatchId", matchId);
                command.Parameters.AddWithValue("@WinnerId", (object)winnerId ?? DBNull.Value);
                command.Parameters.AddWithValue("@Result", result);
                command.Parameters.AddWithValue("@EndTime", endTime);

                try
                {
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Database Error (EndMatch): {ex.Message}");
                    throw;
                }
            }
        }
    }
}