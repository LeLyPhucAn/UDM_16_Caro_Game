namespace Shared.Models
{
    /// <summary>
    /// Người chơi trong game.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Session ID (string/Guid) dùng để định danh player ở runtime TCP.
        /// Khi tạo từ database, dùng DatabaseId để lưu UserId số nguyên.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// UserId trong database (int). Mặc định = 0 nếu chưa đăng nhập.
        /// </summary>
        public int DatabaseId { get; set; }

        public string Username { get; set; }

        public string DisplayName { get; set; }

        public bool IsOnline { get; set; }

        public Player()
        {
            Id = string.Empty;
            Username = string.Empty;
            DisplayName = string.Empty;
            IsOnline = false;
        }

        /// <summary>
        /// Constructor dùng cho runtime (SessionId là string/Guid).
        /// </summary>
        public Player(
            string id,
            string username,
            string displayName = "")
        {
            Id = id ?? string.Empty;

            Username =
                username ?? string.Empty;

            DisplayName =
                string.IsNullOrWhiteSpace(displayName)
                    ? Username
                    : displayName;

            IsOnline = false;
        }

        /// <summary>
        /// Constructor dùng cho database (UserId là int).
        /// </summary>
        public Player(
            int dbId,
            string username,
            string displayName = "")
        {
            DatabaseId = dbId;
            Id = dbId.ToString();

            Username =
                username ?? string.Empty;

            DisplayName =
                string.IsNullOrWhiteSpace(displayName)
                    ? Username
                    : displayName;

            IsOnline = false;
        }
    }
}