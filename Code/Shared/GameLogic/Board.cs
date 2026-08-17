namespace GameLogic.Models
{
    public class Board
    {
        public int Rows { get; set; }

        public int Columns { get; set; }

        public string[,] Cells { get; set; }

        public Board()
        {
            Rows = 15;
            Columns = 15;
            Cells = new string[Rows, Columns];
        }

        public Board(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Cells = new string[Rows, Columns];
        }
    }
}