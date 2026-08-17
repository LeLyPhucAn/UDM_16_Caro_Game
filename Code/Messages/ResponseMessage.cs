namespace CaroGame.Protocol.Messages
{
    /// <summary>
    /// Message dùng để Server phản hồi lại cho Client (thành công / thất bại)
    /// đối với một yêu cầu (Login, Invite, ...) trước đó.
    /// </summary>
    public class ResponseMessage : BaseMessage
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string Data { get; set; }

        public ResponseMessage()
        {
            Type = MessageType.Response;
        }
    }
}
