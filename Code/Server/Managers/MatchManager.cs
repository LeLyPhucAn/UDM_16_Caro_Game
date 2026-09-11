using System;
using System.Collections.Generic;
using System.Linq;
using Server.Services;
using Shared.Models;


namespace Server.Managers
{
    /// <summary>
    /// Quản lý vòng đời và logic của các trận Caro.
    /// </summary>
    public sealed class MatchManager : IDisposable
    {
        private readonly Dictionary<string, Match> matches = new();

        // GameResult chính thức dùng Shared.Enums.GameResultType
        private readonly Dictionary<string, Shared.Models.GameResult> gameResults = new();

        private readonly GameRuleService ruleService;
        private readonly GameTimerService timerService;

        private readonly object syncRoot = new();

        private bool disposed;

        private const int DefaultTurnTimeSeconds = 30;

        // =========================================================
        // EVENT: Thông báo ra ngoài khi một trận hết giờ
        // =========================================================
        /// <summary>
        /// Được kích hoạt khi người chơi hết giờ.
        /// Args: Match, winnerId (string), winnerName (string)
        /// </summary>
        public event Action<Match, string, string>? OnMatchTimeout;

        public MatchManager()
        {
            ruleService = new GameRuleService();

            // GameTimerService yêu cầu TimeSpan
            timerService = new GameTimerService(
                TimeSpan.FromSeconds(DefaultTurnTimeSeconds),
                HandleTimeout);
        }

        // =========================================================
        // CREATE MATCH
        // =========================================================

        public Match CreateMatch()
        {
            string matchId = Guid.NewGuid().ToString();

            return CreateMatch(matchId, string.Empty);
        }

        public Match CreateMatch(string matchId)
        {
            return CreateMatch(matchId, string.Empty);
        }

        public Match CreateMatch(
            string matchId,
            string roomId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
            {
                throw new ArgumentException(
                    "Match ID cannot be empty.",
                    nameof(matchId));
            }

            roomId ??= string.Empty;

            lock (syncRoot)
            {
                if (matches.ContainsKey(matchId))
                {
                    throw new InvalidOperationException(
                        $"Match '{matchId}' already exists.");
                }

                Match match = new Match(
                    matchId,
                    roomId);

                matches.Add(matchId, match);

                return match;
            }
        }

        /// <summary>
        /// Tạo match với 2 Player ngay lập tức (dùng bởi MessageHandler.HandleStartMatchAsync).
        /// </summary>
        public Match? CreateMatch(
            string roomId,
            Player playerX,
            Player playerO,
            int boardSize = 15)
        {
            if (string.IsNullOrWhiteSpace(roomId))
                throw new ArgumentException("Room ID cannot be empty.", nameof(roomId));

            if (playerX == null) throw new ArgumentNullException(nameof(playerX));
            if (playerO == null) throw new ArgumentNullException(nameof(playerO));

            lock (syncRoot)
            {
                // Dùng roomId làm matchId để tra cứu dễ hơn
                if (matches.ContainsKey(roomId))
                    return matches[roomId]; // trận đã tồn tại

                Match match = new Match(roomId, roomId);
                match.PlayerX = playerX;
                match.PlayerO = playerO;
                match.Board = new Board(boardSize, boardSize);

                matches.Add(roomId, match);
                return match;
            }
        }

        // =========================================================
        // CANCEL MATCH (hủy trận khi có người ngắt kết nối)
        // =========================================================

        /// <summary>
        /// Hủy trận đấu đang diễn ra (gọi khi có người disconnect).
        /// </summary>
        public bool CancelMatch(string matchId, string reason)
        {
            return EndMatch(
                matchId,
                winnerId: null,
                loserId: null,
                type: Shared.Enums.GameResultType.Abandoned,
                reason: reason);
        }

        // =========================================================
        // GET MATCH
        // =========================================================

        public Match? GetMatch(string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return null;

            lock (syncRoot)
            {
                matches.TryGetValue(
                    matchId,
                    out Match? match);

                return match;
            }
        }

        // =========================================================
        // ADD PLAYER
        // =========================================================

        public bool AddPlayer(
            string matchId,
            Player player)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return false;

