using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Models;

namespace Server.AI
{
    /// <summary>
    /// Bot AI cho game Caro, sử dụng thuật toán Minimax kết hợp cắt tỉa Alpha-Beta.
    /// Hỗ trợ 3 mức độ: Easy, Medium, Hard.
    /// </summary>
    public class CaroBot
    {
        // CẤU HÌNH ĐỘ KHÓ

        /// <summary>Mức độ khó.</summary>
        public enum Difficulty
        {
            Easy,
            Medium,
            Hard
        }

        /// <summary>Độ sâu tìm kiếm Minimax tương ứng với mức độ khó.</summary>
        private static int GetSearchDepth(Difficulty difficulty)
        {
            return difficulty switch
            {
                Difficulty.Easy => 1,
                Difficulty.Medium => 2,
                Difficulty.Hard => 3,
                _ => 2
            };
        }

        /// <summary>Thời gian chờ (ms) để mô phỏng suy nghĩ.</summary>
        private static int GetThinkDelay(Difficulty difficulty)
        {
            return difficulty switch
            {
                Difficulty.Easy => 300,
                Difficulty.Medium => 500,
                Difficulty.Hard => 800,
                _ => 400
            };
        }
        // HẰNG SỐ ĐIỂM HEURISTIC

        private const int SCORE_FIVE = 100000;   // 5 liên tiếp (thắng)
        private const int SCORE_OPEN_FOUR = 10000;    // 4 mở 2 đầu
        private const int SCORE_HALF_FOUR = 2500;     // 4 chặn 1 đầu
        private const int SCORE_OPEN_THREE = 2500;     // 3 mở 2 đầu
        private const int SCORE_HALF_THREE = 300;      // 3 chặn 1 đầu
        private const int SCORE_OPEN_TWO = 100;      // 2 mở 2 đầu
        private const int SCORE_HALF_TWO = 10;       // 2 chặn 1 đầu

        private static readonly Random _random = new Random();
        // TÌM NƯỚC ĐI TỐT NHẤT

        /// <summary>
        /// Tìm nước đi tốt nhất cho Bot.
        /// </summary>
        /// <param name="board">Bàn cờ hiện tại.</param>
        /// <param name="botPiece">Quân cờ của Bot (X hoặc O).</param>
        /// <param name="difficulty">Mức độ khó.</param>
        /// <returns>Tọa độ (row, col) nước đi tốt nhất, hoặc null nếu không tìm được.</returns>
        public static (int row, int col)? FindBestMove(Board board, CellState botPiece, Difficulty difficulty)
        {
            if (board == null) return null;

            CellState opponentPiece = (botPiece == CellState.X) ? CellState.O : CellState.X;
            int depth = GetSearchDepth(difficulty);

            // Lấy bản sao bàn cờ
            CellState[,] cells = board.GetBoard();
            int rows = board.Rows;
            int cols = board.Columns;

            // Nếu bàn cờ trống, đánh giữa
            if (board.GetOccupiedCount() == 0)
            {
                return (rows / 2, cols / 2);
            }

            // Sinh danh sách nước đi ứng viên (candidate moves)
            var candidates = GenerateCandidateMoves(cells, rows, cols);
            if (candidates.Count == 0) return null;

            // Đối với chế độ Dễ: thêm yếu tố ngẫu nhiên
            if (difficulty == Difficulty.Easy)
            {
                return FindEasyMove(cells, rows, cols, botPiece, opponentPiece, candidates);
            }

            // Minimax + Alpha-Beta
            int bestScore = int.MinValue;
            var bestMoves = new List<(int row, int col)>();

            // Sắp xếp nước đi theo điểm heuristic để cắt tỉa tốt hơn
            var sortedCandidates = candidates
                .Select(m => (move: m, score: QuickEvaluateMove(cells, rows, cols, m.row, m.col, botPiece, opponentPiece)))
                .OrderByDescending(x => x.score)
                .Select(x => x.move)
                .Take(20) // Giới hạn tối đa 20 nước đi ứng viên
                .ToList();

            foreach (var (row, col) in sortedCandidates)
            {
                cells[row, col] = botPiece;

                int score = Minimax(cells, rows, cols, depth - 1, int.MinValue, int.MaxValue,
                                    false, botPiece, opponentPiece);

                cells[row, col] = CellState.Empty;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMoves.Clear();
                    bestMoves.Add((row, col));
                }
                else if (score == bestScore)
                {
                    bestMoves.Add((row, col));
                }
            }

            if (bestMoves.Count == 0) return null;

            // Nếu có nhiều nước đi cùng điểm, chọn ngẫu nhiên
            return bestMoves[_random.Next(bestMoves.Count)];
        }

        /// <summary>
        /// Trả về thời gian chờ suy nghĩ (ms) theo mức độ khó.
        /// </summary>
        public static int GetThinkDelayMs(Difficulty difficulty)
        {
            return GetThinkDelay(difficulty);
        }

