namespace Shared.Models
{
    /// <summary>
    /// Đại diện cho người chơi trong game.
    /// </summary>
    public class Player
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string DisplayName { get; set; }

        public bool IsOnline { get; set; }

        public Player()
        {
            Username = string.Empty;
            DisplayName = string.Empty;
        }

        public Player(
            string id,
            string username,
            string displayName = "")
        {
            Id = id;

            Username = username ?? string.Empty;

            DisplayName =
                string.IsNullOrWhiteSpace(displayName)
                    ? Username
                    : displayName;

            IsOnline = false;
        }
    }
}