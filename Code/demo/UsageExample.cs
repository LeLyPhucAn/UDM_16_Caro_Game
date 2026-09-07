using System;
using CaroGame.Protocol.Messages;
using CaroGame.Protocol.Messages.Room;
using CaroGame.Protocol.Messages.Game;
using CaroGame.Protocol.Messages.Response;
using CaroGame.Protocol.Network;
using CaroGame.Server.Services;

namespace CaroGame.Protocol
{
    public class UsageExample
    {
        public static void Run()
        {
            // ================= Task 1: Room / Lobby Messages =================

            // 1. Client tạo phòng mới
            CreateRoomMessage createRoom = new CreateRoomMessage
            {
                SenderId = "player_001",
                RoomName = "Phòng của Khuyên",
                HostId = "player_001",
                MaxPlayers = 2,
                BoardSize = 15,
                IsPrivate = false
            };
            SendAndReceive(createRoom);

            // 2. Client thứ 2 tham gia phòng
            JoinRoomMessage joinRoom = new JoinRoomMessage
            {
                SenderId = "player_002",
                RoomId = "room_001",
                PlayerName = "player_002"
            };
            SendAndReceive(joinRoom);

            // 3. Server gửi TurnMessage báo lượt đi đầu tiên
            TurnMessage turn = new TurnMessage
            {
                SenderId = "server",
                RoomId = "room_001",
                CurrentPlayerId = "player_001",
                TurnNumber = 1,
                TimeLimitSeconds = 30
            };
            SendAndReceive(turn);

            // 4. Client gửi nước đi
            MoveMessage move = new MoveMessage
            {
                SenderId = "player_001",
                RoomId = "room_001",
                Row = 7,
                Column = 7,
                Symbol = "X"
            };
            SendAndReceive(move);

            // 5. Server broadcast GameStateMessage sau nước đi
            GameStateMessage state = new GameStateMessage
            {
                SenderId = "server",
                RoomId = "room_001",
                BoardSize = 15,
                BoardState = new string('-', 15 * 15),
                CurrentPlayerId = "player_002",
                Status = "Playing"
            };
            SendAndReceive(state);

            // 6. Server gửi GameResultMessage khi ván đấu kết thúc
            GameResultMessage result = new GameResultMessage
            {
                SenderId = "server",
                RoomId = "room_001",
                WinnerId = "player_001",
                ResultType = "Win",
                WinningLine = new[] { "7,7", "7,8", "7,9", "7,10", "7,11" }
            };
            SendAndReceive(result);

            // 7. Client rời phòng sau khi kết thúc
            LeaveRoomMessage leaveRoom = new LeaveRoomMessage
            {
                SenderId = "player_002",
                RoomId = "room_001",
                PlayerId = "player_002",
                Reason = "voluntary"
            };
            SendAndReceive(leaveRoom);

            // ================= Task 2: Protocol Integration & Packet Validation =================

            // 8. Validate Header -> Xử lý Packet thiếu dữ liệu
            byte[] corruptedPacket = new byte[5]; // ít hơn 8 byte Header cần thiết
            if (!PacketParser.TryUnpack(corruptedPacket, out _, out string headerError))
            {
                Console.WriteLine("[Validate Header] Packet thiếu dữ liệu: " + headerError);
            }

            // 9. Validate MessageType -> Xử lý MessageType không hợp lệ
            byte[] invalidTypePacket = BuildRawPacket(rawType: 9999, body: "{}");
            if (!PacketParser.TryUnpack(invalidTypePacket, out _, out string typeError))
            {
                Console.WriteLine("[Validate MessageType] MessageType không hợp lệ: " + typeError);
            }

            // 10. Validate JSON -> Xử lý Packet bị lỗi
            byte[] invalidJsonPacket = BuildRawPacket(rawType: (int)MessageType.Move, body: "{ khong-phai-json");
            if (!PacketParser.TryUnpack(invalidJsonPacket, out _, out string jsonError))
            {
                Console.WriteLine("[Validate JSON] Packet bị lỗi: " + jsonError);
            }

            // ===== Deserialize Packet + Tạo Error Response (thông qua MessageHandler) =====
            MessageHandler handler = new MessageHandler();

            // 11. Test Packet với các Message Lobby (dùng lại createRoom ở Task 1)
            byte[] createRoomReply = handler.HandlePacket(PacketParser.Pack(createRoom));
            PrintReply("Test Packet Lobby - CreateRoom", createRoomReply);

            // 12. Test Packet với các Message Game (dùng lại move ở Task 1)
            byte[] moveReply = handler.HandlePacket(PacketParser.Pack(move));
            PrintReply("Test Packet Game - Move", moveReply);

            // 13. Test Packet lỗi đi qua MessageHandler -> phải nhận Error Response, không crash
            byte[] errorReply = handler.HandlePacket(invalidJsonPacket);
            PrintReply("Test Packet lỗi - CorruptedPacket", errorReply);
        }

        /// <summary>In ra loại message nhận được từ packet phản hồi của MessageHandler.</summary>
        private static void PrintReply(string label, byte[] replyPacket)
        {
            if (replyPacket == null)
            {
                Console.WriteLine("[" + label + "] Không có phản hồi (theo thiết kế).");
                return;
            }

            if (PacketParser.TryUnpack(replyPacket, out BaseMessage reply, out string error))
            {
                Console.WriteLine("[" + label + "] Phản hồi loại: " + reply.Type);
            }
            else
            {
                Console.WriteLine("[" + label + "] Lỗi đọc phản hồi: " + error);
            }
        }

        /// <summary>Đóng gói rồi giải mã ngay một message để minh hoạ luồng Pack/Unpack.</summary>
        private static void SendAndReceive(BaseMessage message)
        {
            byte[] packet = PacketParser.Pack(message);

            if (PacketParser.TryUnpack(packet, out BaseMessage received, out string error))
            {
                Console.WriteLine("Đã nhận [" + received.Type + "] MessageId=" + received.MessageId);
            }
            else
            {
                Console.WriteLine("Lỗi giải mã [" + message.Type + "]: " + error);
            }
        }

        /// <summary>Dựng thủ công một packet thô [Type][Length][Body] để giả lập dữ liệu lỗi.</summary>
        private static byte[] BuildRawPacket(int rawType, string body)
        {
            byte[] typeBytes = BitConverter.GetBytes(rawType);
            byte[] bodyBytes = System.Text.Encoding.UTF8.GetBytes(body);
            byte[] lengthBytes = BitConverter.GetBytes(bodyBytes.Length);

            byte[] packet = new byte[8 + bodyBytes.Length];
            Buffer.BlockCopy(typeBytes, 0, packet, 0, 4);
            Buffer.BlockCopy(lengthBytes, 0, packet, 4, 4);
            Buffer.BlockCopy(bodyBytes, 0, packet, 8, bodyBytes.Length);
            return packet;
        }
    }
}
