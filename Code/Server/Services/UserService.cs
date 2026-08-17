using System;
using System.Data;
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