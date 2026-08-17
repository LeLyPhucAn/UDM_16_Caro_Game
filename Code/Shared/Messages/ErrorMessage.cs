using System;

namespace CaroGame.Protocol
{
    /// <summary>
    /// Message dùng để báo lỗi ở tầng giao thức/hệ thống (packet sai định dạng,
    /// MessageType không được hỗ trợ, lỗi xử lý phía server, ...).
    /// </summary>
    public class ErrorMessage : BaseMessage
    {
        public string ErrorCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ErrorMessage()
        {
            Type = MessageType.Error;
        }
    }
}
