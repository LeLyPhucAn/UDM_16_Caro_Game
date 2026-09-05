using System;
using System.Data;
using Server.Repository;

namespace Server.Services
{
    public class MatchService
    {
        private readonly MatchRepository _matchRepository;
        private readonly HistoryRepository _historyRepository;

        /// <summary>
        /// Constructor mặc định (Khởi tạo tự động các Repository nếu không dùng DI container)
        /// </summary>
        public MatchService() 
            : this(new MatchRepository(), new HistoryRepository())
        {
        }

        /// <summary>
        /// Constructor nhận Dependency Injection
        /// </summary>
        public MatchService(MatchRepository matchRepository, HistoryRepository historyRepository)
        {
            _matchRepository = matchRepository ?? throw new ArgumentNullException(nameof(matchRepository));
            _historyRepository = historyRepository ?? throw new ArgumentNullException(nameof(historyRepository));
        }

        /// <summary>
        /// 1. Tạo Match Record mới khi trận đấu bắt đầu (Lưu Player1, Player2, StartTime, Status)
        /// </summary>
        public int StartNewMatch(int player1Id, int player2Id)
        {
            // Kiểm tra dữ liệu đầu vào (Validation)
            if (player1Id <= 0 || player2Id <= 0)
            {
                Console.WriteLine("[MatchService Warning]: ID người chơi không hợp lệ.");
                return -1;
            }

            if (player1Id == player2Id)
            {
                Console.WriteLine("[MatchService Warning]: Hai người chơi không được trùng ID.");
                return -1;
            }

            try
            {
                Console.WriteLine($"[MatchService]: Đang khởi tạo trận đấu giữa Player {player1Id} và Player {player2Id}...");
                return _matchRepository.CreateMatch(player1Id, player2Id, DateTime.Now);
            }
            catch (Exception ex)
            {
                // Bắt lỗi Database để không làm ngắt kết nối Game của người chơi
                Console.WriteLine($"[MatchService DB Exception - StartNewMatch]: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// 2. Lưu kết quả trận đấu khi kết thúc (Lưu Winner, Result, EndTime, Cập nhật State)
        /// </summary>
        public bool SaveMatchResult(int matchId, int? winnerId, string result)
        {
            // Kiểm tra dữ liệu trước khi Update
            if (matchId <= 0)
            {
                Console.WriteLine("[MatchService Warning]: MatchID không hợp lệ.");
                return false;
            }

            try
            {
                Console.WriteLine($"[MatchService]: Cập nhật kết quả Match #{matchId} (WinnerId: {winnerId?.ToString() ?? "Hòa/Hủy"}, Result: {result})...");
                return _matchRepository.EndMatch(matchId, winnerId, result, DateTime.Now);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MatchService DB Exception - SaveMatchResult]: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 3. Xử lý khi trận đấu bị hủy đột ngột (Thoát game, ngắt kết nối)
        /// </summary>
        public bool CancelMatch(int matchId, string reason)
        {
            if (matchId <= 0) return false;

            try
            {
                Console.WriteLine($"[MatchService]: Hủy trận #{matchId}. Lý do: {reason}");
                return _matchRepository.EndMatch(matchId, null, $"CANCELLED: {reason}", DateTime.Now);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MatchService DB Exception - CancelMatch]: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 4. Truy vấn thông tin Match chi tiết theo MatchID
        /// </summary>
        public DataTable GetMatchById(int matchId)
        {
            if (matchId <= 0)
            {
                Console.WriteLine("[MatchService Warning]: MatchID không hợp lệ.");
                return new DataTable();
            }

            try
            {
                return _matchRepository.GetMatchById(matchId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MatchService DB Exception - GetMatchById]: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// 5. Truy vấn lịch sử đấu (History) theo UserId
        /// </summary>
        public DataTable GetUserMatchHistory(int userId)
        {
            if (userId <= 0)
            {
                Console.WriteLine("[MatchService Warning]: UserId không hợp lệ.");
                return new DataTable();
            }

            try
            {
                return _historyRepository.GetMatchHistoryByUserId(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MatchService DB Exception - GetUserMatchHistory]: {ex.Message}");
                return new DataTable();
            }
        }
    }
}