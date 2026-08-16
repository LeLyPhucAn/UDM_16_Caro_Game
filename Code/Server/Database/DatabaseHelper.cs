using Microsoft.Data.SqlClient;
using System.Data;

namespace CaroGame.Database
{
    public class DatabaseHelper
    {
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(Config.ConnectionString);
        }

        // Dùng cho INSERT, UPDATE, DELETE
        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // Dùng cho SELECT lấy dữ liệu
        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
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
}