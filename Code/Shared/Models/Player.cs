namespace Shared.Models
{
    /// <summary>
    /// Đại diện cho người chơi trong game.
    /// </summary>
    public class Player
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string DisplayName { get; set; }

        public bool IsOnline { get; set; }

        public Player()
        {
            Username = string.Empty;
            DisplayName = string.Empty;
        }

        public Player(
            int id,
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