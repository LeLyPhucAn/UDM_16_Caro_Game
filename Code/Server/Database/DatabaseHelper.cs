using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Server.Database;

public class DatabaseHelper
{
    private static readonly string _defaultConnectionString = new DatabaseConfig().ConnectionString;
    private readonly string _connectionString;

    // Constructor mặc định (sử dụng cấu hình từ DatabaseConfig)
    public DatabaseHelper() : this(new DatabaseConfig())
    {
    }

    // Constructor nhận cấu hình từ bên ngoài
    public DatabaseHelper(DatabaseConfig config)
    {
        _connectionString = config.ConnectionString;
    }

    /// <summary>
    /// Tạo và trả về một SqlConnection mới
    /// </summary>
    public static SqlConnection GetConnection()
    {
        return new SqlConnection(_defaultConnectionString);
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
}
