using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Server.Database;

public static class DatabaseHelper
{
    private static readonly string _connectionString = new DatabaseConfig().ConnectionString;

    /// <summary>
    /// Tạo và trả về một SqlConnection mới
    /// </summary>
    public static SqlConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }

    /// <summary>
    /// Dùng cho các câu lệnh INSERT, UPDATE, DELETE (trả về số dòng bị ảnh hưởng)
    /// </summary>
    public static int ExecuteNonQuery(string query, SqlParameter[]? parameters = null)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = new SqlCommand(query, conn))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                return cmd.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Dùng cho câu lệnh SELECT (trả về bảng dữ liệu DataTable)
    /// </summary>
    public static DataTable ExecuteQuery(string query, SqlParameter[]? parameters = null)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = new SqlCommand(query, conn))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }
    }

    /// <summary>
    /// Dùng cho câu lệnh INSERT cần lấy ID vừa tạo (SCOPE_IDENTITY / OUTPUT INSERTED)
    /// Có hỗ trợ truyền Transaction từ bên ngoài
    /// </summary>
    public static object? ExecuteScalar(string query, SqlParameter[]? parameters = null, SqlConnection? conn = null, SqlTransaction? trans = null)
    {
        bool isExternalConn = conn != null;
        SqlConnection connection = conn ?? GetConnection();

        try
        {
            if (connection.State != ConnectionState.Open) connection.Open();

            using (var cmd = new SqlCommand(query, connection, trans))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                return cmd.ExecuteScalar();
            }
        }
        finally
        {
            if (!isExternalConn) connection.Dispose();
        }
    }

    /// <summary>
    /// Kiểm tra kết nối CSDL khi khởi động Server (Chống sập Server)
    /// </summary>
    public static bool TestConnection()
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DB Error] Không thể kết nối Database: {ex.Message}");
            return false;
        }
    }
}