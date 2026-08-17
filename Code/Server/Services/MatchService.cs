using System.Data;
using CaroGame.Repository;

namespace CaroGame.Services
{
    public class MatchService
    {
        private readonly MatchRepository _matchRepo = new MatchRepository();

        public DataTable GetHistory(int userId)
        {
            return _matchRepo.GetMatchHistory(userId);
        }

        public void RecordMatch(int p1, int p2, int winner)
        {
            _matchRepo.SaveMatch(p1, p2, winner);
        }
    }
}