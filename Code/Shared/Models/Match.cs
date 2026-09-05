using System;
using Shared.Models;

namespace GameLogic.Models
{
    public class Match
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        public Player Player1 { get; set; }

        /// <summary>
        /// ID của trận đấu lưu trong Database (Task 2)
        /// </summary>
        public int DbMatchId { get; set; }


        // =========================
        // NGƯỜI CHƠI
        // =========================

        public Player? PlayerX { get; set; }

        public Player? PlayerO { get; set; }


        // =========================
        // GAME STATE
        // =========================

        public Board Board { get; set; }

        public int CurrentPlayerId { get; set; }

        public string Status { get; set; }

        public int? WinnerId { get; set; }

        public Match()
        {
            MatchId = string.Empty;
            RoomId = string.Empty;
            DbMatchId = 0;

            Board = new Board();

            Status = "Waiting";

            WinnerId = null;
        }

        public Match(
            int id,
            int roomId,
            Player player1,
            Player player2)
        {
            Id = id;
            RoomId = roomId;

            Player1 = player1;
            Player2 = player2;

            DbMatchId = 0;

            PlayerX = null;
            PlayerO = null;

            CurrentPlayerId = player1.Id;

            Status = "Playing";

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
        /// Kiểm tra trận đấu hòa hay không.
        /// </summary>
        public bool IsDraw()
        {
            return State == MatchState.Finished && string.IsNullOrEmpty(WinnerId);
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


        public void EndMatch(string? winnerId = null)
        {
            End(winnerId);
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


        public void ResetMatch()
        {
            Reset();
        }
    }
}