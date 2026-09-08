namespace CaroGame.Protocol.Messages.Response
{
    /// <summary>
    /// Message dùng để báo lỗi ở tầng giao thức/hệ thống (packet sai định dạng,
    /// MessageType không được hỗ trợ, lỗi xử lý phía server, ...).
    /// Khác với ResponseMessage.ErrorMessage vốn dùng cho lỗi nghiệp vụ của
    /// một request cụ thể (ví dụ sai mật khẩu), ErrorMessage ở đây dùng khi
    /// server/client không thể xử lý được gói tin gửi tới.
    /// </summary>
    public class ErrorMessage : BaseMessage
    {
        /// <summary>Mã lỗi ngắn gọn, ví dụ: INVALID_PACKET, UNKNOWN_TYPE, SERVER_ERROR.</summary>
        public string ErrorCode { get; set; }

        /// <summary>Mô tả chi tiết lỗi để log/debug hoặc hiển thị cho người dùng.</summary>
        public string Description { get; set; }

        public ErrorMessage()
        {
            Type = MessageType.Error;
        }
    }
}
