namespace CaroGame.Protocol.Messages.Response
{
    /// <summary>
    /// Message dùng để Server phản hồi lại cho Client (thành công / thất bại)
    /// đối với một yêu cầu (Login, CreateRoom, JoinRoom, Invite, ...) trước đó.
    /// </summary>
    public class ResponseMessage : BaseMessage
    {
        /// <summary>
        /// MessageId của request gốc mà response này phản hồi lại, giúp client
        /// đối chiếu đúng response cho đúng request khi gửi nhiều request liên tiếp.
        /// </summary>
        public string RequestMessageId { get; set; }

        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string Data { get; set; }
        public string Action { get; set; } = string.Empty;

        public ResponseMessage()
        {
            Type = MessageType.Response;
        }
    }
}