            if (player == null)
                return false;

            lock (syncRoot)
            {
                if (!matches.TryGetValue(
                        matchId,
                        out Match? match))
                {
                    return false;
                }

                if (match.IsFinished())
                    return false;

                if (match.PlayerX != null &&
                    match.PlayerO != null)
                {
                    return false;
                }

                // Player.Id là int
                if (IsPlayerInMatch(
                        match,
                        player.Id.ToString()))
                {
                    return false;
                }

                if (match.PlayerX == null)
                {
                    match.PlayerX = player;
                }
                else
                {
                    match.PlayerO = player;
                }

                return true;
            }
        }

        // =========================================================
        // REMOVE PLAYER
        // =========================================================

        public bool RemovePlayer(
            string matchId,
            string playerId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return false;

            if (string.IsNullOrWhiteSpace(playerId))
                return false;

            lock (syncRoot)
            {
                if (!matches.TryGetValue(
                        matchId,
                        out Match? match))
                {
                    return false;
                }

                bool removed = false;

                if (match.PlayerX?.Id.ToString() == playerId)
                {
                    match.PlayerX = null;
                    removed = true;
                }

                if (match.PlayerO?.Id.ToString() == playerId)
                {
                    match.PlayerO = null;
                    removed = true;
                }

                if (!removed)
                    return false;

                // Nếu trận đang chơi mà một người rời,
                // kết thúc trận.
                if (match.IsPlaying())
                {
                    string? opponentId =
                        GetOpponentId(
                            match,
                            playerId);

                    EndMatch(
                        matchId,
                        opponentId,
                        playerId,
                        Shared.Enums.GameResultType.Abandoned,
                        "Player left the match.");
                }

                return true;
            }
        }

        // =========================================================
        // START MATCH
        // =========================================================

        public bool StartMatch(string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return false;

            lock (syncRoot)
            {
                if (!matches.TryGetValue(
                        matchId,
                        out Match? match))
                {
                    return false;
                }

                if (!match.HasTwoPlayers())
                    return false;

                if (match.IsPlaying())
                    return false;

                if (match.IsFinished())
                    return false;

                if (!match.Start())
                    return false;

                // Timer chỉ nhận matchId
                timerService.StartOrReset(matchId);

                return true;
            }
        }

        // =========================================================
        // TRY MAKE MOVE
        // =========================================================

        /// <summary>
        /// Overload tiện lợi: nhận playerId và tọa độ riêng biệt thay vì Move object.
        /// Gọi bởi GameRequestHandler.
        /// </summary>
        public MoveResult TryMakeMove(
            string matchId,
            string playerId,
            int row,
            int column)
        {
            var move = new Move
            {
                PlayerId = playerId,
                Row = row,
                Column = column
            };
            return TryMakeMove(matchId, move);
        }

