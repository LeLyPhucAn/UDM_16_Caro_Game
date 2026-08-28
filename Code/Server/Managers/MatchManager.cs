using GameLogic.Models;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Data;
using Server.Services;

namespace Server.Managers
{
    // ==============================
    // TRẠNG THÁI TRẬN ĐẤU
    // ==============================
    public enum MatchStatus
    {
        Waiting,
        Playing,
        Finished
    }

    // ==============================
    // MATCH
    // ==============================
    public class Match
    {
        public string MatchId { get; private set; } // ID quản lý tạm thời trong RAM (Guid)
        public int DbMatchId { get; set; }          // ID lưu trữ trong Database (Primary Key)
        public string RoomId { get; private set; }
        public Player PlayerX { get; private set; }
        public Player PlayerO { get; private set; }
        public Board Board { get; private set; }
        public CellState CurrentTurn { get; private set; }
        public MatchStatus Status { get; private set; }
        public string? WinnerId { get; private set; }

        public Match(
            string matchId,
            string roomId,
            Player playerX,
            Player playerO)
        {
            MatchId = matchId;
            DbMatchId = -1; // Mặc định -1 nếu chưa lưu được vào DB
            RoomId = roomId;

            PlayerX = playerX;
            PlayerO = playerO;

            // Bàn cờ Caro 15x15
            Board = new Board(15, 15);

            // X đi trước
            CurrentTurn = CellState.X;

            Status = MatchStatus.Waiting;

            WinnerId = null;
        }

        public bool Start()
        {
            if (PlayerX == null || PlayerO == null)
                return false;

            if (Status != MatchStatus.Waiting)
                return false;

            Board.Reset();
            CurrentTurn = CellState.X;
            WinnerId = null;
            Status = MatchStatus.Playing;

            Console.WriteLine($"[MATCH] Started: {MatchId}");
            Console.WriteLine($"[MATCH] X: {PlayerX.PlayerName}");
            Console.WriteLine($"[MATCH] O: {PlayerO.PlayerName}");

            return true;
        }

        public bool MakeMove(string playerId, int row, int column)
        {
            if (Status != MatchStatus.Playing)
                return false;

            if (string.IsNullOrWhiteSpace(playerId))
                return false;

            CellState playerPiece;

            if (PlayerX.PlayerId == playerId)
                playerPiece = CellState.X;
            else if (PlayerO.PlayerId == playerId)
                playerPiece = CellState.O;
            else
                return false;

            if (playerPiece != CurrentTurn)
            {
                Console.WriteLine($"[MATCH] Wrong turn: {playerId}");
                return false;
            }

            if (!Board.IsValidPosition(row, column))
            {
                Console.WriteLine($"[MATCH] Invalid position: ({row},{column})");
                return false;
            }

            if (!Board.IsEmpty(row, column))
            {
                Console.WriteLine($"[MATCH] Cell occupied: ({row},{column})");
                return false;
            }

            bool placed = Board.PlacePiece(row, column, playerPiece);
            if (!placed) return false;

            Console.WriteLine($"[MATCH] {playerId} placed {playerPiece} at ({row},{column})");

            // Kiểm tra thắng
            if (Board.CheckWin(row, column))
            {
                WinnerId = playerId;
                Status = MatchStatus.Finished;
                Console.WriteLine($"[MATCH] Winner: {playerId}");
                return true;
            }

            // Kiểm tra hòa
            if (Board.IsFull())
            {
                Status = MatchStatus.Finished;
                WinnerId = null;
                Console.WriteLine($"[MATCH] Draw: {MatchId}");
                return true;
            }

            SwitchTurn();
            return true;
        }

        private void SwitchTurn()
        {
            CurrentTurn = (CurrentTurn == CellState.X) ? CellState.O : CellState.X;
            Console.WriteLine($"[MATCH] Current turn: {CurrentTurn}");
        }

        public string GetCurrentPlayerId() => CurrentTurn == CellState.X ? PlayerX.PlayerId : PlayerO.PlayerId;
        public Player GetCurrentPlayer() => CurrentTurn == CellState.X ? PlayerX : PlayerO;

        public void EndMatch()
        {
            Status = MatchStatus.Finished;
            Console.WriteLine($"[MATCH] Finished: {MatchId}");
        }

        public void ResetMatch()
        {
            Board.Reset();
            CurrentTurn = CellState.X;
            WinnerId = null;
            Status = MatchStatus.Waiting;
            Console.WriteLine($"[MATCH] Reset: {MatchId}");
        }

        public bool IsPlaying() => Status == MatchStatus.Playing;
        public bool IsFinished() => Status == MatchStatus.Finished;
        public bool IsDraw() => Status == MatchStatus.Finished && WinnerId == null && Board.IsFull();
    }

    // ==============================
    // MATCH MANAGER (TÍCH HỢP DB)
    // ==============================
    public class MatchManager
    {
        private readonly Dictionary<string, Match> matches;
        private readonly MatchService _matchService;

        public MatchManager()
        {
            matches = new Dictionary<string, Match>();
            _matchService = new MatchService();
        }

        // Constructor dùng cho Dependency Injection / Unit Test
        public MatchManager(MatchService matchService)
        {
            matches = new Dictionary<string, Match>();
            _matchService = matchService ?? new MatchService();
        }

