namespace GameLogic.Models
{
    public class Match
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        public Player Player1 { get; set; }

        public Player Player2 { get; set; }

        public Board Board { get; set; }

        public int CurrentPlayerId { get; set; }

        public string Status { get; set; }

        public int? WinnerId { get; set; }

        public Match()
        {
            Player1 = new Player();
            Player2 = new Player();

            Board = new Board();

            Status = "Waiting";

            WinnerId = null;
        }

        public Match(
            int id,
            int roomId,
            Player player1,
            Player player2)
        {
            Id = id;
            RoomId = roomId;

            Player1 = player1;
            Player2 = player2;

            Board = new Board();

            CurrentPlayerId = player1.Id;

            Status = "Playing";

            WinnerId = null;
        }
    }
}