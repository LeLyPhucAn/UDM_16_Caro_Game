using System;

namespace Shared.Models
{
    /// <summary>
    /// Trạng thái của một ô trên bàn cờ.
    /// </summary>
    public enum CellState
    {
        Empty = 0,
        X = 1,
        O = 2
    }

    /// <summary>
    /// Bàn cờ Caro.
    /// </summary>
    public class Board
    {
        public const int DefaultRows = 15;
        public const int DefaultColumns = 15;
        public const int WinLength = 5;

        public int Rows { get; }
        public int Columns { get; }

        private readonly CellState[,] cells;

        public Board()
            : this(DefaultRows, DefaultColumns)
        {
        }

        public Board(int rows, int columns)
        {
            if (rows <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(rows),
                    "Rows must be greater than zero.");

            if (columns <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(columns),
                    "Columns must be greater than zero.");

            Rows = rows;
            Columns = columns;

            cells = new CellState[Rows, Columns];

            Reset();
        }

        /// <summary>
        /// Reset toàn bộ bàn cờ.
        /// </summary>
        public void Reset()
        {
            Array.Clear(cells, 0, cells.Length);
        }

        /// <summary>
        /// Kiểm tra tọa độ có nằm trong bàn cờ.
        /// </summary>
        public bool IsValidPosition(int row, int column)
        {
            return row >= 0 &&
                   row < Rows &&
                   column >= 0 &&
                   column < Columns;
        }

        /// <summary>
        /// Kiểm tra ô có trống.
        /// </summary>
        public bool IsEmpty(int row, int column)
        {
            return IsValidPosition(row, column) &&
                   cells[row, column] == CellState.Empty;
        }

        /// <summary>
        /// Lấy trạng thái ô.
        /// </summary>
        public CellState GetCell(int row, int column)
        {
            if (!IsValidPosition(row, column))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(row),
                    "Position is outside the board.");
            }

            return cells[row, column];
        }

        /// <summary>
        /// Đặt quân.
        ///
        /// Không cho:
        /// - tọa độ sai
        /// - đặt Empty
        /// - đặt vào ô đã có quân
        /// </summary>
        public bool PlacePiece(
            int row,
            int column,
            CellState piece)
        {
            if (!IsValidPosition(row, column))
                return false;

            if (piece != CellState.X &&
                piece != CellState.O)
            {
                return false;
            }

            if (!IsEmpty(row, column))
                return false;

            cells[row, column] = piece;

            return true;
        }

        /// <summary>
        /// Xóa một ô.
        /// </summary>
        public bool ClearCell(int row, int column)
        {
            if (!IsValidPosition(row, column))
                return false;

            cells[row, column] = CellState.Empty;

            return true;
        }

        /// <summary>
        /// Kiểm tra bàn cờ đã đầy.
        /// </summary>
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

        /// <summary>
        /// Kiểm tra người chơi tại vị trí vừa đánh
        /// có đạt 5 quân liên tiếp hay không.
        ///
        /// Kiểm tra:
        /// - ngang
        /// - dọc
        /// - chéo \
        /// - chéo /
        /// </summary>
        public bool CheckWin(int row, int column)
        {
            if (!IsValidPosition(row, column))
                return false;

            CellState piece = cells[row, column];

            if (piece == CellState.Empty)
                return false;

            return CountLine(row, column, 0, 1, piece) >= WinLength ||
                   CountLine(row, column, 1, 0, piece) >= WinLength ||
                   CountLine(row, column, 1, 1, piece) >= WinLength ||
                   CountLine(row, column, 1, -1, piece) >= WinLength;
        }

        private int CountLine(
            int row,
            int column,
            int rowDirection,
            int columnDirection,
            CellState piece)
        {
            int count = 1;

            count += CountDirection(
                row,
                column,
                rowDirection,
                columnDirection,
                piece);

            count += CountDirection(
                row,
                column,
                -rowDirection,
                -columnDirection,
                piece);

            return count;
        }

        private int CountDirection(
            int row,
            int column,
            int rowDirection,
            int columnDirection,
            CellState piece)
        {
            int count = 0;

            int currentRow = row + rowDirection;
            int currentColumn = column + columnDirection;

            while (
                IsValidPosition(currentRow, currentColumn) &&
                cells[currentRow, currentColumn] == piece)
            {
                count++;

                currentRow += rowDirection;
                currentColumn += columnDirection;
            }

            return count;
        }

        /// <summary>
        /// Trả về bản sao bàn cờ.
        /// </summary>
        public CellState[,] GetBoard()
        {
            CellState[,] result =
                new CellState[Rows, Columns];

            Array.Copy(
                cells,
                result,
                cells.Length);

            return result;
        }

        /// <summary>
        /// Đếm số ô đã đánh.
        /// </summary>
        public int GetOccupiedCount()
        {
            int count = 0;

            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    if (cells[row, column] != CellState.Empty)
                        count++;
                }
            }

            return count;
        }
    }
}