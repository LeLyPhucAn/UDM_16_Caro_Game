
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
                Console.WriteLine($"[UserService]: Tên đăng nhập '{username}' đã tồn tại.");
                return false;
            }

            int rows = _userRepo.InsertUser(username, password);
            return rows > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UserService Exception - Register]: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Đăng nhập đơn giản (Trả về true/false)
    /// </summary>
    public bool Login(string username, string password)
    {
        return Login(username, password, out _);
    }

    /// <summary>
    /// Đăng nhập và lấy thông tin chi tiết User (out DataRow userRow).
    /// Giúp Server lấy ngay UserId để gán vào Session của Socket.
    /// </summary>
    public bool Login(string username, string password, out DataRow? userRow)
    {
        userRow = null;
        try
        {
            DataTable dt = _userRepo.ValidateUser(username, password);
            if (dt != null && dt.Rows.Count > 0)
            {
                userRow = dt.Rows[0];
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UserService Exception - Login]: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Lấy thông tin User theo UserId (Hiển thị Trang thông tin cá nhân)
    /// </summary>
    public DataRow? GetUserById(int userId)
    {
        if (userId <= 0) return null;

        try
        {
            DataTable dt = _userRepo.GetUserById(userId);
            return (dt != null && dt.Rows.Count > 0) ? dt.Rows[0] : null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UserService Exception - GetUserById]: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Lấy danh sách tất cả người dùng (Dùng cho Bảng xếp hạng / Danh sách Online)
    /// </summary>
    public DataTable GetAllUsers()
    {
        try
        {
            return _userRepo.GetAllUsers();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UserService Exception - GetAllUsers]: {ex.Message}");
            return new DataTable();
        }
    }

    /// <summary>
    /// Lấy UserId dựa trên Username
    /// </summary>
    public int GetUserId(string username)
    {
        try
        {
            DataTable dt = _userRepo.GetUserByUsername(username);
            if (dt != null && dt.Rows.Count > 0)
            {
                if (dt.Columns.Contains("Id") && dt.Rows[0]["Id"] != DBNull.Value)
                    return Convert.ToInt32(dt.Rows[0]["Id"]);
                if (dt.Columns.Contains("UserId") && dt.Rows[0]["UserId"] != DBNull.Value)
                    return Convert.ToInt32(dt.Rows[0]["UserId"]);
            }
        }
        catch { }
        return 0;
    }

    /// <summary>
    /// Lấy thông tin hồ sơ Profile của User theo Username
    /// </summary>
    public CaroGame.Protocol.Messages.UserProfileDto? GetUserProfile(string username)
    {
        try
        {
            DataTable dt = _userRepo.GetUserByUsername(username);
            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new CaroGame.Protocol.Messages.UserProfileDto
                {
                    Username = username,
                    Score = row.Table.Columns.Contains("Score") && row["Score"] != DBNull.Value ? Convert.ToInt32(row["Score"]) : 0,
                    Wins = row.Table.Columns.Contains("Wins") && row["Wins"] != DBNull.Value ? Convert.ToInt32(row["Wins"]) : 0,
                    Losses = row.Table.Columns.Contains("Losses") && row["Losses"] != DBNull.Value ? Convert.ToInt32(row["Losses"]) : 0,
                    Draws = row.Table.Columns.Contains("Draws") && row["Draws"] != DBNull.Value ? Convert.ToInt32(row["Draws"]) : 0
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UserService Exception - GetUserProfile]: {ex.Message}");
        }
        return null;
    }
}