        /// <summary>
        /// Chuyển đổi chuỗi mức độ khó sang enum.
        /// </summary>
        public static Difficulty ParseDifficulty(string? difficultyStr)
        {
            if (string.IsNullOrWhiteSpace(difficultyStr))
                return Difficulty.Medium;

            return difficultyStr.ToLower() switch
            {
                "easy" or "dễ" => Difficulty.Easy,
                "medium" or "trung bình" => Difficulty.Medium,
                "hard" or "khó" => Difficulty.Hard,
                _ => Difficulty.Medium
            };
        }
        // CHẾ ĐỘ DỄ (CÓ NGẪU NHIÊN)

        /// <summary>
        /// Chế độ Dễ: kiểm tra thắng/chặn trước, nếu không thì chọn ngẫu nhiên
        /// trong top nước đi có điểm khá để tạo cơ hội cho người chơi.
        /// </summary>
        private static (int row, int col)? FindEasyMove(
            CellState[,] cells, int rows, int cols,
            CellState botPiece, CellState opponentPiece,
            List<(int row, int col)> candidates)
        {
            // 1. Kiểm tra nước thắng ngay
            foreach (var (r, c) in candidates)
            {
                cells[r, c] = botPiece;
                bool win = CheckWinAt(cells, rows, cols, r, c, botPiece);
                cells[r, c] = CellState.Empty;
                if (win) return (r, c);
            }

            // 2. Kiểm tra nước chặn đối thủ sắp thắng
            foreach (var (r, c) in candidates)
            {
                cells[r, c] = opponentPiece;
                bool opWin = CheckWinAt(cells, rows, cols, r, c, opponentPiece);
                cells[r, c] = CellState.Empty;
                if (opWin) return (r, c);
            }

            // 3. Chọn ngẫu nhiên trong top 5 nước đi có điểm cao
            var scored = candidates
                .Select(m => (move: m, score: QuickEvaluateMove(cells, rows, cols, m.row, m.col, botPiece, opponentPiece)))
                .OrderByDescending(x => x.score)
                .Take(5)
                .ToList();

            if (scored.Count == 0) return null;

            return scored[_random.Next(scored.Count)].move;
        }
        // MINIMAX + ALPHA-BETA PRUNING

        /// <summary>
        /// Thuật toán Minimax kết hợp cắt tỉa Alpha-Beta.
        /// </summary>
        private static int Minimax(
            CellState[,] cells, int rows, int cols,
            int depth, int alpha, int beta,
            bool isMaximizing,
            CellState botPiece, CellState opponentPiece)
        {
            // Đánh giá thế cờ khi đạt độ sâu giới hạn
            if (depth == 0)
            {
                return EvaluateBoard(cells, rows, cols, botPiece, opponentPiece);
            }

            var candidates = GenerateCandidateMoves(cells, rows, cols);
            if (candidates.Count == 0)
            {
                return EvaluateBoard(cells, rows, cols, botPiece, opponentPiece);
            }

            // Sắp xếp để cắt tỉa hiệu quả hơn
            CellState currentPiece = isMaximizing ? botPiece : opponentPiece;
            var sortedCandidates = candidates
                .Select(m => (move: m, score: QuickEvaluateMove(cells, rows, cols, m.row, m.col, currentPiece,
                    isMaximizing ? opponentPiece : botPiece)))
                .OrderByDescending(x => x.score)
                .Select(x => x.move)
                .Take(15) // Giới hạn nhánh
                .ToList();

            if (isMaximizing)
            {
                int maxScore = int.MinValue;

                foreach (var (row, col) in sortedCandidates)
                {
                    cells[row, col] = botPiece;

                    // Kiểm tra thắng ngay
                    if (CheckWinAt(cells, rows, cols, row, col, botPiece))
                    {
                        cells[row, col] = CellState.Empty;
                        return SCORE_FIVE * (depth + 1); // Ưu tiên thắng nhanh
                    }

                    int score = Minimax(cells, rows, cols, depth - 1, alpha, beta, false, botPiece, opponentPiece);
                    cells[row, col] = CellState.Empty;

                    maxScore = Math.Max(maxScore, score);
                    alpha = Math.Max(alpha, score);

                    if (beta <= alpha) break; // Cắt tỉa Beta
                }

                return maxScore;
            }
            else
            {
                int minScore = int.MaxValue;

                foreach (var (row, col) in sortedCandidates)
                {
                    cells[row, col] = opponentPiece;

                    // Kiểm tra đối thủ thắng ngay
                    if (CheckWinAt(cells, rows, cols, row, col, opponentPiece))
                    {
                        cells[row, col] = CellState.Empty;
                        return -SCORE_FIVE * (depth + 1);
                    }

                    int score = Minimax(cells, rows, cols, depth - 1, alpha, beta, true, botPiece, opponentPiece);
                    cells[row, col] = CellState.Empty;

                    minScore = Math.Min(minScore, score);
                    beta = Math.Min(beta, score);

                    if (beta <= alpha) break; // Cắt tỉa Alpha
                }

                return minScore;
            }
        }
        // SINH NƯỚC ĐI ỨNG VIÊN (CANDIDATE MOVES)

