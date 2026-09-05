using GameLogic.Models;
using Shared.Models;
using System;
using System.Collections.Generic;


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
        public string MatchId { get; private set; }

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

        // ==============================
        // BẮT ĐẦU TRẬN
        // ==============================
        public bool Start()
        {
            if (PlayerX == null ||
                PlayerO == null)
            {
                return false;
            }

            if (Status != MatchStatus.Waiting)
                return false;

            Board.Reset();

            CurrentTurn = CellState.X;

            WinnerId = null;

            Status = MatchStatus.Playing;

            Console.WriteLine(
                $"[MATCH] Started: {MatchId}");

            Console.WriteLine(
                $"[MATCH] X: {PlayerX.PlayerName}");

            Console.WriteLine(
                $"[MATCH] O: {PlayerO.PlayerName}");

            return true;
        }

        // ==============================
        // ĐẶT QUÂN
        // ==============================
        public bool MakeMove(
            string playerId,
            int row,
            int column)
        {
            // Trận chưa bắt đầu
            if (Status != MatchStatus.Playing)
                return false;

            // Player ID không hợp lệ
            if (string.IsNullOrWhiteSpace(playerId))
                return false;

            CellState playerPiece;

            // ==========================
            // XÁC ĐỊNH QUÂN
            // ==========================

            if (PlayerX.PlayerId == playerId)
            {
                playerPiece = CellState.X;
            }
            else if (PlayerO.PlayerId == playerId)
            {
                playerPiece = CellState.O;
            }
            else
            {
                // Không phải người chơi
                return false;
            }

            // ==========================
            // KIỂM TRA LƯỢT
            // ==========================

            if (playerPiece != CurrentTurn)
            {
                Console.WriteLine(
                    $"[MATCH] Wrong turn: {playerId}");

                return false;
            }

            // ==========================
            // KIỂM TRA VỊ TRÍ
            // ==========================

            if (!Board.IsValidPosition(row, column))
            {
                Console.WriteLine(
                    $"[MATCH] Invalid position: ({row},{column})");

                return false;
            }

            // ==========================
            // KIỂM TRA Ô TRỐNG
            // ==========================

            if (!Board.IsEmpty(row, column))
            {
                Console.WriteLine(
                    $"[MATCH] Cell already occupied: ({row},{column})");

                return false;
            }

            // ==========================
            // ĐẶT QUÂN
            // ==========================

            bool placed = Board.PlacePiece(
                row,
                column,
                playerPiece);

            if (!placed)
                return false;

            Console.WriteLine(
                $"[MATCH] {playerId} placed " +
                $"{playerPiece} at ({row},{column})");

            // ==========================
            // KIỂM TRA THẮNG
            // ==========================

            if (Board.CheckWin(row, column))
            {
                WinnerId = playerId;

                Status = MatchStatus.Finished;

                Console.WriteLine(
                    $"[MATCH] Winner: {playerId}");

                return true;
            }

            // ==========================
            // KIỂM TRA HÒA
            // ==========================

            if (Board.IsFull())
            {
                Status = MatchStatus.Finished;

                WinnerId = null;

                Console.WriteLine(
                    $"[MATCH] Draw: {MatchId}");

                return true;
            }

            // ==========================
            // ĐỔI LƯỢT
            // ==========================

            SwitchTurn();

            return true;
        }

        // ==============================
        // ĐỔI LƯỢT
        // ==============================
        private void SwitchTurn()
        {
            if (CurrentTurn == CellState.X)
            {
                CurrentTurn = CellState.O;
            }
            else
            {
                CurrentTurn = CellState.X;
            }

            Console.WriteLine(
                $"[MATCH] Current turn: {CurrentTurn}");
        }

        // ==============================
        // LẤY PLAYER ĐANG ĐẾN LƯỢT
        // ==============================
        public string GetCurrentPlayerId()
        {
            if (CurrentTurn == CellState.X)
            {
                return PlayerX.PlayerId;
            }

            return PlayerO.PlayerId;
        }

        // ==============================
        // LẤY PLAYER ĐANG ĐẾN LƯỢT
        // ==============================
        public Player GetCurrentPlayer()
        {
            if (CurrentTurn == CellState.X)
            {
                return PlayerX;
            }

            return PlayerO;
        }

        // ==============================
        // KẾT THÚC TRẬN
        // ==============================
        public void EndMatch()
        {
            Status = MatchStatus.Finished;

            Console.WriteLine(
                $"[MATCH] Finished: {MatchId}");
        }

        // ==============================
        // RESET TRẬN
        // ==============================
        public void ResetMatch()
        {
            Board.Reset();

            CurrentTurn = CellState.X;

            WinnerId = null;

            Status = MatchStatus.Waiting;

            Console.WriteLine(
                $"[MATCH] Reset: {MatchId}");
        }

        // ==============================
        // KIỂM TRA TRẬN ĐANG CHƠI
        // ==============================
        public bool IsPlaying()
        {
            return Status == MatchStatus.Playing;
        }

        // ==============================
        // KIỂM TRA TRẬN ĐÃ KẾT THÚC
        // ==============================
        public bool IsFinished()
        {
            return Status == MatchStatus.Finished;
        }

        // ==============================
        // KIỂM TRA HÒA
        // ==============================
        public bool IsDraw()
        {
            return Status == MatchStatus.Finished &&
                   WinnerId == null &&
                   Board.IsFull();
        }
    }

    // ==============================
    // MATCH MANAGER
    // ==============================
    public class MatchManager
    {
        private readonly Dictionary<string, Match> matches;

        public MatchManager()
        {
            matches = new Dictionary<string, Match>();
        }

        // ==============================
        // TẠO MATCH
        // ==============================
        public Match? CreateMatch(
            string roomId,
            Player playerX,
            Player playerO)
        {
            if (string.IsNullOrWhiteSpace(roomId))

using System.Data;
using System.Linq;
using Server.Services;
using Shared.Models;

namespace Server.Managers
{
    /// <summary>
    /// Quản lý danh sách và vòng đời các trận đấu trong Server.
    /// </summary>
    public class MatchManager
    {
        private readonly Dictionary<string, Match> matches = new Dictionary<string, Match>();
        private readonly Dictionary<string, int> matchDbIds = new Dictionary<string, int>(); // Lưu trữ DbMatchId tạm thời trong RAM
        private readonly MatchService _matchService;
        private readonly GameRuleService ruleService;
        private readonly object syncRoot = new object();

        public MatchManager()
        {
            _matchService = new MatchService();
            ruleService = new GameRuleService();
        }

        public MatchManager(MatchService matchService)
        {
            _matchService = matchService ?? new MatchService();
            ruleService = new GameRuleService();
        }

        // =====================================================
        // TẠO MATCH
        // =====================================================
        public Match? CreateMatch(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
                return null;

            lock (syncRoot)
            {
                Match? existing = FindRoomMatchInternal(roomId);
                if (existing != null)
                    return null;

                string matchId = Guid.NewGuid().ToString();
                Match match = new Match(matchId, roomId);
                matches.Add(matchId, match);
                return match;
            }
        }

        public Match? CreateMatch(string roomId, Player playerX, Player playerO)
        {
            if (string.IsNullOrWhiteSpace(roomId) || playerX == null || playerO == null)

                return null;

            if (string.IsNullOrWhiteSpace(playerX.Id) || string.IsNullOrWhiteSpace(playerO.Id))
                return null;

            }

            // Không cho cùng Player đấu với chính mình
            if (playerX.PlayerId == playerO.PlayerId)
            {
                Console.WriteLine(
                    "[MATCH] Cannot create match: " +
                    "same player.");

                return null;
            }

            string matchId = Guid.NewGuid().ToString();

            Match match = new Match(
                matchId,
                roomId,
                playerX,
                playerO);

            matches.Add(
                matchId,
                match);

            Console.WriteLine(
                $"[MATCH] Created: {matchId}");

            return match;
        }

        // ==============================
        // LẤY MATCH
        // ==============================
        public Match? GetMatch(string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return null;

            matches.TryGetValue(
                matchId,
                out Match? match);

            return match;
        }

        // ==============================
        // BẮT ĐẦU MATCH
        // ==============================


            if (playerX.Id == playerO.Id)
            {
                Console.WriteLine("[MATCH] Cannot create match: same player.");
                return null;
            }

            lock (syncRoot)
            {
                if (FindRoomMatchInternal(roomId) != null)
                    return null;

                string matchId = Guid.NewGuid().ToString();
                Match match = new Match(matchId, roomId)
                {
                    PlayerX = playerX,
                    PlayerO = playerO
                };

                matches.Add(matchId, match);
                Console.WriteLine($"[MATCH] Created: {matchId}");
                return match;
            }
        }

        // =====================================================
        // QUẢN LÝ NGƯỜI CHƠI TRONG MATCH
        // =====================================================
        public bool AddPlayer(string matchId, Player player)
        {
            if (player == null || string.IsNullOrWhiteSpace(player.Id))
                return false;

            Match? match = GetMatch(matchId);
            if (match == null)
                return false;

            lock (syncRoot)
            {
                if (match.State != MatchState.Waiting)
                    return false;

                if (match.PlayerX != null && match.PlayerX.Id == player.Id)
                    return false;

                if (match.PlayerO != null && match.PlayerO.Id == player.Id)
                    return false;

                if (match.PlayerX == null)
                {
                    match.PlayerX = player;
                    return true;
                }

                if (match.PlayerO == null)
                {
                    match.PlayerO = player;
                    return true;
                }

                return false;
            }
        }

        public bool RemovePlayer(string matchId, string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
                return false;

            Match? match = GetMatch(matchId);
            if (match == null)
                return false;

            lock (syncRoot)
            {
                if (match.State == MatchState.Playing)
                    return false;

                if (match.PlayerX != null && match.PlayerX.Id == playerId)
                {
                    match.PlayerX = null;
                    return true;
                }

                if (match.PlayerO != null && match.PlayerO.Id == playerId)
                {
                    match.PlayerO = null;
                    return true;
                }

                return false;
            }
        }

        // =====================================================
        // BẮT ĐẦU MATCH (TÍCH HỢP DB)
        // =====================================================

        public bool StartMatch(string matchId)
        {
            Match? match = GetMatch(matchId);


            if (match == null)
                return false;

            return match.Start();
        }

        // ==============================
        // THỰC HIỆN NƯỚC ĐI
        // ==============================
        public bool MakeMove(
            string matchId,
            string playerId,
            int row,
            int column)
        {
            Match? match = GetMatch(matchId);

            if (match == null)
                return false;

            return match.MakeMove(
                playerId,
                row,
                column);
        }

        // ==============================
        // KẾT THÚC MATCH
        // ==============================
        public bool EndMatch(string matchId)
        {
            Match? match = GetMatch(matchId);

            if (match == null)
                return false;

            match.EndMatch();

            return true;
        }

        // ==============================
        // RESET MATCH
        // ==============================
        public bool ResetMatch(string matchId)
        {
            Match? match = GetMatch(matchId);

            if (match == null)
                return false;

            match.ResetMatch();

            return true;
        }

        // ==============================
        // XÓA MATCH
        // ==============================
        public bool RemoveMatch(string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return false;

            if (!matches.ContainsKey(matchId))
                return false;

            matches.Remove(matchId);

            Console.WriteLine(
                $"[MATCH] Removed: {matchId}");

            return true;
        }

        // ==============================
        // TÌM MATCH CỦA PLAYER
        // ==============================
        public Match? FindPlayerMatch(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
                return null;

            foreach (Match match in matches.Values)
            {
                if (match.PlayerX.PlayerId == playerId ||
                    match.PlayerO.PlayerId == playerId)
                {
                    return match;
                }
            }

            return null;
        }

        // ==============================
        // TÌM MATCH THEO ROOM
        // ==============================
        public Match? FindRoomMatch(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
                return null;

            foreach (Match match in matches.Values)
            {
                if (match.RoomId == roomId)
                {
                    return match;
                }
            }

            return null;
        }

        // ==============================
        // KIỂM TRA MATCH TỒN TẠI
        // ==============================
        public bool MatchExists(string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return false;

            return matches.ContainsKey(matchId);
        }

        // ==============================
        // LẤY DANH SÁCH MATCH
        // ==============================
        public List<Match> GetMatches()
        {
            return new List<Match>(matches.Values);
        }

        // ==============================
        // LẤY SỐ MATCH
        // ==============================

            lock (syncRoot)
            {
                if (match.State != MatchState.Waiting || !match.HasTwoPlayers())
                    return false;

                match.Board.Reset();
                match.CurrentTurn = CellState.X;
                match.WinnerId = null;
                match.MoveCount = 0;
                match.State = MatchState.Playing;

                // [TASK 2]: Lưu bản ghi trận đấu mới vào DB
                try
                {
                    int p1Id = int.TryParse(match.PlayerX?.Id, out int id1) ? id1 : 0;
                    int p2Id = int.TryParse(match.PlayerO?.Id, out int id2) ? id2 : 0;

                    int dbMatchId = _matchService.StartNewMatch(p1Id, p2Id);
                    matchDbIds[matchId] = dbMatchId;
                    Console.WriteLine($"[MATCH DB] Match saved to DB with ID: {dbMatchId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[MATCH DB ERROR - StartMatch]: {ex.Message}");
                }

                return true;
            }
        }

        // =====================================================
        // THỰC HIỆN NƯỚC ĐI (TÍCH HỢP DB)
        // =====================================================
        public MoveResult TryMakeMove(string matchId, string playerId, int row, int column)
        {
            Match? match = GetMatch(matchId);
            if (match == null)
            {
                return new MoveResult
                {
                    Result = MoveValidationResult.MatchNotPlaying,
                    Message = "Match not found."
                };
            }

            lock (syncRoot)
            {
                if (match.State == MatchState.Finished)
                {
                    return new MoveResult
                    {
                        Result = MoveValidationResult.GameOver,
                        Message = "Game is already over."
                    };
                }

                if (match.State != MatchState.Playing)
                {
                    return new MoveResult
                    {
                        Result = MoveValidationResult.MatchNotPlaying,
                        Message = "Match has not started."
                    };
                }

                if (!match.HasTwoPlayers())
                {
                    return new MoveResult
                    {
                        Result = MoveValidationResult.InvalidPlayer,
                        Message = "Match does not have two players."
                    };
                }

                string playerXId = match.PlayerX!.Id;
                string playerOId = match.PlayerO!.Id;

                MoveResult result = ruleService.ApplyMove(
                    match.Board,
                    playerId,
                    playerXId,
                    playerOId,
                    match.CurrentTurn,
                    row,
                    column,
                    true);

                if (!result.IsValid)
                    return result;

                match.MoveCount++;

                if (result.IsWin)
                {
                    match.WinnerId = playerId;
                    match.State = MatchState.Finished;
                    SaveMatchResultToDb(matchId, playerId, "WIN_NORMAL");
                    return result;
                }

                if (result.IsDraw)
                {
                    match.WinnerId = null;
                    match.State = MatchState.Finished;
                    SaveMatchResultToDb(matchId, null, "DRAW");
                    return result;
                }

                // Đổi lượt chơi
                match.CurrentTurn = (match.CurrentTurn == CellState.X) ? CellState.O : CellState.X;
                return result;
            }
        }

        public bool MakeMove(string matchId, string playerId, int row, int column)
        {
            MoveResult result = TryMakeMove(matchId, playerId, row, column);
            return result.IsValid;
        }

        // =====================================================
        // KẾT THÚC / RESET / XÓA MATCH
        // =====================================================
        public bool EndMatch(string matchId, string? winnerId = null, string resultReason = "ENDED_MANUALLY")
        {
            Match? match = GetMatch(matchId);
            if (match == null) return false;

            lock (syncRoot)
            {
                if (winnerId != null)
                {
                    bool validWinner = (match.PlayerX != null && match.PlayerX.Id == winnerId) ||
                                       (match.PlayerO != null && match.PlayerO.Id == winnerId);
                    if (!validWinner) return false;
                }

                match.WinnerId = winnerId;
                match.State = MatchState.Finished;

                SaveMatchResultToDb(matchId, winnerId, resultReason);
                return true;
            }
        }

        public bool ResetMatch(string matchId)
        {
            Match? match = GetMatch(matchId);
            if (match == null) return false;

            lock (syncRoot)
            {
                match.Board.Reset();
                match.CurrentTurn = CellState.X;
                match.WinnerId = null;
                match.MoveCount = 0;
                match.State = MatchState.Waiting;
                return true;
            }
        }

        public bool RemoveMatch(string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId)) return false;

            lock (syncRoot)
            {
                matchDbIds.Remove(matchId);
                bool removed = matches.Remove(matchId);
                if (removed)
                {
                    Console.WriteLine($"[MATCH] Removed: {matchId}");
                }
                return removed;
            }
        }

        // =====================================================
        // LƯU DB AN TOÀN
        // =====================================================
        private void SaveMatchResultToDb(string matchId, string? winnerIdStr, string result)
        {
            if (!matchDbIds.TryGetValue(matchId, out int dbMatchId) || dbMatchId <= 0)
                return;

            try
            {
                int? winnerId = null;
                if (!string.IsNullOrEmpty(winnerIdStr) && int.TryParse(winnerIdStr, out int wId))
                {
                    winnerId = wId;
                }

                _matchService.SaveMatchResult(dbMatchId, winnerId, result);
                Console.WriteLine($"[MATCH DB] Updated result for Match DB ID {dbMatchId}: Winner={winnerIdStr}, Result={result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MATCH DB ERROR - SaveResult]: {ex.Message}");
            }
        }

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

            return new DataTable();
        }

        // =====================================================
        // TRUY VẤN TIỆN ÍCH
        // =====================================================
        public Match? GetMatch(string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId)) return null;

            lock (syncRoot)
            {
                matches.TryGetValue(matchId, out Match? match);
                return match;
            }
        }

        public Match? FindPlayerMatch(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId)) return null;

            lock (syncRoot)
            {
                return matches.Values.FirstOrDefault(match =>
                    (match.PlayerX != null && match.PlayerX.Id == playerId) ||
                    (match.PlayerO != null && match.PlayerO.Id == playerId));
            }
        }

        public Match? FindRoomMatch(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId)) return null;

            lock (syncRoot)
            {
                return FindRoomMatchInternal(roomId);
            }
        }

        private Match? FindRoomMatchInternal(string roomId)
        {
            return matches.Values.FirstOrDefault(match => match.RoomId == roomId);
        }

        public bool MatchExists(string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId)) return false;

            lock (syncRoot)
            {
                return matches.ContainsKey(matchId);
            }
        }

        public List<Match> GetAllMatches()
        {
            lock (syncRoot)
            {
                return matches.Values.ToList();
            }
        }

        public List<Match> GetPlayingMatches()
        {
            lock (syncRoot)
            {
                return matches.Values.Where(match => match.State == MatchState.Playing).ToList();
            }
        }


        public int GetMatchCount()
        {
            return matches.Count;
        }
    }
}