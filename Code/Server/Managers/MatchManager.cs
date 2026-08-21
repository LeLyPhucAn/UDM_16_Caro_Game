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
                return null;

            if (playerX == null ||
                playerO == null)
            {
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
        public int GetMatchCount()
        {
            return matches.Count;
        }
    }
}