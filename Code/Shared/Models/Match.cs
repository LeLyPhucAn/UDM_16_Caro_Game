using System;

namespace Shared.Models
{
    public enum MatchState
    {
        Waiting,
        Playing,
        Finished
    }

    /// <summary>
    /// Model đại diện cho một trận đấu Caro.
    /// </summary>
    public class Match
    {
        // =========================
        // THÔNG TIN MATCH
        // =========================

        public string MatchId { get; set; }

        public string RoomId { get; set; }


        // =========================
        // NGƯỜI CHƠI
        // =========================

        public Player? PlayerX { get; set; }

        public Player? PlayerO { get; set; }


        // =========================
        // GAME STATE
        // =========================

        public Board Board { get; set; }

        public CellState CurrentTurn { get; set; }

        public MatchState State { get; set; }


        // =========================
        // KẾT QUẢ
        // =========================

        public string? WinnerId { get; set; }

        public int MoveCount { get; set; }


        // =========================
        // THỜI GIAN
        // =========================

        public DateTime CreatedAt { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }


        // =========================
        // CONSTRUCTOR MẶC ĐỊNH
        // =========================

        public Match()
        {
            MatchId = string.Empty;
            RoomId = string.Empty;

            PlayerX = null;
            PlayerO = null;

            Board = new Board(15, 15);

            CurrentTurn = CellState.X;

            State = MatchState.Waiting;

            WinnerId = null;

            MoveCount = 0;

            CreatedAt = DateTime.Now;

            StartedAt = null;
            FinishedAt = null;
        }


        // =========================
        // CONSTRUCTOR
        // =========================

        public Match(
            string matchId,
            string roomId)
        {
            MatchId = matchId ?? string.Empty;

            RoomId = roomId ?? string.Empty;

            PlayerX = null;
            PlayerO = null;

            Board = new Board(15, 15);

            CurrentTurn = CellState.X;

            State = MatchState.Waiting;

            WinnerId = null;

            MoveCount = 0;

            CreatedAt = DateTime.Now;

            StartedAt = null;
            FinishedAt = null;
        }


        // =========================
        // PLAYER
        // =========================

        /// <summary>
        /// Kiểm tra Match đã có đủ 2 người chơi chưa.
        /// </summary>
        public bool HasTwoPlayers()
        {
            return PlayerX != null &&
                   PlayerO != null;
        }


        /// <summary>
        /// Lấy người chơi đang tới lượt.
        /// </summary>
        public Player? GetCurrentPlayer()
        {
            if (CurrentTurn == CellState.X)
            {
                return PlayerX;
            }

            if (CurrentTurn == CellState.O)
            {
                return PlayerO;
            }

            return null;
        }


        /// <summary>
        /// Lấy Id của người chơi đang tới lượt.
        /// </summary>
        public string? GetCurrentPlayerId()
        {
            Player? player = GetCurrentPlayer();

            if (player == null)
            {
                return null;
            }

            return player.Id.ToString();
        }


        // =========================
        // MATCH STATE
        // =========================

        /// <summary>
        /// Bắt đầu trận đấu.
        /// </summary>
        public bool Start()
        {
            if (!HasTwoPlayers())
            {
                return false;
            }

            if (State != MatchState.Waiting)
            {
                return false;
            }

            State = MatchState.Playing;

            CurrentTurn = CellState.X;

            MoveCount = 0;

            WinnerId = null;

            StartedAt = DateTime.Now;

            FinishedAt = null;

            return true;
        }


        /// <summary>
        /// Kiểm tra trận đấu có đang diễn ra hay không.
        /// </summary>
        public bool IsPlaying()
        {
            return State == MatchState.Playing;
        }


        /// <summary>
        /// Kiểm tra trận đấu đã kết thúc hay chưa.
        /// </summary>
        public bool IsFinished()
        {
            return State == MatchState.Finished;
        }


        /// <summary>
        /// Kết thúc trận đấu.
        /// </summary>
        public void End(string? winnerId = null)
        {
            State = MatchState.Finished;

            WinnerId = winnerId;

            FinishedAt = DateTime.Now;
        }


        // =========================
        // TURN
        // =========================

        /// <summary>
        /// Chuyển lượt chơi.
        /// </summary>
        public void ChangeTurn()
        {
            if (CurrentTurn == CellState.X)
            {
                CurrentTurn = CellState.O;
            }
            else
            {
                CurrentTurn = CellState.X;
            }
        }


        // =========================
        // MOVE
        // =========================

        /// <summary>
        /// Tăng số lượt đánh.
        /// </summary>
        public void IncrementMoveCount()
        {
            MoveCount++;
        }


        // =========================
        // RESET
        // =========================

        /// <summary>
        /// Reset Match về trạng thái ban đầu.
        /// </summary>
        public void Reset()
        {
            Board = new Board(15, 15);

            CurrentTurn = CellState.X;

            State = MatchState.Waiting;

            WinnerId = null;

            MoveCount = 0;

            CreatedAt = DateTime.Now;

            StartedAt = null;

            FinishedAt = null;
        }
    }
}