using Server.Utils;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Server.Repositories;

namespace Server.Services;

public class UserService
{
    private readonly UserRepository _userRepo = new();

    /// <summary>
    /// Băm mật khẩu bằng thuật toán SHA-256 an toàn
    /// </summary>
    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password)) return string.Empty;
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

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
                Logger.Warn($"[UserService]: Tên đăng nhập '{username}' đã tồn tại.");
                return false;
            }

            // Băm mật khẩu bằng SHA-256 trước khi lưu vào CSDL
            string passwordHash = HashPassword(password);
            int rows = _userRepo.InsertUser(username, passwordHash);
            return rows > 0;
        }
        catch (Exception ex)
        {
            Logger.Error("[UserService Exception - Register]", ex);
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
            string passwordHash = HashPassword(password);
            DataTable dt = _userRepo.ValidateUser(username, passwordHash);
            if (dt != null && dt.Rows.Count > 0)
            {
                userRow = dt.Rows[0];
                return true;
            }

            // Hỗ trợ tương thích ngược: nếu tài khoản cũ trong CSDL chưa được băm SHA-256
            DataTable dtOld = _userRepo.ValidateUser(username, password);
            if (dtOld != null && dtOld.Rows.Count > 0)
            {
                userRow = dtOld.Rows[0];
                // Tự động nâng cấp mật khẩu cũ sang dạng băm SHA-256
                _userRepo.UpdatePassword(username, passwordHash);
                Logger.Info($"[UserService]: Đã tự động nâng cấp mật khẩu sang SHA-256 cho tài khoản '{username}'.");
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Logger.Error("[UserService Exception - Login]", ex);
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
            Logger.Error("[UserService Exception - GetUserById]", ex);
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
            Logger.Error("[UserService Exception - GetAllUsers]", ex);
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
        catch (Exception ex)
        {
            Logger.Error("[UserService Exception - GetUserId]", ex);
        }
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
            Logger.Error("[UserService Exception - GetUserProfile]", ex);
        }
        return null;
    }
}