        /// <summary>
        /// Sinh danh sách các ô trống nằm trong bán kính 2 quanh các quân cờ đã đánh.
        /// Tối ưu không gian tìm kiếm rất lớn so với duyệt toàn bàn.
        /// </summary>
        private static List<(int row, int col)> GenerateCandidateMoves(
            CellState[,] cells, int rows, int cols)
        {
            var candidates = new HashSet<(int, int)>();
            const int radius = 2;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (cells[r, c] == CellState.Empty)
                        continue;

                    // Thêm các ô trống trong bán kính xung quanh
                    for (int dr = -radius; dr <= radius; dr++)
                    {
                        for (int dc = -radius; dc <= radius; dc++)
                        {
                            int nr = r + dr;
                            int nc = c + dc;

                            if (nr >= 0 && nr < rows && nc >= 0 && nc < cols
                                && cells[nr, nc] == CellState.Empty)
                            {
                                candidates.Add((nr, nc));
                            }
                        }
                    }
                }
            }

            return candidates.ToList();
        }
        // ĐÁNH GIÁ NHANH MỘT NƯỚC ĐI (QUICK EVALUATE)

        /// <summary>
        /// Đánh giá nhanh điểm tiềm năng của một nước đi (dùng để sắp xếp ưu tiên).
        /// Gồm cả điểm tấn công lẫn phòng thủ.
        /// </summary>
        private static int QuickEvaluateMove(
            CellState[,] cells, int rows, int cols,
            int row, int col,
            CellState myPiece, CellState opponentPiece)
        {
            int score = 0;

            // Điểm tấn công: giả sử đặt quân mình
            cells[row, col] = myPiece;
            score += EvaluatePositionFor(cells, rows, cols, row, col, myPiece) * 2;
            cells[row, col] = CellState.Empty;

            // Điểm phòng thủ: giả sử đối thủ đặt quân
            cells[row, col] = opponentPiece;
            score += EvaluatePositionFor(cells, rows, cols, row, col, opponentPiece);
            cells[row, col] = CellState.Empty;

            return score;
        }
        // ĐÁNH GIÁ THẾ CỜ TOÀN BÀN (HEURISTIC EVALUATION)

        /// <summary>
        /// Đánh giá toàn bàn cờ cho Bot.
        /// Trả về điểm dương nếu Bot có lợi thế, âm nếu đối thủ có lợi thế.
        /// </summary>
        private static int EvaluateBoard(
            CellState[,] cells, int rows, int cols,
            CellState botPiece, CellState opponentPiece)
        {
            int score = 0;

            // Duyệt tất cả các đường (ngang, dọc, chéo chính, chéo phụ)
            // theo cửa sổ trượt 5 ô
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    // Ngang →
                    if (c + 4 < cols)
                        score += EvaluateWindow(cells, r, c, 0, 1, botPiece, opponentPiece);

                    // Dọc ↓
                    if (r + 4 < rows)
                        score += EvaluateWindow(cells, r, c, 1, 0, botPiece, opponentPiece);

                    // Chéo ↘
                    if (r + 4 < rows && c + 4 < cols)
                        score += EvaluateWindow(cells, r, c, 1, 1, botPiece, opponentPiece);

                    // Chéo ↗
                    if (r - 4 >= 0 && c + 4 < cols)
                        score += EvaluateWindow(cells, r, c, -1, 1, botPiece, opponentPiece);
                }
            }

            return score;
        }

        /// <summary>
        /// Đánh giá một cửa sổ 5 ô theo hướng cho trước.
        /// </summary>
        private static int EvaluateWindow(
            CellState[,] cells,
            int startRow, int startCol,
            int dRow, int dCol,
            CellState botPiece, CellState opponentPiece)
        {
            int botCount = 0;
            int opponentCount = 0;

            for (int i = 0; i < 5; i++)
            {
                CellState cell = cells[startRow + i * dRow, startCol + i * dCol];
                if (cell == botPiece) botCount++;
                else if (cell == opponentPiece) opponentCount++;
            }

            // Nếu cả hai đều có quân trong cửa sổ → không ai lợi
            if (botCount > 0 && opponentCount > 0) return 0;

            // Điểm cho Bot
            if (botCount > 0)
            {
                return botCount switch
                {
                    5 => SCORE_FIVE,
                    4 => SCORE_OPEN_FOUR,
                    3 => SCORE_OPEN_THREE,
                    2 => SCORE_OPEN_TWO,
                    1 => 1,
                    _ => 0
                };
            }

            // Điểm phòng thủ (đối thủ)
            if (opponentCount > 0)
            {
                return -(opponentCount switch
                {
                    5 => SCORE_FIVE,
                    4 => SCORE_OPEN_FOUR,
                    3 => SCORE_OPEN_THREE,
                    2 => SCORE_OPEN_TWO,
                    1 => 1,
                    _ => 0
                });
            }

            return 0;
        }
        // ĐÁNH GIÁ MỘT VỊ TRÍ CỤ THỂ

        /// <summary>
        /// Đánh giá tiềm năng của một vị trí cụ thể trên bàn cờ cho một quân cờ.
        /// Xét 4 hướng: ngang, dọc, chéo chính, chéo phụ.
        /// </summary>
        private static int EvaluatePositionFor(
            CellState[,] cells, int rows, int cols,
            int row, int col, CellState piece)
        {
            int score = 0;

            // 4 hướng: (dRow, dCol)
            int[][] directions = new int[][]
            {
                new[] { 0, 1 },  // Ngang
                new[] { 1, 0 },  // Dọc
                new[] { 1, 1 },  // Chéo \
                new[] { 1, -1 }  // Chéo /
            };

            foreach (var dir in directions)
            {
                score += EvaluateLine(cells, rows, cols, row, col, dir[0], dir[1], piece);
            }

            return score;
        }

        /// <summary>
        /// Đánh giá một đường thẳng (line) đi qua (row, col) theo hướng (dR, dC).
        /// Đếm số quân liên tiếp cùng loại và kiểm tra trạng thái 2 đầu (mở/chặn).
        /// </summary>
        private static int EvaluateLine(
            CellState[,] cells, int rows, int cols,
            int row, int col,
            int dR, int dC, CellState piece)
        {
            int count = 1; // Đếm quân liên tiếp (bao gồm ô hiện tại)
            bool openStart = false;
            bool openEnd = false;

            // Đếm theo hướng dương
            int r = row + dR, c = col + dC;
            while (r >= 0 && r < rows && c >= 0 && c < cols && cells[r, c] == piece)
            {
                count++;
                r += dR;
                c += dC;
            }
            // Kiểm tra đầu dương có mở không
            if (r >= 0 && r < rows && c >= 0 && c < cols && cells[r, c] == CellState.Empty)
                openEnd = true;

            // Đếm theo hướng âm
            r = row - dR;
            c = col - dC;
            while (r >= 0 && r < rows && c >= 0 && c < cols && cells[r, c] == piece)
            {
                count++;
                r -= dR;
                c -= dC;
            }
            // Kiểm tra đầu âm có mở không
            if (r >= 0 && r < rows && c >= 0 && c < cols && cells[r, c] == CellState.Empty)
                openStart = true;

            // Tính điểm dựa trên số quân và trạng thái đầu
            if (count >= 5) return SCORE_FIVE;

            bool isOpen = openStart && openEnd;

            return count switch
            {
                4 => isOpen ? SCORE_OPEN_FOUR : (openStart || openEnd ? SCORE_HALF_FOUR : 0),
                3 => isOpen ? SCORE_OPEN_THREE : (openStart || openEnd ? SCORE_HALF_THREE : 0),
                2 => isOpen ? SCORE_OPEN_TWO : (openStart || openEnd ? SCORE_HALF_TWO : 0),
                1 => isOpen ? 1 : 0,
                _ => 0
            };
        }
        // KIỂM TRA THẮNG TẠI VỊ TRÍ

        /// <summary>
        /// Kiểm tra xem đặt quân tại (row, col) có tạo thành 5 liên tiếp không.
        /// </summary>
        private static bool CheckWinAt(
            CellState[,] cells, int rows, int cols,
            int row, int col, CellState piece)
        {
            int[][] directions = new int[][]
            {
                new[] { 0, 1 },  // Ngang
                new[] { 1, 0 },  // Dọc
                new[] { 1, 1 },  // Chéo \
                new[] { 1, -1 }  // Chéo /
            };

            foreach (var dir in directions)
            {
                int count = 1;

                // Đếm hướng dương
                int r = row + dir[0], c = col + dir[1];
                while (r >= 0 && r < rows && c >= 0 && c < cols && cells[r, c] == piece)
                {
                    count++;
                    r += dir[0];
                    c += dir[1];
                }

                // Đếm hướng âm
                r = row - dir[0];
                c = col - dir[1];
                while (r >= 0 && r < rows && c >= 0 && c < cols && cells[r, c] == piece)
                {
                    count++;
                    r -= dir[0];
                    c -= dir[1];
                }

                if (count >= 5) return true;
            }

            return false;
        }
    }
}

