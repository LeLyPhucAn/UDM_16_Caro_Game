using System;
using System.Data;
using Server.Repository;

namespace Server.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Đăng ký User
        public bool Register(string username, string password, string email = null)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            if (_userRepository.IsUsernameExists(username))
            {
                return false; // Username đã tồn tại
            }

            // Mã hóa mật khẩu
            string hashedPassword = HashPassword(password);
            return _userRepository.CreateUser(username, hashedPassword, email);
        }

        // Hỗ trợ Login và kiểm tra dữ liệu từ Database
        public bool Login(string username, string password, out DataRow userRow)
        {
            userRow = null;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            userRow = _userRepository.GetUserByUsername(username);
            if (userRow == null)
            {
                return false; // Không tìm thấy User
            }

            string storedHash = userRow["PasswordHash"].ToString();
            return VerifyPassword(password, storedHash);
        }

        // Tìm kiếm User theo Username/ID
        public DataRow GetUser(string username) => _userRepository.GetUserByUsername(username);
        public DataRow GetUser(int userId) => _userRepository.GetUserById(userId);

        // Update thông tin User
        public bool UpdateUserProfile(int userId, string newEmail, string newPassword)
        {
            string passwordHash = string.IsNullOrEmpty(newPassword) ? null : HashPassword(newPassword);
            return _userRepository.UpdateUser(userId, newEmail, passwordHash);
        }

        // Hàm hỗ trợ hash mật khẩu đơn giản 
        private string HashPassword(string password)
        {
            return password; 
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return HashPassword(password) == storedHash;

using Server.Repositories;

namespace Server.Services;

public class UserService
{
    private readonly UserRepository _userRepo = new();

    /// <summary>
    /// Đăng ký tài khoản mới. Trả về true nếu thành công, false nếu thất bại (hoặc đã tồn tại).
    /// </summary>
    public bool Register(string username, string password)
    {
        try
        {
            // Kiểm tra xem username đã tồn tại chưa
            DataTable existing = _userRepo.GetUserByUsername(username);
            if (existing != null && existing.Rows.Count > 0)
            {
                return false; // Tên đăng nhập đã tồn tại
            }

            int rows = _userRepo.InsertUser(username, password);
            return rows > 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Kiểm tra thông tin đăng nhập. Trả về true nếu đúng username và password.
    /// </summary>
    public bool Login(string username, string password)
    {
        try
        {
            DataTable dt = _userRepo.ValidateUser(username, password);
            return dt != null && dt.Rows.Count > 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Lấy danh sách tất cả người dùng trong hệ thống
    /// </summary>
    public DataTable GetAllUsers()
    {
        return _userRepo.GetAllUsers();
    }
}