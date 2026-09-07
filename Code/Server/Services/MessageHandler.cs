using System;
using CaroGame.Protocol.Messages;
using CaroGame.Protocol.Messages.Room;
using CaroGame.Protocol.Messages.Game;
using CaroGame.Protocol.Messages.Response;
using CaroGame.Protocol.Network;

namespace CaroGame.Server.Services
{
    /// <summary>
    /// Nơi tiếp nhận packet thô (byte[]) từ Socket, kiểm tra tính hợp lệ
    /// thông qua PacketParser, sau đó điều hướng (route) message hợp lệ
    /// đến đúng hàm xử lý nghiệp vụ tương ứng với từng MessageType.
    ///
    /// Task 2 - Protocol Integration &amp; Packet Validation: các hàm trong
    /// class này được đặt tên bám sát từng mục trong đề bài để dễ đối chiếu:
    /// HandleMissingDataPacket, HandleInvalidMessageType, HandleCorruptedPacket,
    /// CreateErrorResponse.
    /// </summary>
    public class MessageHandler
    {
        // ===================== Đảm bảo Packet lỗi không làm Client/Server crash =====================
        /// <summary>
        /// Hàm chính được SocketServer gọi mỗi khi nhận đủ 1 packet từ client.
        /// Toàn bộ luồng xử lý được bọc trong try/catch nên packet lỗi ở bất
        /// kỳ bước nào (Validate hay xử lý nghiệp vụ) cũng không làm crash chương trình.
        /// </summary>
        public byte[] HandlePacket(byte[] rawData)
        {
            try
            {
                BaseMessage message = PacketParser.Unpack(rawData);
                return RouteMessage(message);
            }
            catch (PacketException ex)
            {
                // Packet không hợp lệ (thiếu dữ liệu, sai MessageType, hoặc bị lỗi)
                return HandleInvalidPacket(ex);
            }
            catch (Exception ex)
            {
                // Lỗi phát sinh ngoài dự kiến trong lúc xử lý nghiệp vụ
                Console.WriteLine("[Lỗi Server] " + ex.Message);
                return CreateErrorResponse("SERVER_ERROR", "Lỗi xử lý message: " + ex.Message);
            }
        }

        /// <summary>Dựa vào PacketErrorType để gọi đúng hàm xử lý lỗi tương ứng.</summary>
        private byte[] HandleInvalidPacket(PacketException ex)
        {
            switch (ex.ErrorType)
            {
                case PacketErrorType.MissingData:
                    return HandleMissingDataPacket(ex);

                case PacketErrorType.InvalidMessageType:
                    return HandleInvalidMessageType(ex);

                case PacketErrorType.CorruptedPacket:
                default:
                    return HandleCorruptedPacket(ex);
            }
        }

        // ===================== Xử lý Packet thiếu dữ liệu =====================
        /// <summary>Packet thiếu Header hoặc thiếu Body so với độ dài khai báo.</summary>
        private byte[] HandleMissingDataPacket(PacketException ex)
        {
            Console.WriteLine("[Packet thiếu dữ liệu] " + ex.Message);
            return CreateErrorResponse("MISSING_DATA", ex.Message);
        }

        // ===================== Xử lý MessageType không hợp lệ =====================
        /// <summary>MessageType trong Header không tồn tại trong enum MessageType.</summary>
        private byte[] HandleInvalidMessageType(PacketException ex)
        {
            Console.WriteLine("[MessageType không hợp lệ] " + ex.Message);
            return CreateErrorResponse("INVALID_MESSAGE_TYPE", ex.Message);
        }

        // ===================== Xử lý Packet bị lỗi =====================
        /// <summary>Body không phải JSON hợp lệ, thiếu MessageId, hoặc deserialize thất bại.</summary>
        private byte[] HandleCorruptedPacket(PacketException ex)
        {
            Console.WriteLine("[Packet bị lỗi] " + ex.Message);
            return CreateErrorResponse("CORRUPTED_PACKET", ex.Message);
        }

        // ===================== Tạo Error Response =====================
        /// <summary>Hàm dùng chung để tạo và đóng gói ErrorMessage trả về cho client.</summary>
        private byte[] CreateErrorResponse(string code, string description)
        {
            ErrorMessage error = new ErrorMessage
            {
                SenderId = "server",
                ErrorCode = code,
                Description = description
            };
            return PacketParser.Pack(error);
        }

