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
        string query = "SELECT * FROM Users WHERE UserId = @UserId";
        SqlParameter[] parameters = {
            new SqlParameter("@UserId", userId)
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

    // BỔ SUNG 2: Cập nhật WinCount / LossCount khi trận đấu kết thúc (Chạy chung Transaction với MatchService)
    public bool UpdateUserStats(int userId, bool isWinner, SqlConnection conn, SqlTransaction trans)
    {
        // Thay tên cột WinCount, LossCount hoặc Score theo đúng tên cột trong CSDL của bạn
        string query = isWinner 
            ? "UPDATE Users SET WinCount = ISNULL(WinCount, 0) + 1 WHERE UserId = @UserId"
            : "UPDATE Users SET LossCount = ISNULL(LossCount, 0) + 1 WHERE UserId = @UserId";

        using (var cmd = new SqlCommand(query, conn, trans))
        {
            cmd.Parameters.AddWithValue("@UserId", userId);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}