using System;
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

    // Lấy thông tin User theo UserId
    public DataTable GetUserById(int userId)
    {
        string query = "SELECT * FROM Users WHERE Id = @Id";
        SqlParameter[] parameters = {
            new SqlParameter("@Id", userId)
        };
        return DatabaseHelper.ExecuteQuery(query, parameters);
    }

    public DataTable ValidateUser(string username, string password)
    {
        string query = "SELECT * FROM Users WHERE Username = @Username AND PasswordHash = @Password";
        SqlParameter[] parameters = {
            new SqlParameter("@Username", username),
            new SqlParameter("@Password", password)
        };
        return DatabaseHelper.ExecuteQuery(query, parameters);
    }

    public int InsertUser(string username, string password)
    {
        string query = "INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @Password)";
        SqlParameter[] parameters = {
            new SqlParameter("@Username", username),
            new SqlParameter("@Password", password)
        };
        return DatabaseHelper.ExecuteNonQuery(query, parameters);
    }

    // Cập nhật số trận thắng, thua hoặc hòa của người chơi
    public bool UpdateUserStats(int userId, bool isWinner, bool isDraw = false)
    {
        string query;
        if (isDraw)
        {
            query = "UPDATE Users SET Draws = ISNULL(Draws, 0) + 1 WHERE Id = @Id";
        }
        else if (isWinner)
        {
            query = "UPDATE Users SET Wins = ISNULL(Wins, 0) + 1 WHERE Id = @Id";
        }
        else
        {
            query = "UPDATE Users SET Losses = ISNULL(Losses, 0) + 1 WHERE Id = @Id";
        }

        SqlParameter[] parameters = { new SqlParameter("@Id", userId) };
        return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
    }
}