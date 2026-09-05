namespace Shared.Models
{
    /// <summary>
    /// Đại diện cho một nước đi.
    /// </summary>
    public class Move
    {
        public string PlayerId { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }

        public CellState Piece { get; set; }

        public string Symbol { get; set; }

        public int MoveNumber { get; set; }

        public Move()
        {
            PlayerId = string.Empty;
            Symbol = string.Empty;
            Piece = CellState.Empty;
        }

        public Move(
            string playerId,
            int row,
            int column,
            CellState piece,
            int moveNumber)
        {
            PlayerId = playerId ?? string.Empty;

            Row = row;

            Column = column;

            Piece = piece;

            Symbol = piece switch
            {
                CellState.X => "X",
                CellState.O => "O",
                _ => ""
            };

            MoveNumber = moveNumber;
        }
    }
}