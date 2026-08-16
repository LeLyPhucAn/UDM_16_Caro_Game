using System.Data;
using Microsoft.Data.SqlClient;
using CaroGame.Database;

namespace CaroGame.Repository
{
    public class UserRepository
    {
        public DataTable GetAllUsers()
        {
            return DatabaseHelper.ExecuteQuery("SELECT * FROM Users");
        }

        public int InsertUser(string username, string password)
        {
            string query = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
            SqlParameter[] parameters = {
                new SqlParameter("@Username", username),
                new SqlParameter("@Password", password)
            };
            return DatabaseHelper.ExecuteNonQuery(query, parameters);
        }
    }
}