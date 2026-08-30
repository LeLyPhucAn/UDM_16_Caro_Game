using System;
using System.Collections.Generic;

using System.Data;
using Server.Services;
using System.Linq;
using Server.Services;
using Shared.Models;


namespace Server.Managers
{
    /// <summary>
    /// Trạng thái của một trận đấu.
    /// </summary>
    public enum MatchStatus
    {
        Waiting,

        Playing,

        Finished
    }

    /// <summary>
    /// Quản lý vòng đời của một trận đấu.
    /// </summary>
    public class MatchManager
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

        private readonly Dictionary<string, Match> matches;

        private readonly GameRuleService ruleService;

        private readonly object syncRoot;

        public MatchManager()
        {
            matches =
                new Dictionary<string, Match>();

            ruleService =
                new GameRuleService();

            syncRoot =
                new object();
        }

        // =====================================================
        // CREATE MATCH
        // =====================================================


        public Match? CreateMatch(
            string roomId)
        {

            MatchId = matchId;
            DbMatchId = -1; // Mặc định -1 nếu chưa lưu được vào DB
            RoomId = roomId;

            if (string.IsNullOrWhiteSpace(roomId))
            {
                return null;
            }

            lock (syncRoot)
            {
                Match? existing =
                    FindRoomMatchInternal(roomId);




            // Bàn cờ Caro 15x15
            Board = new Board(10,10);
              

                if (existing != null)
                {
                    return null;
                }

                string matchId =
                    Guid.NewGuid().ToString();

                Match match =
                    new Match(
                        matchId,
                        roomId);

                matches.Add(
                    matchId,
                    match);

                return match;
            }
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

        // =====================================================
        // CREATE MATCH WITH 2 PLAYERS
        // =====================================================

        public Match? CreateMatch(
            string roomId,
            Player playerX,
            Player playerO)
        {
            if (string.IsNullOrWhiteSpace(roomId))
            {
                return null;
            }

            if (playerX == null ||
                playerO == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(playerX.Id) ||
                string.IsNullOrWhiteSpace(playerO.Id))
            {
                return null;
            }

            if (playerX.Id == playerO.Id)
            {
                return null;
            }

            lock (syncRoot)
            {
                if (FindRoomMatchInternal(
                        roomId) != null)
                {
                    return null;
                }

                string matchId =
                    Guid.NewGuid().ToString();

                Match match =
                    new Match(
                        matchId,
                        roomId);


                match.PlayerX = playerX;


        public bool MakeMove(string playerId, int row, int column)
        {
            if (Status != MatchStatus.Playing)
                return false;

            if (string.IsNullOrWhiteSpace(playerId))
                return false;

                match.PlayerO = playerO;

                matches.Add(
                    matchId,
                    match);

                return match;
            }
        }


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

        // =====================================================
        // GET MATCH
        // =====================================================

        public Match? GetMatch(
            string matchId)
        {
            if (string.IsNullOrWhiteSpace(
                    matchId))
            {
                return null;
            }

            lock (syncRoot)
            {
                if (matches.TryGetValue(
                        matchId,
                        out Match? match))
                {
                    return match;
                }

                return null;

            }
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

        // =====================================================
        // ADD PLAYER
        // =====================================================

        public bool AddPlayer(
            string matchId,
            Player player)
        {
            if (player == null)
            {
                return false;
            }

            Match? match =
                GetMatch(matchId);

            if (match == null)
            {
                return false;
            }

            lock (syncRoot)
            {
                if (match.State !=
                    MatchState.Waiting)
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(player.Id))
                {
                    return false;
                }

                if (match.PlayerX != null &&
                    match.PlayerX.Id == player.Id)
                {
                    return false;
                }

                if (match.PlayerO != null &&
                    match.PlayerO.Id == player.Id)
                {
                    return false;
                }

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

        // =====================================================
        // REMOVE PLAYER
        // =====================================================


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

        public bool RemovePlayer(
            string matchId,
            string playerId)
        {
            Match? match =
                GetMatch(matchId);

            if (match == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(playerId))
            {
                return false;
            }

            lock (syncRoot)
            {
                if (match.State ==
                    MatchState.Playing)
                {
                    return false;
                }

                if (match.PlayerX != null &&
                    match.PlayerX.Id == playerId)
                {
                    match.PlayerX = null;

                    return true;
                }

                if (match.PlayerO != null &&
                    match.PlayerO.Id == playerId)
                {
                    match.PlayerO = null;

                    return true;
                }

                return false;
            }
        }

        // =====================================================
        // START MATCH
        // =====================================================

        public bool StartMatch(
            string matchId)
        {
            Match? match =
                GetMatch(matchId);

            if (match == null)
            {
                return false;
            }

            lock (syncRoot)
            {
                if (match.State !=
                    MatchState.Waiting)
                {
                    return false;
                }

                if (!match.HasTwoPlayers())
                {
                    return false;
                }

                match.Board.Reset();

                match.CurrentTurn =
                    CellState.X;

                match.WinnerId =
                    null;

                match.MoveCount =
                    0;

                match.State =
                    MatchState.Playing;

                return true;
            }
        }

        // =====================================================
        // MAKE MOVE
        // =====================================================

        public MoveResult TryMakeMove(
            string matchId,
            string playerId,
            int row,
            int column)
        {
            Match? match =
                GetMatch(matchId);

            if (match == null)
            {
                return new MoveResult
                {
                    Result =
                        MoveValidationResult
                            .MatchNotPlaying,

                    Message =
                        "Match not found."
                };
            }

            lock (syncRoot)
            {
                // -------------------------
                // Game Over
                // -------------------------

                if (match.State ==
                    MatchState.Finished)
                {
                    return new MoveResult
                    {
                        Result =
                            MoveValidationResult
                                .GameOver,

                        Message =
                            "Game is already over."
                    };
                }

                // -------------------------
                // Chưa bắt đầu
                // -------------------------

                if (match.State !=
                    MatchState.Playing)
                {
                    return new MoveResult
                    {
                        Result =
                            MoveValidationResult
                                .MatchNotPlaying,

                        Message =
                            "Match has not started."
                    };
                }

                // -------------------------
                // Kiểm tra Player
                // -------------------------

                if (!match.HasTwoPlayers())
                {
                    return new MoveResult
                    {
                        Result =
                            MoveValidationResult
                                .InvalidPlayer,

                        Message =
                            "Match does not have two players."
                    };
                }

                string playerXId =
                    match.PlayerX!.Id;

                string playerOId =
                    match.PlayerO!.Id;

                // -------------------------
                // Validate + Apply Move
                // -------------------------

                MoveResult result =
                    ruleService.ApplyMove(
                        match.Board,
                        playerId,
                        playerXId,
                        playerOId,
                        match.CurrentTurn,
                        row,
                        column,
                        true);

                if (!result.IsValid)
                {
                    return result;
                }

                // -------------------------
                // Tạo Move
                // -------------------------

                match.MoveCount++;

                Move move =
                    new Move(
                        playerId,
                        row,
                        column,
                        result.Piece,
                        match.MoveCount);

                // -------------------------
                // Win
                // -------------------------

                if (result.IsWin)
                {
                    match.WinnerId =
                        playerId;

                    match.State =
                        MatchState.Finished;

                    return result;
                }


                // -------------------------
                // Draw
                // -------------------------


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

                if (result.IsDraw)
                {
                    match.WinnerId =
                        null;

                    match.State =
                        MatchState.Finished;

                    return result;
                }

                // -------------------------
                // Change Turn
                // -------------------------

                if (match.CurrentTurn ==
                    CellState.X)
                {
                    match.CurrentTurn =
                        CellState.O;
                }
                else
                {
                    match.CurrentTurn =
                        CellState.X;
                }

                return result;
            }
        }

        // =====================================================
        // SIMPLE MAKE MOVE
        // =====================================================

        public bool MakeMove(
            string matchId,
            string playerId,
            int row,
            int column)
        {
            MoveResult result =
                TryMakeMove(
                    matchId,
                    playerId,
                    row,
                    column);

            return result.IsValid;
        }

        // =====================================================
        // END MATCH
        // =====================================================

        public bool EndMatch(
            string matchId,
            string? winnerId = null)
        {
            Match? match =
                GetMatch(matchId);

            if (match == null)
            {
                return false;
            }

            lock (syncRoot)
            {
                if (winnerId != null)
                {
                    bool validWinner =
                        match.PlayerX != null &&
                        match.PlayerX.Id == winnerId;

                    bool validWinner2 =
                        match.PlayerO != null &&
                        match.PlayerO.Id == winnerId;

                    if (!validWinner &&
                        !validWinner2)
                    {
                        return false;
                    }
                }

                match.WinnerId =
                    winnerId;

                match.State =
                    MatchState.Finished;

                return true;
            }
        }

        // =====================================================
        // RESET MATCH
        // =====================================================

        public bool ResetMatch(
            string matchId)
        {
            Match? match =
                GetMatch(matchId);

            if (match == null)
            {
                return false;
            }

            lock (syncRoot)
            {
                match.Board.Reset();

                match.CurrentTurn =
                    CellState.X;

                match.WinnerId =
                    null;

                match.MoveCount =
                    0;

                match.State =
                    MatchState.Waiting;

                return true;
            }
        }

        // =====================================================
        // REMOVE MATCH
        // =====================================================

        public bool RemoveMatch(
            string matchId)
        {
            if (string.IsNullOrWhiteSpace(
                    matchId))
            {
                return false;
            }

            lock (syncRoot)
            {
                return matches.Remove(
                    matchId);
            }
        }

        // =====================================================
        // FIND BY PLAYER
        // =====================================================

        public Match? FindPlayerMatch(
            string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
            {
                return null;
            }


            lock (syncRoot)
            {

                if (match.PlayerX.PlayerId == playerId || match.PlayerO.PlayerId == playerId)
                    return match;
            }
            return null;
        }

        public Match? FindRoomMatch(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId)) return null;

                return matches.Values
                    .FirstOrDefault(
                        match =>
                            (match.PlayerX != null &&
                             match.PlayerX.Id == playerId)
                            ||
                            (match.PlayerO != null &&
                             match.PlayerO.Id == playerId));
            }
        }

        // =====================================================
        // FIND BY ROOM
        // =====================================================

        public Match? FindRoomMatch(
            string roomId)
        {
            if (string.IsNullOrWhiteSpace(
                    roomId))
            {
                return null;
            }


            lock (syncRoot)
            {

                if (match.RoomId == roomId) return match;
            }
            return null;
        }

        public bool MatchExists(string matchId) => !string.IsNullOrWhiteSpace(matchId) && matches.ContainsKey(matchId);
        public List<Match> GetMatches() => new List<Match>(matches.Values);
        public int GetMatchCount() => matches.Count;

                return FindRoomMatchInternal(
                    roomId);
            }
        }

        private Match?
            FindRoomMatchInternal(
                string roomId)
        {
            return matches.Values
                .FirstOrDefault(
                    match =>
                        match.RoomId == roomId);
        }

        // =====================================================
        // EXISTS
        // =====================================================

        public bool MatchExists(
            string matchId)
        {
            lock (syncRoot)
            {
                return matches.ContainsKey(
                    matchId);
            }
        }

        // =====================================================
        // GET ALL
        // =====================================================

        public List<Match> GetAllMatches()
        {
            lock (syncRoot)
            {
                return matches.Values
                    .ToList();
            }
        }

        // =====================================================
        // GET PLAYING MATCHES
        // =====================================================

        public List<Match>
            GetPlayingMatches()
        {
            lock (syncRoot)
            {
                return matches.Values
                    .Where(
                        match =>
                            match.State ==
                            MatchState.Playing)
                    .ToList();
            }
        }

        // =====================================================
        // COUNT
        // =====================================================

        public int GetMatchCount()
        {
            lock (syncRoot)
            {
                return matches.Count;
            }
        }

    }
}