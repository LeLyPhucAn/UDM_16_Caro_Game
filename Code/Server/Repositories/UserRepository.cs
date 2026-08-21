using System.Data;
using Microsoft.Data.SqlClient;
using Server.Database;

namespace Server.Repositories;

public class UserRepository
{
    public DataTable GetAllUsers()
    {
        return DatabaseHelper.ExecuteQuery("SELECT * FROM Users");
    }

    public DataTable GetUserByUsername(string username)
    {
        string query = "SELECT * FROM Users WHERE Username = @Username";
        SqlParameter[] parameters = {
            new SqlParameter("@Username", username)
        };
        return DatabaseHelper.ExecuteQuery(query, parameters);
    }

    public DataTable ValidateUser(string username, string password)
    {
        string query = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
        SqlParameter[] parameters = {
            new SqlParameter("@Username", username),
            new SqlParameter("@Password", password)
        };
        return DatabaseHelper.ExecuteQuery(query, parameters);
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