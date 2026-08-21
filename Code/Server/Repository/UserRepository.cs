using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Server.Repository
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Tạo User
        public bool CreateUser(string username, string passwordHash, string email = null)
        {
            string query = "INSERT INTO Users (Username, PasswordHash, Email, CreatedAt) VALUES (@Username, @PasswordHash, @Email, GETDATE())";
            
            using (var connection = GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                command.Parameters.AddWithValue("@Email", (object)email ?? DBNull.Value);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (SqlException ex)
                {
                    // Xử lý Database Exception
                    Console.WriteLine($"Database Error (CreateUser): {ex.Message}");
                    throw;
                }
            }
        }

        //Kiểm tra Username đã tồn tại 
        public bool IsUsernameExists(string username)
        {
            string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username";

            using (var connection = GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);

                try
                {
                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Database Error (IsUsernameExists): {ex.Message}");
                    throw;
                }
            }
        }

        //Tìm User theo Username
        public DataRow GetUserByUsername(string username)
        {
            string query = "SELECT * FROM Users WHERE Username = @Username";

            using (var connection = GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);

                try
                {
                    connection.Open();
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Database Error (GetUserByUsername): {ex.Message}");
                    throw;
                }
            }
        }

        //Tìm User theo ID
        public DataRow GetUserById(int userId)
        {
            string query = "SELECT * FROM Users WHERE Id = @Id";

            using (var connection = GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", userId);

                try
                {
                    connection.Open();
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Database Error (GetUserById): {ex.Message}");
                    throw;
                }
            }
        }

        //Update thông tin User
        public bool UpdateUser(int userId, string email, string passwordHash)
        {
            string query = "UPDATE Users SET Email = @Email, PasswordHash = @PasswordHash WHERE Id = @Id";

            using (var connection = GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", userId);
                command.Parameters.AddWithValue("@Email", (object)email ?? DBNull.Value);
                command.Parameters.AddWithValue("@PasswordHash", passwordHash);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Database Error (UpdateUser): {ex.Message}");
                    throw;
                }
            }
        }
    }
}