        public MoveResult TryMakeMove(
            string matchId,
            Move move)
        {
            if (string.IsNullOrWhiteSpace(matchId))
            {
                return InvalidResult(
                    MoveValidationResult.MatchNotPlaying,
                    "Match ID is invalid.");
            }

            if (move == null)
            {
                return InvalidResult(
                    MoveValidationResult.InvalidPosition,
                    "Move cannot be null.");
            }

            lock (syncRoot)
            {
                if (!matches.TryGetValue(
                        matchId,
                        out Match? match))
                {
                    return InvalidResult(
                        MoveValidationResult.MatchNotPlaying,
                        "Match not found.");
                }

                if (match.PlayerX == null ||
                    match.PlayerO == null)
                {
                    return InvalidResult(
                        MoveValidationResult.InvalidPlayer,
                        "Match does not have two players.");
                }

                // Player.Id trong Player là int,
                // còn GameRuleService dùng string ID.
                string playerXId =
                    match.PlayerX.Id.ToString();

                string playerOId =
                    match.PlayerO.Id.ToString();

                MoveResult result =
                    ruleService.ApplyMove(
                        match.Board,
                        move.PlayerId,
                        playerXId,
                        playerOId,
                        match.CurrentTurn,
                        move.Row,
                        move.Column,
                        match.IsPlaying());

                if (!result.IsValid)
                    return result;

                // Nước đi hợp lệ
                match.IncrementMoveCount();

                // Cập nhật Piece của Move nếu cần
                move.Piece = result.Piece;

                // Tắt timer của lượt cũ
                timerService.Stop(matchId);

                // =================================================
                // WIN
                // =================================================

                if (result.IsWin)
                {
                    string winnerId =
                        result.WinnerId ??
                        move.PlayerId;

                    string? loserId =
                        result.LoserId ??
                        GetOpponentId(
                            match,
                            winnerId);

                    match.End(winnerId);

                    gameResults[matchId] =
                        new Shared.Models.GameResult(
                            match.MatchId,
                            Shared.Enums.GameResultType.Win,
                            winnerId,
                            loserId,
                            "Player completed five consecutive pieces.");

                    return result;
                }

                // =================================================
                // DRAW
                // =================================================

                if (result.IsDraw)
                {
                    match.End();

                    gameResults[matchId] =
                        new Shared.Models.GameResult(
                            match.MatchId,
                            Shared.Enums.GameResultType.Draw,
                            null,
                            null,
                            "Board is full and there is no winner.");

                    return result;
                }

                // =================================================
                // CONTINUE MATCH
                // =================================================

                match.ChangeTurn();

                // Timer cho người chơi tiếp theo
                timerService.StartOrReset(matchId);

                return result;
            }
        }

        // =========================================================
        // MAKE MOVE
        // =========================================================

        public MoveResult MakeMove(
            string matchId,
            Move move)
        {
            return TryMakeMove(
                matchId,
                move);
        }

        // =========================================================
        // TIMEOUT
        // =========================================================

