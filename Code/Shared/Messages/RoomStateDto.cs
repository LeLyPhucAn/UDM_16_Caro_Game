namespace CaroGame.Protocol.Messages
{
    public class RoomStateDto
    {
        public string RoomId { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;

        // Thong tin chu phong va khach
        public string HostName { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;

        // Ky hieu quan co ("X" hoac "O")
        public string HostSymbol { get; set; } = "X";
        public string GuestSymbol { get; set; } = "O";

        // Trang thai san sang cua khach
        public bool IsGuestReady { get; set; } = false;

        // Tuong thich cu: Nguoi choi X va O
        public string PlayerX { get; set; } = string.Empty;
        public string PlayerO { get; set; } = string.Empty;
        public bool IsPlayerOReady { get; set; } = false;

        // Kich thuoc ban co
        public int BoardSize { get; set; } = 15;

        // So luong khan gia
        public int SpectatorCount { get; set; } = 0;
    }
}