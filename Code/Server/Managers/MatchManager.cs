using System;
using System.Collections.Generic;
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
            if (string.IsNullOrWhiteSpace(roomId))
            {
                return null;
            }

            lock (syncRoot)
            {
                Match? existing =
                    FindRoomMatchInternal(roomId);


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

                match.PlayerO = playerO;

                matches.Add(
                    matchId,
                    match);

                return match;
            }
        }

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