        private void HandleTimeout(
            string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return;

            lock (syncRoot)
            {
                if (!matches.TryGetValue(
                        matchId,
                        out Match? match))
                {
                    return;
                }

                if (!match.IsPlaying())
                    return;

                Player? currentPlayer =
                    match.GetCurrentPlayer();

                if (currentPlayer == null)
                    return;

                string loserId =
                    currentPlayer.Id.ToString();

                string? winnerId =
                    GetOpponentId(
                        match,
                        loserId);

                match.End(winnerId);

                gameResults[matchId] =
                    new Shared.Models.GameResult(
                        match.MatchId,
                        Shared.Enums.GameResultType.Timeout,
                        winnerId,
                        loserId,
                        "The current player ran out of time.");

                // Fire event để TcpServer gửi thông báo cho clients
                string winnerName = winnerId != null
                    ? (match.PlayerX?.Id == winnerId ? match.PlayerX?.Username : match.PlayerO?.Username) ?? string.Empty
                    : string.Empty;

                // Lưu lại match và winnerId trước khi unlock để fire event ngoài lock
                var capMatch = match;
                var capWinnerId = winnerId ?? string.Empty;
                var capWinnerName = winnerName;

                // Fire ngoài lock để tránh deadlock
                System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                    OnMatchTimeout?.Invoke(capMatch, capWinnerId, capWinnerName));
            }
        }

        // =========================================================
        // GET GAME RESULT
        // =========================================================

        public Shared.Models.GameResult? GetGameResult(
            string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return null;

            lock (syncRoot)
            {
                gameResults.TryGetValue(
                    matchId,
                    out Shared.Models.GameResult? result);

                return result;
            }
        }

        // =========================================================
        // TIMER
        // =========================================================

        public int GetRemainingSeconds(
            string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return 0;

            return timerService.GetRemainingSeconds(
                matchId);
        }

        public bool IsTimerRunning(
            string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return false;

            return timerService.IsRunning(
                matchId);
        }

        public void StopTimer(
            string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return;

            timerService.Stop(matchId);
        }

        public void ResetTimer(
            string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return;

            lock (syncRoot)
            {
                if (!matches.TryGetValue(
                        matchId,
                        out Match? match))
                {
                    return;
                }

                if (!match.IsPlaying())
                    return;

                timerService.StartOrReset(matchId);
            }
        }

        // =========================================================
        // END MATCH
        // =========================================================

        public bool EndMatch(
            string matchId,
            string? winnerId = null,
            string? loserId = null,
            Shared.Enums.GameResultType type =
                Shared.Enums.GameResultType.Abandoned,
            string? reason = null)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return false;

            lock (syncRoot)
            {
                if (!matches.TryGetValue(
                        matchId,
                        out Match? match))
                {
                    return false;
                }

                timerService.Stop(matchId);

                if (!match.IsFinished())
                {
                    match.End(winnerId);
                }

                gameResults[matchId] =
                    new Shared.Models.GameResult(
                        match.MatchId,
                        type,
                        winnerId,
                        loserId,
                        reason);

                return true;
            }
        }

        // =========================================================
        // RESET MATCH
        // =========================================================

        public bool ResetMatch(string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return false;

            lock (syncRoot)
            {
                if (!matches.TryGetValue(
                        matchId,
                        out Match? match))
                {
                    return false;
                }

                timerService.Stop(matchId);

                match.Reset();

                gameResults.Remove(matchId);

                return true;
            }
        }

        // =========================================================
        // REMOVE MATCH
        // =========================================================

        public bool RemoveMatch(string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return false;

            lock (syncRoot)
            {
                timerService.Stop(matchId);

                gameResults.Remove(matchId);

                return matches.Remove(matchId);
            }
        }

        // =========================================================
        // FIND PLAYER MATCH
        // =========================================================

        public Match? FindPlayerMatch(
            string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
                return null;

            lock (syncRoot)
            {
                return matches.Values.FirstOrDefault(
                    match =>
                        IsPlayerInMatch(
                            match,
                            playerId));
            }
        }

        // =========================================================
        // FIND ROOM MATCH
        // =========================================================

        public Match? FindRoomMatch(
            string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
                return null;

            lock (syncRoot)
            {
                return FindRoomMatchInternal(
                    roomId);
            }
        }

        private Match? FindRoomMatchInternal(
            string roomId)
        {
            return matches.Values.FirstOrDefault(
                match =>
                    string.Equals(
                        match.RoomId,
                        roomId,
                        StringComparison.OrdinalIgnoreCase));
        }

        // =========================================================
        // MATCH EXISTS
        // =========================================================

        public bool MatchExists(
            string matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId))
                return false;

            lock (syncRoot)
            {
                return matches.ContainsKey(
                    matchId);
            }
        }

        // =========================================================
        // GET ALL MATCHES
        // =========================================================

        public List<Match> GetAllMatches()
        {
            lock (syncRoot)
            {
                return matches.Values.ToList();
            }
        }

        // =========================================================
        // GET PLAYING MATCHES
        // =========================================================

        public List<Match> GetPlayingMatches()
        {
            lock (syncRoot)
            {
                return matches.Values
                    .Where(
                        match => match.IsPlaying())
                    .ToList();
            }
        }

        // =========================================================
        // GET MATCH COUNT
        // =========================================================

        public int GetMatchCount()
        {
            lock (syncRoot)
            {
                return matches.Count;
            }
        }

        // =========================================================
        // HELPERS
        // =========================================================

        private static bool IsPlayerInMatch(
            Match match,
            string playerId)
        {
            if (match == null)
                return false;

            if (string.IsNullOrWhiteSpace(playerId))
                return false;

            return match.PlayerX?.Id.ToString() == playerId ||
                   match.PlayerO?.Id.ToString() == playerId;
        }

        private static string? GetOpponentId(
            Match match,
            string playerId)
        {
            if (match == null)
                return null;

            if (match.PlayerX?.Id.ToString() == playerId)
            {
                return match.PlayerO?.Id.ToString();
            }

            if (match.PlayerO?.Id.ToString() == playerId)
            {
                return match.PlayerX?.Id.ToString();
            }

            return null;
        }

        private static MoveResult InvalidResult(
            MoveValidationResult validationResult,
            string message)
        {
            // IsValid là readonly.
            // Chỉ cần thiết lập Result.
            return new MoveResult
            {
                Result = validationResult,
                Row = -1,
                Column = -1,
                Piece = CellState.Empty,
                Message = message
            };
        }

        // =========================================================
        // DISPOSE
        // =========================================================

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

            timerService.Dispose();
        }
    }
}