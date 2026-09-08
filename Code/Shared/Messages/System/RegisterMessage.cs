namespace CaroGame.Protocol.Messages
{
    /// <summary>
    /// Message client gửi lên server để đăng ký tài khoản.
    /// </summary>
    public class RegisterMessage : BaseMessage
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public RegisterMessage()
        {
            Type = MessageType.Register;
        }
    }
}
