namespace CaroGame.Protocol.Messages
{
    /// <summary>
    /// Message client gửi lên server để đăng nhập vào hệ thống.
    /// </summary>
    public class LoginMessage : BaseMessage
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginMessage()
        {
            Type = MessageType.Login;
        }
    }
}