        // ===================== Điều hướng Packet hợp lệ =====================
        /// <summary>Dựa vào Type để gọi đúng hàm xử lý nghiệp vụ tương ứng.</summary>
        private byte[] RouteMessage(BaseMessage message)
        {
            switch (message.Type)
            {
                case MessageType.Login:
                    return HandleLogin((LoginMessage)message);

                // ===== Lobby Messages =====
                case MessageType.CreateRoom:
                    return HandleCreateRoom((CreateRoomMessage)message);

                case MessageType.JoinRoom:
                    return HandleJoinRoom((JoinRoomMessage)message);

                case MessageType.LeaveRoom:
                    return HandleLeaveRoom((LeaveRoomMessage)message);

                case MessageType.Invite:
                    return HandleInvite((InviteMessage)message);

                // ===== Game Messages =====
                case MessageType.Move:
                    return HandleMove((MoveMessage)message);

                case MessageType.Turn:
                    return HandleTurn((TurnMessage)message);

                case MessageType.GameState:
                    return HandleGameState((GameStateMessage)message);

                case MessageType.GameResult:
                    return HandleGameResult((GameResultMessage)message);

                // Server thường không cần tự xử lý message loại Response/Error
                // gửi ngược lại chính nó, chỉ Client mới cần đọc 2 loại này.
                case MessageType.Response:
                case MessageType.Error:
                    return null;

                default:
                    return CreateErrorResponse("UNSUPPORTED_TYPE",
                        "Server chưa hỗ trợ xử lý message loại: " + message.Type);
            }
        }

        // ===================== Xử lý nghiệp vụ từng loại Message (khung sườn) =====================
        // Ở đây chỉ demo log + trả Response mẫu. Khi làm Task nghiệp vụ thật
        // (quản lý phòng, quản lý ván đấu) sẽ thay phần TODO bằng logic thật.

        private byte[] HandleLogin(LoginMessage msg)
        {
            Console.WriteLine("[Login] user=" + msg.Username);
            // TODO: kiểm tra username/password trong CSDL
            return PacketParser.Pack(CreateSuccessResponse(msg, "Đăng nhập thành công"));
        }

        private byte[] HandleCreateRoom(CreateRoomMessage msg)
        {
            Console.WriteLine("[CreateRoom] " + msg.RoomName + " host=" + msg.HostId);
            // TODO: tạo phòng thật trong RoomManager, lấy RoomId trả về
            return PacketParser.Pack(CreateSuccessResponse(msg, "Tạo phòng thành công"));
        }

        private byte[] HandleJoinRoom(JoinRoomMessage msg)
        {
            Console.WriteLine("[JoinRoom] player=" + msg.PlayerName + " room=" + msg.RoomId);
            // TODO: kiểm tra phòng tồn tại, còn chỗ trống, đúng mật khẩu
            return PacketParser.Pack(CreateSuccessResponse(msg, "Tham gia phòng thành công"));
        }

        private byte[] HandleLeaveRoom(LeaveRoomMessage msg)
        {
            Console.WriteLine("[LeaveRoom] player=" + msg.PlayerId + " reason=" + msg.Reason);
            // TODO: xóa người chơi khỏi phòng, broadcast cho người còn lại
            return null;
        }

        private byte[] HandleInvite(InviteMessage msg)
        {
            Console.WriteLine("[Invite] target=" + msg.TargetPlayerId + " room=" + msg.RoomId);
            // TODO: forward InviteMessage tới đúng client đích
            return null;
        }

        private byte[] HandleMove(MoveMessage msg)
        {
            Console.WriteLine("[Move] player=" + msg.PlayerId + " (" + msg.Row + "," + msg.Column + ")");
            // TODO: validate nước đi, cập nhật bàn cờ, kiểm tra thắng/thua
            // rồi tạo GameStateMessage/TurnMessage/GameResultMessage để broadcast
            return null;
        }

        private byte[] HandleTurn(TurnMessage msg)
        {
            // Turn thường do Server tự tạo và gửi đi, ít khi Client gửi lên
            return null;
        }

        private byte[] HandleGameState(GameStateMessage msg)
        {
            return null;
        }

        private byte[] HandleGameResult(GameResultMessage msg)
        {
            return null;
        }

        private ResponseMessage CreateSuccessResponse(BaseMessage request, string data)
        {
            return new ResponseMessage
            {
                SenderId = "server",
                RequestMessageId = request.MessageId,
                Success = true,
                Data = data
            };
        }
    }
}
