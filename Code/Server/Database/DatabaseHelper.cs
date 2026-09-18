using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Server.Database;

public static class DatabaseHelper
{
    private static string _connectionString = new DatabaseConfig().ConnectionString;

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
    /// Tự động kiểm tra và khởi tạo Database CaroDB cùng các bảng nếu chưa có trên máy mới
    /// </summary>
    public static bool EnsureDatabaseCreated()
    {
        string[] candidateServers = new[]
        {
            @"(localdb)\mssqllocaldb",
            "localhost",
            @".\SQLEXPRESS",
            "."
        };

        foreach (var server in candidateServers)
        {
            try
            {
                string masterConnStr = $"Server={server};Database=master;Trusted_Connection=True;TrustServerCertificate=True;";
                using (var masterConn = new SqlConnection(masterConnStr))
                {
                    masterConn.Open();

                    // 1. Tạo Database CaroDB nếu chưa tồn tại
                    string createDbSql = "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'CaroDB') CREATE DATABASE CaroDB;";
                    using (var cmd = new SqlCommand(createDbSql, masterConn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                // 2. Kết nối trực tiếp vào CaroDB để tạo bảng nếu chưa có
                string caroDbConnStr = $"Server={server};Database=CaroDB;Trusted_Connection=True;TrustServerCertificate=True;";
                using (var caroConn = new SqlConnection(caroDbConnStr))
                {
                    caroConn.Open();
                    string createTablesSql = @"
                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
                        CREATE TABLE Users (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Username NVARCHAR(50) NOT NULL UNIQUE,
                            PasswordHash NVARCHAR(256) NOT NULL,
                            Score INT NOT NULL DEFAULT 0,
                            Wins INT NOT NULL DEFAULT 0,
                            Losses INT NOT NULL DEFAULT 0,
                            Draws INT NOT NULL DEFAULT 0,
                            CreatedAt DATETIME DEFAULT GETDATE()
                        );

                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Matches')
                        CREATE TABLE Matches (
                            MatchId INT IDENTITY(1,1) PRIMARY KEY,
                            Player1Id INT NOT NULL,
                            Player2Id INT NOT NULL,
                            StartTime DATETIME NOT NULL,
                            EndTime DATETIME,
                            WinnerId INT,
                            Result NVARCHAR(50),
                            Status NVARCHAR(50) NOT NULL DEFAULT 'IN_PROGRESS'
                        );

                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'History')
                        CREATE TABLE History (
                            HistoryId INT IDENTITY(1,1) PRIMARY KEY,
                            MatchId INT NOT NULL,
                            PlayerId INT NOT NULL,
                            MoveX INT NOT NULL,
                            MoveY INT NOT NULL,
                            StepOrder INT NOT NULL,
                            MoveTime DATETIME DEFAULT GETDATE()
                        );
                    ";
                    using (var cmd = new SqlCommand(createTablesSql, caroConn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                _connectionString = caroDbConnStr;
                Console.WriteLine($"[Database] Đã kết nối và tự động khởi tạo CaroDB tại server: {server}");
                return true;
            }
            catch
            {
                // Thử ứng viên tiếp theo nếu server này không chạy
                continue;
            }
        }

        Console.WriteLine("[Database Warning] Không thể tự động kết nối SQL Server. Vui lòng kiểm tra dịch vụ SQL Server.");
        return false;
    }

    /// <summary>
    /// Kiểm tra kết nối CSDL khi khởi động Server
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