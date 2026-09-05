namespace Shared.Enums
{
    /// <summary>
    /// Loại kết quả của một trận Caro.
    /// </summary>
    public enum GameResultType
    {
        /// <summary>
        /// Một người chơi thắng bằng cách tạo đủ 5 quân liên tiếp.
        /// </summary>
        Win,

        /// <summary>
        /// Hai người chơi hòa vì bàn cờ đã đầy.
        /// </summary>
        Draw,

        /// <summary>
        /// Người chơi hết thời gian.
        /// </summary>
        Timeout,

        /// <summary>
        /// Trận đấu bị kết thúc bởi Server hoặc người chơi rời trận.
        /// </summary>
        Abandoned
    }
}