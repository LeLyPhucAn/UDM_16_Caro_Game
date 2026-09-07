namespace Shared.Messages.Response
{
    /// <summary>
    /// Message dùng để Server phản hồi lại cho Client (thành công / thất bại)
    /// đối với một yêu cầu (Login, CreateRoom, JoinRoom, Invite, ...) trước đó.
    /// </summary>
    public class ResponseMessage : Shared.Messages.BaseMessage
    {
        /// <summary>
        /// MessageId của request gốc mà response này phản hồi lại, giúp client
        /// đối chiếu đúng response cho đúng request khi gửi nhiều request liên tiếp.
        /// </summary>
        public string RequestMessageId { get; set; }

        public bool Success { get; set; }

        /// <summary>
        /// Mô tả lỗi nghiệp vụ khi Success = false (ví dụ: sai mật khẩu, phòng đầy).
        /// Đổi tên từ "ErrorMessage" (bản cũ) thành "ErrorDetail" để không trùng
        /// tên với class ErrorMessage bên dưới - class đó dùng cho lỗi tầng
        /// giao thức/hệ thống, khác với lỗi nghiệp vụ của 1 request cụ thể.
        /// </summary>
        public string ErrorDetail { get; set; }

        public string Data { get; set; }

        public ResponseMessage()
        {
            Type = Shared.Messages.MessageType.Response;
        }
    }

    /// <summary>
    /// Message dùng để báo lỗi ở tầng giao thức/hệ thống (packet sai định dạng,
    /// MessageType không được hỗ trợ, lỗi xử lý phía server, ...).
    /// Khác với ResponseMessage.ErrorDetail vốn dùng cho lỗi nghiệp vụ của
    /// một request cụ thể (ví dụ sai mật khẩu), ErrorMessage ở đây dùng khi
    /// server/client không thể xử lý được gói tin gửi tới.
    /// </summary>
    public class ErrorMessage : Shared.Messages.BaseMessage
    {
        /// <summary>Mã lỗi ngắn gọn, ví dụ: MISSING_DATA, INVALID_MESSAGE_TYPE, CORRUPTED_PACKET, SERVER_ERROR.</summary>
        public string ErrorCode { get; set; }

        /// <summary>Mô tả chi tiết lỗi để log/debug hoặc hiển thị cho người dùng.</summary>
        public string Description { get; set; }

        public ErrorMessage()
        {
            Type = Shared.Messages.MessageType.Error;
        }
    }
}
