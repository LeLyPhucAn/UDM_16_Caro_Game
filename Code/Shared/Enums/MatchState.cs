namespace Shared.Enums
{
    /// <summary>
    /// Trạng thái hiện tại của một trận Caro.
    /// </summary>
    public enum MatchState
    {
        /// <summary>
        /// Trận đấu đã được tạo nhưng chưa bắt đầu.
        /// </summary>
        Waiting = 0,

        /// <summary>
        /// Trận đấu đang diễn ra.
        /// </summary>
        Playing = 1,

        /// <summary>
        /// Trận đấu kết thúc do có người chiến thắng.
        /// </summary>
        Finished = 2,

        /// <summary>
        /// Trận đấu kết thúc với kết quả hòa.
        /// </summary>
        Draw = 3,

        /// <summary>
        /// Trận đấu bị hủy.
        /// </summary>
        Cancelled = 4
    }
}