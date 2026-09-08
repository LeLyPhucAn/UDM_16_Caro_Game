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

    // BỔ SUNG 1: Lấy thông tin User bằng UserId
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

    // BỔ SUNG 2: Cập nhật WinCount / LossCount khi trận đấu kết thúc (Chạy chung Transaction với MatchService)
    public bool UpdateUserStats(int userId, bool isWinner, SqlConnection conn, SqlTransaction trans)
    {
        // Thay tên cột WinCount, LossCount hoặc Score theo đúng tên cột trong CSDL của bạn
        string query = isWinner 
            ? "UPDATE Users SET Wins = ISNULL(Wins, 0) + 1 WHERE Id = @Id"
            : "UPDATE Users SET Losses = ISNULL(Losses, 0) + 1 WHERE Id = @Id";

        using (var cmd = new SqlCommand(query, conn, trans))
        {
            cmd.Parameters.AddWithValue("@Id", userId);
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    // BỔ SUNG 3: Overload đơn giản (không cần Transaction ngoài) - dùng DatabaseHelper
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