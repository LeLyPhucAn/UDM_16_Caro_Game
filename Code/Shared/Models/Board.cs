using System;

namespace Shared.Models
{
    // Trạng thái của một ô trên bàn cờ
    public enum CellState
    {
        Empty = 0,
        X = 1,
        O = 2
    }

    public class Board
    {
        // Kích thước bàn cờ
        public int Rows { get; private set; }
        public int Columns { get; private set; }

        // Ma trận bàn cờ
        private readonly CellState[,] cells;

        // Constructor mặc định: 15x15
        public Board()
            : this(15, 15)
        {
        }

        // Constructor tùy chỉnh
        public Board(int rows, int columns)
        {
            if (rows <= 0)
                throw new ArgumentException("Rows must be greater than 0.");

            if (columns <= 0)
                throw new ArgumentException("Columns must be greater than 0.");

            Rows = rows;
            Columns = columns;

            cells = new CellState[Rows, Columns];

            Reset();
        }

        // =====================================================
        // RESET BOARD
        // =====================================================

        public void Reset()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    cells[row, column] = CellState.Empty;
                }
            }
        }

        // =====================================================
        // KIỂM TRA VỊ TRÍ
        // =====================================================

        public bool IsValidPosition(int row, int column)
        {
            return row >= 0 &&
                   row < Rows &&
                   column >= 0 &&
                   column < Columns;
        }

        // =====================================================
        // KIỂM TRA Ô TRỐNG
        // =====================================================

        public bool IsEmpty(int row, int column)
        {
            if (!IsValidPosition(row, column))
                return false;

            return cells[row, column] == CellState.Empty;
        }

        // =====================================================
        // LẤY GIÁ TRỊ Ô
        // =====================================================

        public CellState GetCell(int row, int column)
        {
            if (!IsValidPosition(row, column))
                throw new ArgumentOutOfRangeException(
                    nameof(row),
                    "Position is outside the board.");

            return cells[row, column];
        }

        // =====================================================
        // ĐẶT QUÂN
        // =====================================================

        public bool PlacePiece(
            int row,
            int column,
            CellState player)
        {
            // Vị trí không hợp lệ
            if (!IsValidPosition(row, column))
                return false;

            // Không cho đặt Empty
            if (player == CellState.Empty)
                return false;

            // Ô đã có quân
            if (!IsEmpty(row, column))
                return false;

            cells[row, column] = player;

            return true;
        }

        // =====================================================
        // XÓA QUÂN
        // =====================================================

        public bool ClearCell(int row, int column)
        {
            if (!IsValidPosition(row, column))
                return false;

            cells[row, column] = CellState.Empty;

            return true;
        }

        // =====================================================
        // KIỂM TRA BÀN CỜ ĐẦY
        // =====================================================

        public bool IsFull()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    if (cells[row, column] == CellState.Empty)
                        return false;
                }
            }

            return true;
        }

        // =====================================================
        // KIỂM TRA THẮNG
        // =====================================================

        public bool CheckWin(int row, int column)
        {
            if (!IsValidPosition(row, column))
                return false;

            CellState player = cells[row, column];

            if (player == CellState.Empty)
                return false;

            // Ngang
            int horizontal =
                CountDirection(row, column, 0, 1, player) +
                CountDirection(row, column, 0, -1, player) +
                1;

            if (horizontal >= 5)
                return true;

            // Dọc
            int vertical =
                CountDirection(row, column, 1, 0, player) +
                CountDirection(row, column, -1, 0, player) +
                1;

            if (vertical >= 5)
                return true;

            // Chéo \
            int diagonal1 =
                CountDirection(row, column, 1, 1, player) +
                CountDirection(row, column, -1, -1, player) +
                1;

            if (diagonal1 >= 5)
                return true;

            // Chéo /
            int diagonal2 =
                CountDirection(row, column, 1, -1, player) +
                CountDirection(row, column, -1, 1, player) +
                1;

            if (diagonal2 >= 5)
                return true;

            return false;
        }

        // =====================================================
        // ĐẾM QUÂN LIÊN TIẾP
        // =====================================================

        private int CountDirection(
            int row,
            int column,
            int rowDirection,
            int columnDirection,
            CellState player)
        {
            int count = 0;

            int currentRow = row + rowDirection;
            int currentColumn = column + columnDirection;

            while (
                IsValidPosition(currentRow, currentColumn) &&
                cells[currentRow, currentColumn] == player)
            {
                count++;

                currentRow += rowDirection;
                currentColumn += columnDirection;
            }

            return count;
        }

        // =====================================================
        // LẤY BÀN CỜ
        // =====================================================

        public CellState[,] GetBoard()
        {
            CellState[,] result =
                new CellState[Rows, Columns];

            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    result[row, column] =
                        cells[row, column];
                }
            }

            return result;
        }

        // =====================================================
        // IN BÀN CỜ - DÙNG ĐỂ TEST
        // =====================================================

        public void PrintBoard()
        {
            Console.WriteLine();

            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    switch (cells[row, column])
                    {
                        case CellState.X:
                            Console.Write(" X ");
                            break;

                        case CellState.O:
                            Console.Write(" O ");
                            break;

                        default:
                            Console.Write(" . ");
                            break;
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }
}