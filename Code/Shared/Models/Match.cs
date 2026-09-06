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
    /// Model đại diện cho một trận Caro.
    /// </summary>
    public class Match
    {
        public string MatchId { get; set; }

        public string RoomId { get; set; }

        public Player? PlayerX { get; set; }

        public Player? PlayerO { get; set; }

        public Board Board { get; set; }

        public CellState CurrentTurn { get; set; }

        public MatchState State { get; set; }

        public string? WinnerId { get; set; }

        public int MoveCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        public Match()
        {
            MatchId = string.Empty;
            RoomId = string.Empty;

            PlayerX = null;
            PlayerO = null;

            Board = new Board();

            CurrentTurn = CellState.X;

            State = MatchState.Waiting;

            WinnerId = null;

            MoveCount = 0;

            CreatedAt = DateTime.UtcNow;

            StartedAt = null;
            FinishedAt = null;
        }

        public Match(
            string matchId,
            string roomId)
            : this()
        {
            MatchId = matchId ?? string.Empty;
            RoomId = roomId ?? string.Empty;
        }

        /// <summary>
        /// Match đã đủ 2 Player chưa.
        /// </summary>
        public bool HasTwoPlayers()
        {
            return PlayerX != null &&
                   PlayerO != null;
        }

        /// <summary>
        /// Lấy Player đang tới lượt.
        /// </summary>
        public Player? GetCurrentPlayer()
        {
            return CurrentTurn switch
            {
                CellState.X => PlayerX,
                CellState.O => PlayerO,
                _ => null
            };
        }

        /// <summary>
        /// Lấy ID Player đang tới lượt.
        /// </summary>
        public string? GetCurrentPlayerId()
        {
            return GetCurrentPlayer()?.Id.ToString();
        }

        /// <summary>
        /// Bắt đầu Match.
        /// </summary>
        public bool Start()
        {
            if (!HasTwoPlayers())
                return false;

            if (State != MatchState.Waiting)
                return false;

            Board.Reset();

            CurrentTurn = CellState.X;

            MoveCount = 0;

            WinnerId = null;

            State = MatchState.Playing;

            StartedAt = DateTime.UtcNow;

            FinishedAt = null;

            return true;
        }

        public bool IsPlaying()
        {
            return State == MatchState.Playing;
        }

        public bool IsFinished()
        {
            return State == MatchState.Finished;
        }

        /// <summary>
        /// Kết thúc Match.
        /// </summary>
        public void End(string? winnerId = null)
        {
            State = MatchState.Finished;

            WinnerId = winnerId;

            FinishedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Chuyển lượt.
        /// </summary>
        public bool ChangeTurn()
        {
            if (State != MatchState.Playing)
                return false;

            CurrentTurn =
                CurrentTurn == CellState.X
                    ? CellState.O
                    : CellState.X;

            return true;
        }

        public void IncrementMoveCount()
        {
            MoveCount++;
        }

        /// <summary>
        /// Reset Match nhưng giữ Player.
        /// </summary>
        public void Reset()
        {
            Board.Reset();

            CurrentTurn = CellState.X;

            State = MatchState.Waiting;

            WinnerId = null;

            MoveCount = 0;

            StartedAt = null;

            FinishedAt = null;
        }
    }
}