        // ==============================
        // TẠO MATCH
        // ==============================
        public Match? CreateMatch(string roomId, Player playerX, Player playerO)
        {
            if (string.IsNullOrWhiteSpace(roomId) || playerX == null || playerO == null)
                return null;

            if (playerX.PlayerId == playerO.PlayerId)
            {
                Console.WriteLine("[MATCH] Cannot create match: same player.");
                return null;
            }

            string matchId = Guid.NewGuid().ToString();
            Match match = new Match(matchId, roomId, playerX, playerO);

            matches.Add(matchId, match);
            Console.WriteLine($"[MATCH] Created: {matchId}");

            return match;
        }

        // ==============================
        // BẮT ĐẦU MATCH (TÍCH HỢP DB)
        // ==============================
        public bool StartMatch(string matchId)
        {
            Match? match = GetMatch(matchId);
            if (match == null) return false;

            bool started = match.Start();
            if (!started) return false;

            // [TASK 2]: Lưu bản ghi Match mới vào Database khi trận bắt đầu
            try
            {
                // Chuyển PlayerId sang int nếu DB dùng kiểu INT
                int p1Id = int.TryParse(match.PlayerX.PlayerId, out int id1) ? id1 : 0;
                int p2Id = int.TryParse(match.PlayerO.PlayerId, out int id2) ? id2 : 0;

                match.DbMatchId = _matchService.StartNewMatch(p1Id, p2Id);
                Console.WriteLine($"[MATCH DB] Match saved to DB with ID: {match.DbMatchId}");
            }
            catch (Exception ex)
            {
                // An toàn: Không để lỗi DB làm Server bị crash
                Console.WriteLine($"[MATCH DB ERROR - StartMatch]: {ex.Message}");
            }

            return true;
        }

        // ==============================
        // THỰC HIỆN NƯỚC ĐI (TÍCH HỢP DB)
        // ==============================
        public bool MakeMove(string matchId, string playerId, int row, int column)
        {
            Match? match = GetMatch(matchId);
            if (match == null) return false;

            bool moveSuccess = match.MakeMove(playerId, row, column);

            // [TASK 2]: Nếu nước đi làm trận đấu kết thúc (Thắng hoặc Hòa), cập nhật DB
            if (moveSuccess && match.IsFinished())
            {
                SaveMatchResultToDb(match, match.WinnerId, match.IsDraw() ? "DRAW" : "WIN_NORMAL");
            }

            return moveSuccess;
        }

        // ==============================
        // KẾT THÚC MATCH (TÍCH HỢP DB)
        // ==============================
        public bool EndMatch(string matchId, string? winnerId = null, string resultReason = "ENDED_MANUALLY")
        {
            Match? match = GetMatch(matchId);
            if (match == null) return false;

            match.EndMatch();

            // [TASK 2]: Cập nhật kết quả trận đấu vào DB khi đầu hàng/rớt mạng
            SaveMatchResultToDb(match, winnerId ?? match.WinnerId, resultReason);

            return true;
        }

        // ==============================
        // HÀM BỌC LƯU DB AN TOÀN
        // ==============================
        private void SaveMatchResultToDb(Match match, string? winnerIdStr, string result)
        {
            if (match.DbMatchId <= 0) return; // Không có ID DB hợp lệ thì bỏ qua

            try
            {
                int? winnerId = null;
                if (!string.IsNullOrEmpty(winnerIdStr) && int.TryParse(winnerIdStr, out int wId))
                {
                    winnerId = wId;
                }

                _matchService.SaveMatchResult(match.DbMatchId, winnerId, result);
                Console.WriteLine($"[MATCH DB] Updated result for Match DB ID {match.DbMatchId}: Winner={winnerIdStr}, Result={result}");
            }
            catch (Exception ex)
            {
                // Bảo vệ Server không crash
                Console.WriteLine($"[MATCH DB ERROR - SaveResult]: {ex.Message}");
            }
        }

        // ==============================
        // TRUY VẤN LỊCH SỬ ĐẤU (TASK 2)
        // ==============================
        public DataTable GetPlayerHistory(string playerId)
        {
            try
            {
                if (int.TryParse(playerId, out int userId))
                {
                    return _matchService.GetUserMatchHistory(userId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MATCH DB ERROR - GetHistory]: {ex.Message}");
            }

            return new DataTable(); // Trả về bảng rỗng nếu lỗi/không tìm thấy
        }

        // ==============================
        // CÁC HÀM TIỆN ÍCH KHÁC
        // ==============================
        public Match? GetMatch(string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId)) return null;
            matches.TryGetValue(matchId, out Match? match);
            return match;
        }

        public bool ResetMatch(string matchId)
        {
            Match? match = GetMatch(matchId);
            if (match == null) return false;
            match.ResetMatch();
            return true;
        }

        public bool RemoveMatch(string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId) || !matches.ContainsKey(matchId))
                return false;

            matches.Remove(matchId);
            Console.WriteLine($"[MATCH] Removed: {matchId}");
            return true;
        }

        public Match? FindPlayerMatch(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId)) return null;

            foreach (Match match in matches.Values)
            {
                if (match.PlayerX.PlayerId == playerId || match.PlayerO.PlayerId == playerId)
                    return match;
            }
            return null;
        }

        public Match? FindRoomMatch(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId)) return null;

            foreach (Match match in matches.Values)
            {
                if (match.RoomId == roomId) return match;
            }
            return null;
        }

        public bool MatchExists(string matchId) => !string.IsNullOrWhiteSpace(matchId) && matches.ContainsKey(matchId);
        public List<Match> GetMatches() => new List<Match>(matches.Values);
        public int GetMatchCount() => matches.Count;
    }
}