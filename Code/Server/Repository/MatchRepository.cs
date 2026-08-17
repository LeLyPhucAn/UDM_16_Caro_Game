using System.Data;
using Microsoft.Data.SqlClient;
using CaroGame.Database;

namespace CaroGame.Repository
{
    public class MatchRepository
    {
        public DataTable GetMatchHistory(int userId)
        {
            string query = "SELECT * FROM Matches WHERE Player1ID = @UserId OR Player2ID = @UserId";
            SqlParameter[] parameters = { new SqlParameter("@UserId", userId) };
            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        public int SaveMatch(int player1Id, int player2Id, int winnerId)
        {
            string query = "INSERT INTO Matches (Player1ID, Player2ID, WinnerID, MatchDate) VALUES (@P1, @P2, @Winner, GETDATE())";
            SqlParameter[] parameters = {
                new SqlParameter("@P1", player1Id),
                new SqlParameter("@P2", player2Id),
                new SqlParameter("@Winner", winnerId)
            };
            return DatabaseHelper.ExecuteNonQuery(query, parameters);
        }
    }
}