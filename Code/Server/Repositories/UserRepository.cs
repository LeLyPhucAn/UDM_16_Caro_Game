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

    public bool UpdatePassword(string username, string passwordHash)
    {
        string query = "UPDATE Users SET PasswordHash = @PasswordHash WHERE Username = @Username";
        SqlParameter[] parameters = {
            new SqlParameter("@Username", username),
            new SqlParameter("@PasswordHash", passwordHash)
        };
        return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
    }

    // Cập nhật số trận thắng, thua, hòa và điểm số của người chơi
    public bool UpdateUserStats(int userId, bool isWinner, bool isDraw = false)
    {
        string query;
        if (isDraw)
        {
            // Hòa: cộng 1 điểm
            query = "UPDATE Users SET Draws = ISNULL(Draws, 0) + 1, Score = ISNULL(Score, 0) + 1 WHERE Id = @Id";
        }
        else if (isWinner)
        {
            // Thắng: cộng 3 điểm
            query = "UPDATE Users SET Wins = ISNULL(Wins, 0) + 1, Score = ISNULL(Score, 0) + 3 WHERE Id = @Id";
        }
        else
        {
            // Thua: giữ nguyên điểm số, tăng số trận thua
            query = "UPDATE Users SET Losses = ISNULL(Losses, 0) + 1 WHERE Id = @Id";
        }

        SqlParameter[] parameters = { new SqlParameter("@Id", userId) };
        return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
    }
}