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
        }
    }
}