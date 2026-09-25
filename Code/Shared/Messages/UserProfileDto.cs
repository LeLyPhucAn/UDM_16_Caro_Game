using System;

namespace CaroGame.Protocol.Messages
{
    public class UserProfileDto
    {
        public string Username { get; set; } = string.Empty;
        public int Score { get; set; } = 0;
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int Draws { get; set; } = 0;

        public int TotalMatches => Wins + Losses + Draws;
        public double WinRate => TotalMatches > 0 ? Math.Round((double)Wins / TotalMatches * 100, 1) : 0;
    }
}
