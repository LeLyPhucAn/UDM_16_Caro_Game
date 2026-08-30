using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Models;

namespace Server.Managers
{
    // ==============================
    // ROOM
    // ==============================
    public class Room
    {
        public string RoomId { get; private set; }
        public string RoomName { get; private set; }

        public int MaxPlayers { get; private set; }

        public List<Player> Players { get; private set; }

        public bool IsPlaying { get; set; }

        public Room(
            string roomId,
            string roomName,
            int maxPlayers = 2)
        {
            RoomId = roomId;
            RoomName = roomName;
            MaxPlayers = maxPlayers;

            Players = new List<Player>();

            IsPlaying = false;
        }

        // ==============================
        // KIỂM TRA PHÒNG ĐẦY
        // ==============================
        public bool IsFull()
        {
            return Players.Count >= MaxPlayers;
        }

        // ==============================
        // THÊM PLAYER
        // ==============================
        public bool AddPlayer(Player player)
        {
            if (player == null)
                return false;

            if (IsFull())
                return false;

            // Không cho cùng một Player vào phòng 2 lần
            bool alreadyExists = Players.Any(
                p => p.Id == player.Id);

            if (alreadyExists)
                return false;

            Players.Add(player);

            return true;
        }

        // ==============================
        // XÓA PLAYER
        // ==============================
        public bool RemovePlayer(string playerId)
        {
            Player? player = Players.FirstOrDefault(
                p => p.Id == playerId);

            if (player == null)
                return false;

            Players.Remove(player);

            return true;
        }

        // ==============================
        // TÌM PLAYER
        // ==============================
        public Player? GetPlayer(string playerId)
        {
            return Players.FirstOrDefault(
                p => p.Id == playerId);
        }

        // ==============================
        // KIỂM TRA PLAYER CÓ TRONG PHÒNG
        // ==============================
        public bool ContainsPlayer(string playerId)
        {
            return Players.Any(
                p => p.Id == playerId);
        }
    }

    // ==============================
    // ROOM MANAGER
    // ==============================
    public class RoomManager
    {
        private readonly Dictionary<string, Room> rooms;

        public RoomManager()
        {
            rooms = new Dictionary<string, Room>();
        }

        // ==============================
        // TẠO PHÒNG
        // ==============================
        public Room CreateRoom(string roomName)
        {
            if (string.IsNullOrWhiteSpace(roomName))
            {
                roomName = "Room";
            }

            string roomId = Guid.NewGuid().ToString();

            Room room = new Room(
                roomId,
                roomName,
                2);

            rooms.Add(roomId, room);

            Console.WriteLine(
                $"[ROOM] Created: {room.RoomName} ({room.RoomId})");

            return room;
        }

        // ==============================
        // XÓA PHÒNG
        // ==============================
        public bool RemoveRoom(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
                return false;

            if (!rooms.ContainsKey(roomId))
                return false;

            rooms.Remove(roomId);

            Console.WriteLine(
                $"[ROOM] Removed: {roomId}");

            return true;
        }

        // ==============================
        // LẤY PHÒNG
        // ==============================
        public Room? GetRoom(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
                return null;

            rooms.TryGetValue(
                roomId,
                out Room? room);

            return room;
        }

        // ==============================
        // PLAYER THAM GIA PHÒNG
        // ==============================
        public bool JoinRoom(
            string roomId,
            Player player)
        {
            if (string.IsNullOrWhiteSpace(roomId))
                return false;

            if (player == null)
                return false;

            Room? room = GetRoom(roomId);

            if (room == null)
                return false;

            bool result = room.AddPlayer(player);

            if (result)
            {
                Console.WriteLine(
                    $"[ROOM] {player.Username} joined {room.RoomName}");
            }

            return result;
        }

        // ==============================
        // PLAYER RỜI PHÒNG
        // ==============================
        public bool LeaveRoom(
            string roomId,
            string playerId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
                return false;

            if (string.IsNullOrWhiteSpace(playerId))
                return false;

            Room? room = GetRoom(roomId);

            if (room == null)
                return false;

            Player? player = room.GetPlayer(playerId);

            if (player == null)
                return false;

            bool result = room.RemovePlayer(playerId);

            if (result)
            {
                Console.WriteLine(
                    $"[ROOM] {player.Username} left {room.RoomName}");
            }

            // Nếu không còn Player nào thì xóa phòng
            if (room.Players.Count == 0)
            {
                RemoveRoom(roomId);
            }

            return result;
        }

        // ==============================
        // TÌM PHÒNG CỦA PLAYER
        // ==============================
        public Room? FindPlayerRoom(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
                return null;

            foreach (Room room in rooms.Values)
            {
                if (room.ContainsPlayer(playerId))
                {
                    return room;
                }
            }

            return null;
        }

        // ==============================
        // KIỂM TRA PHÒNG TỒN TẠI
        // ==============================
        public bool RoomExists(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
                return false;

            return rooms.ContainsKey(roomId);
        }

        // ==============================
        // LẤY DANH SÁCH PHÒNG
        // ==============================
        public List<Room> GetRooms()
        {
            return rooms.Values.ToList();
        }

        // ==============================
        // LẤY SỐ LƯỢNG PHÒNG
        // ==============================
        public int GetRoomCount()
        {
            return rooms.Count;
        }

        // ==============================
        // LẤY SỐ PLAYER TRONG PHÒNG
        // ==============================
        public int GetPlayerCount(string roomId)
        {
            Room? room = GetRoom(roomId);

            if (room == null)
                return 0;

            return room.Players.Count;
        }

        // ==============================
        // KIỂM TRA PHÒNG CÓ THỂ CHƠI
        // ==============================
        public bool CanStartGame(string roomId)
        {
            Room? room = GetRoom(roomId);

            if (room == null)
                return false;

            return room.Players.Count == 2 &&
                   !room.IsPlaying;
        }

        // ==============================
        // ĐẶT TRẠNG THÁI ĐANG CHƠI
        // ==============================
        public bool SetPlaying(
            string roomId,
            bool playing)
        {
            Room? room = GetRoom(roomId);

            if (room == null)
                return false;

            room.IsPlaying = playing;

            return true;
        }
    }
}