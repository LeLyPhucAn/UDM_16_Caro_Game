namespace GameLogic.Models
{
    public class Move
    {
        public int PlayerId { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }

        public string Symbol { get; set; }

        public int MoveNumber { get; set; }

        public Move()
        {
            Symbol = string.Empty;
        }

        public Move(
            int playerId,
            int row,
            int column,
            string symbol,
            int moveNumber)
        {
            PlayerId = playerId;
            Row = row;
            Column = column;
            Symbol = symbol;
            MoveNumber = moveNumber;
        }
    }
}