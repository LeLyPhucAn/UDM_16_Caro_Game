using System;
using Shared.Enums;

namespace Shared.Models
{
    /// <summary>
    /// Đại diện cho kết quả cuối cùng của một trận Caro.
    /// </summary>
    public class GameResult
    {
        /// <summary>
        /// ID của trận đấu.
        /// </summary>
        public string MatchId { get; set; }

        /// <summary>
        /// Loại kết quả:
        /// Win / Draw / Timeout / Abandoned.
        /// </summary>
        public GameResultType ResultType { get; set; }

        /// <summary>
        /// ID người thắng.
        /// Null nếu hòa.
        /// </summary>
        public string? WinnerId { get; set; }

        /// <summary>
        /// ID người thua.
        /// Null nếu hòa.
        /// </summary>
        public string? LoserId { get; set; }

        /// <summary>
        /// Nguyên nhân kết thúc trận.
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// Thời điểm trận đấu kết thúc.
        /// </summary>
        public DateTime FinishedAt { get; set; }

        /// <summary>
        /// Kiểm tra kết quả có phải hòa hay không.
        /// </summary>
        public bool IsDraw
        {
            get
            {
                return ResultType == GameResultType.Draw;
            }
        }

        /// <summary>
        /// Kiểm tra có người thắng hay không.
        /// </summary>
        public bool HasWinner
        {
            get
            {
                return !string.IsNullOrWhiteSpace(WinnerId);
            }
        }

        public GameResult()
        {
            MatchId = string.Empty;
            ResultType = GameResultType.Draw;

            WinnerId = null;
            LoserId = null;
            Reason = null;

            FinishedAt = DateTime.Now;
        }

        public GameResult(
            string matchId,
            GameResultType resultType,
            string? winnerId,
            string? loserId,
            string? reason = null)
        {
            MatchId = matchId ?? string.Empty;

            ResultType = resultType;

            WinnerId = winnerId;

            LoserId = loserId;

            Reason = reason;

            FinishedAt = DateTime.Now;
        }
    }
}