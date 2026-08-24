using System;
using System.Data;
using Server.Repository;

namespace Server.Services
{
    public class MatchService
    {
        private readonly MatchRepository _matchRepository;
        private readonly HistoryRepository _historyRepository;

        public MatchService(MatchRepository matchRepository, HistoryRepository historyRepository)
        {
            _matchRepository = matchRepository;
            _historyRepository = historyRepository;
        }

        // Bắt đầu trận đấu mới
        public int StartNewMatch(int player1Id, int player2Id)
        {
            return _matchRepository.CreateMatch(player1Id, player2Id, DateTime.Now);
        }

        // Hoàn tất trận đấu và lưu thông tin vào Database
        public bool SaveMatchResult(int matchId, int? winnerId, string result)
        {
            return _matchRepository.EndMatch(matchId, winnerId, result, DateTime.Now);
        }

        // Lấy danh sách lịch sử đấu của User
        public DataTable GetUserMatchHistory(int userId)
        {
            return _historyRepository.GetMatchHistoryByUserId(userId);
        }
    }
}