using System;
using System.Data.SqlClient; 

namespace Server.Repository
{
    public static class DatabaseConnection
    {
        // Chuỗi kết nối chuẩn LocalDB
        private static readonly string ConnectionString = @"Server=(localdb)\MSSQLLocalDB;Database=CaroDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
