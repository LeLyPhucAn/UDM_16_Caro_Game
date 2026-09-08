using Shared.Models;

namespace Server.Services
{
    public enum MoveValidationResult
    {
        Valid,
        MatchNotPlaying,
        GameOver,
        InvalidPlayer,
        WrongTurn,
        InvalidPosition,
        CellOccupied
    }

    /// <summary>
    /// Kết quả xử lý một nước đi.
    /// </summary>
    public class MoveResult
    {
        public MoveValidationResult Result { get; set; }

        public bool IsValid =>
            Result == MoveValidationResult.Valid;

        public bool IsWin { get; set; }

        public bool IsDraw { get; set; }

        public string? WinnerId { get; set; }

        public string? LoserId { get; set; }

        public CellState Piece { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }

        public string Message { get; set; }

        public MoveResult()
        {
            Result = MoveValidationResult.Valid;

            Piece = CellState.Empty;

            Message = string.Empty;

            Row = -1;
            Column = -1;
        }
    }

    /// <summary>
    /// GameResult lưu kết quả cuối cùng của Match.
    /// </summary>
    public enum GameResultType
    {
        Win,
        Draw,
        Timeout,
        Abandoned
    }

    public class GameResult
    {
        public string MatchId { get; }

        public GameResultType ResultType { get; }

        public string? WinnerId { get; }

        public string? LoserId { get; }

        public string Reason { get; }

        public System.DateTime FinishedAt { get; }

        public GameResult(
            string matchId,
            GameResultType resultType,
            string? winnerId,
            string? loserId,
            string reason)
        {
            MatchId = matchId;

            ResultType = resultType;

            WinnerId = winnerId;

            LoserId = loserId;

            Reason = reason;

            FinishedAt = System.DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Xử lý luật Caro phía Server.
    /// </summary>
    public class GameRuleService
    {
        public const int WinLength = 5;

        /// <summary>
        /// Validate một nước đi.
        /// </summary>
        public MoveValidationResult ValidateMove(
            Board board,
            string playerId,
            string playerXId,
            string playerOId,
            CellState currentTurn,
            int row,
            int column,
            bool matchPlaying)
        {
            if (!matchPlaying)
            {
                return MoveValidationResult.MatchNotPlaying;
            }

            if (string.IsNullOrWhiteSpace(playerId))
            {
                return MoveValidationResult.InvalidPlayer;
            }

            CellState piece =
                GetPlayerPiece(
                    playerId,
                    playerXId,
                    playerOId);

            if (piece == CellState.Empty)
            {
                return MoveValidationResult.InvalidPlayer;
            }

            if (piece != currentTurn)
            {
                return MoveValidationResult.WrongTurn;
            }

            if (!board.IsValidPosition(row, column))
            {
                return MoveValidationResult.InvalidPosition;
            }

            if (!board.IsEmpty(row, column))
            {
                return MoveValidationResult.CellOccupied;
            }

            return MoveValidationResult.Valid;
        }

        /// <summary>
        /// Validate và đặt quân.
        /// </summary>
        public MoveResult ApplyMove(
            Board board,
            string playerId,
            string playerXId,
            string playerOId,
            CellState currentTurn,
            int row,
            int column,
            bool matchPlaying)
        {
            MoveValidationResult validation =
                ValidateMove(
                    board,
                    playerId,
                    playerXId,
                    playerOId,
                    currentTurn,
                    row,
                    column,
                    matchPlaying);

            if (validation !=
                MoveValidationResult.Valid)
            {
                return new MoveResult
                {
                    Result = validation,
                    Row = row,
                    Column = column,
                    Message = GetMessage(validation)
                };
            }

            CellState piece =
                GetPlayerPiece(
                    playerId,
                    playerXId,
                    playerOId);

            bool placed =
                board.PlacePiece(
                    row,
                    column,
                    piece);

            if (!placed)
            {
                return new MoveResult
                {
                    Result =
                        MoveValidationResult.CellOccupied,

                    Row = row,
                    Column = column,

                    Message =
                        "Cell is already occupied."
                };
            }

            bool win =
                CheckWin(
                    board,
                    row,
                    column);

            bool draw =
                !win &&
                CheckDraw(board);

            return new MoveResult
            {
                Result =
                    MoveValidationResult.Valid,

                IsWin = win,

                IsDraw = draw,

                WinnerId =
                    win ? playerId : null,

                Piece = piece,

                Row = row,

                Column = column,

                Message =
                    win
                        ? "Player wins."
                        : draw
                            ? "Match draw."
                            : "Move accepted."
            };
        }

        /// <summary>
        /// Xác định quân X/O của Player.
        /// </summary>
        public CellState GetPlayerPiece(
            string playerId,
            string playerXId,
            string playerOId)
        {
            if (playerId == playerXId)
                return CellState.X;

            if (playerId == playerOId)
                return CellState.O;

            return CellState.Empty;
        }

        /// <summary>
        /// Kiểm tra thắng tại vị trí vừa đánh.
        /// </summary>
        public bool CheckWin(
            Board board,
            int row,
            int column)
        {
            if (!board.IsValidPosition(row, column))
                return false;

            CellState piece =
                board.GetCell(row, column);

            if (piece == CellState.Empty)
                return false;

            return CountLine(
                       board,
                       row,
                       column,
                       0,
                       1,
                       piece) >= WinLength
                   ||
                   CountLine(
                       board,
                       row,
                       column,
                       1,
                       0,
                       piece) >= WinLength
                   ||
                   CountLine(
                       board,
                       row,
                       column,
                       1,
                       1,
                       piece) >= WinLength
                   ||
                   CountLine(
                       board,
                       row,
                       column,
                       1,
                       -1,
                       piece) >= WinLength;
        }

        /// <summary>
        /// Kiểm tra hòa.
        /// </summary>
        public bool CheckDraw(Board board)
        {
            return board.IsFull();
        }

        private int CountLine(
            Board board,
            int row,
            int column,
            int rowDirection,
            int columnDirection,
            CellState piece)
        {
            int count = 1;

            count += CountDirection(
                board,
                row,
                column,
                rowDirection,
                columnDirection,
                piece);

            count += CountDirection(
                board,
                row,
                column,
                -rowDirection,
                -columnDirection,
                piece);

            return count;
        }

        private int CountDirection(
            Board board,
            int row,
            int column,
            int rowDirection,
            int columnDirection,
            CellState piece)
        {
            int count = 0;

            int currentRow =
                row + rowDirection;

            int currentColumn =
                column + columnDirection;

            while (
                board.IsValidPosition(
                    currentRow,
                    currentColumn)
                &&
                board.GetCell(
                    currentRow,
                    currentColumn) == piece)
            {
                count++;

                currentRow += rowDirection;

                currentColumn += columnDirection;
            }

            return count;
        }

        public string GetMessage(
            MoveValidationResult result)
        {
            return result switch
            {
                MoveValidationResult.Valid =>
                    "Move accepted.",

                MoveValidationResult.MatchNotPlaying =>
                    "Match has not started.",

                MoveValidationResult.GameOver =>
                    "Game is already over.",

                MoveValidationResult.InvalidPlayer =>
                    "Player is not in this match.",

                MoveValidationResult.WrongTurn =>
                    "It is not this player's turn.",

                MoveValidationResult.InvalidPosition =>
                    "Position is outside the board.",

                MoveValidationResult.CellOccupied =>
                    "Cell is already occupied.",

                _ =>
                    "Invalid move."
            };
        }
    }
}