using System.Data.SqlClient;

namespace Server.Database;

public class DatabaseHelper
{
    private readonly string _connectionString;

    public DatabaseHelper(DatabaseConfig config)
    {
        _connectionString = config.ConnectionString;
    }

    public SqlConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }
}