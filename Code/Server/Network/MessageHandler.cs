using System;
using System.Threading.Tasks;
using CaroGame.Protocol;
using CaroGame.Protocol.Messages;
using CaroGame.Protocol.Messages.Room;
using CaroGame.Protocol.Messages.Game;
using CaroGame.Protocol.Messages.History;
using CaroGame.Protocol.Messages.System;
using CaroGame.Protocol.Messages.Response;
using Shared.Models;
using Server.Managers;
using Server.Services;
using Server.Utils;

namespace Server.Network;

/// <summary>
/// Chịu trách nhiệm định tuyến (Routing) các gói tin từ Client đến đúng Service xử lý.
/// </summary>
public class MessageHandler
{
    private readonly UserService _userService;
    private readonly RoomManager _roomManager;
    private readonly MatchManager _matchManager;
    private readonly ConnectionManager _connectionManager;
    private readonly GameRequestHandler _gameRequestHandler;
    private readonly MatchService _matchService;

    public MessageHandler(UserService userService, RoomManager roomManager, MatchManager matchManager, ConnectionManager connectionManager)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _roomManager = roomManager ?? throw new ArgumentNullException(nameof(roomManager));
        _matchManager = matchManager ?? throw new ArgumentNullException(nameof(matchManager));
        _connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));
        _gameRequestHandler = new GameRequestHandler(_matchManager, _connectionManager);
        _matchService = new MatchService();
    }

    /// <summary>
    /// Hàm chính để phân loại và xử lý BaseMessage từ Client
    /// </summary>
    public async Task ProcessMessageAsync(ClientSession session, BaseMessage message)
    {
        try
        {
            switch (message.Type)
            {
                case MessageType.Login:
                    if (message is LoginMessage loginMsg)
                        await HandleLoginAsync(session, loginMsg);
                    else
                        Logger.Warn($"[Network] Gói tin không đúng định dạng LoginMessage từ {session.SessionId}");
                    break;

                case MessageType.CreateRoom:
                    if (message is CreateRoomMessage createRoomMsg)
                        await HandleCreateRoomAsync(session, createRoomMsg);
                    break;

                case MessageType.JoinRoom:
                    if (message is JoinRoomMessage joinRoomMsg)
                        await HandleJoinRoomAsync(session, joinRoomMsg);
                    break;

                case MessageType.LeaveRoom:
                    if (message is LeaveRoomMessage leaveRoomMsg)
                        await HandleLeaveRoomAsync(session, leaveRoomMsg);
                    break;

                case MessageType.Move:
                    if (message is MoveMessage moveMsg)
                        await _gameRequestHandler.HandlePlayMoveAsync(session, moveMsg);
                    break;

                case MessageType.HistoryRequest:
                    if (message is HistoryRequestMessage historyReq)
                        await HandleHistoryRequestAsync(session, historyReq);
                    break;

                case MessageType.Pong:
                    if (message is PongMessage)
                        HandlePongMessage(session);
                    break;

                default:
                    Logger.Warn($"[Network] Không tìm thấy handler xử lý cho MessageType: {message.Type}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"[Network] Lỗi nội bộ khi xử lý {message.Type} từ {session.SessionId}", ex);

            ResponseMessage errorResponse = new ResponseMessage
            {
                SenderId = "Server",
                Success = false,
                ErrorMessage = "Đã xảy ra lỗi trên Server khi xử lý yêu cầu."
            };
            await session.SendAsync(errorResponse);
        }
    }

    /// <summary>
    /// Xử lý yêu cầu đăng nhập
    /// </summary>
    private async Task HandleLoginAsync(ClientSession session, LoginMessage loginMsg)
    {
        Logger.Info($"[Login] Processing login for user '{loginMsg.Username}' (Session: {session.SessionId})");

        // Gọi logic nghiệp vụ từ UserService
        bool isValid = _userService.Login(loginMsg.Username, loginMsg.Password);
        //bool isValid = true;

        // Chuẩn bị Response gửi trả Client
        ResponseMessage response = new ResponseMessage
        {
            SenderId = "Server",
            Success = isValid,
            ErrorMessage = isValid ? string.Empty : "Sai tên đăng nhập hoặc mật khẩu.",
            Data = isValid ? "Login thành công" : string.Empty
        };

        // Gửi kết quả lại cho Client
        await session.SendAsync(response);
    }

    private async Task HandleCreateRoomAsync(ClientSession session, CreateRoomMessage msg)
    {
        Logger.Info($"[CreateRoom] Yêu cầu từ Session: {session.SessionId}");
        var room = _roomManager.CreateRoom(msg.RoomName);

        var response = new ResponseMessage
        {
            SenderId = "Server",
            Success = true,
            Data = room.RoomId // Trả về RoomId để Client biết
        };
        await session.SendAsync(response);
    }

    private async Task HandleJoinRoomAsync(ClientSession session, JoinRoomMessage msg)
    {
        Logger.Info($"[JoinRoom] Yêu cầu từ Session: {session.SessionId} vào phòng {msg.RoomId}");

        var player = new Player(session.SessionId.ToString(), "Player_" + session.SessionId.ToString().Substring(0, 4));
        bool success = _roomManager.JoinRoom(msg.RoomId, player);

        var response = new ResponseMessage
        {
            SenderId = "Server",
            Success = success,
            ErrorMessage = success ? string.Empty : "Không thể tham gia phòng. Phòng đã đầy hoặc không tồn tại."
        };
        await session.SendAsync(response);

        if (success && _roomManager.CanStartGame(msg.RoomId))
        {
            var room = _roomManager.GetRoom(msg.RoomId);
            if (room != null && room.Players.Count == 2)
            {
                _roomManager.SetPlaying(room.RoomId, true);
                var match = _matchManager.CreateMatch(room.RoomId, room.Players[0], room.Players[1]);
                if (match != null)
                {
                    _matchManager.StartMatch(match.MatchId);
                    Logger.Info($"[Match] Đã tạo và bắt đầu trận đấu {match.MatchId} cho phòng {room.RoomId}");

                    var gameStateX = new GameSyncMessage
                    {
                        PlayerXName = room.Players[0].Id, // 👉 Đã sửa thành .Id
                        PlayerOName = room.Players[1].Id, // 👉 Đã sửa thành .Id
                        CurrentTurnName = match.CurrentTurn == CellState.X ? room.Players[0].Id : room.Players[1].Id, // 👉 Đã sửa thành .Id
                        MySymbol = "X"
                    };

                    // Phát GameSyncMessage cho người chơi 2 (O)
                    var gameStateO = new GameSyncMessage
                    {
                        PlayerXName = room.Players[0].Id, // 👉 Đã sửa thành .Id
                        PlayerOName = room.Players[1].Id, // 👉 Đã sửa thành .Id
                        CurrentTurnName = match.CurrentTurn == CellState.X ? room.Players[0].Id : room.Players[1].Id, // 👉 Đã sửa thành .Id
                        MySymbol = "O"
                    };

                    await _connectionManager.SendMessageToClientAsync(room.Players[0].Id, gameStateX);
                    await _connectionManager.SendMessageToClientAsync(room.Players[1].Id, gameStateO);
                }
            }
        }
    }

    private async Task HandleLeaveRoomAsync(ClientSession session, LeaveRoomMessage msg)
    {
        Logger.Info($"[LeaveRoom] Yêu cầu từ Session: {session.SessionId} rời phòng {msg.RoomId}");
        bool success = _roomManager.LeaveRoom(msg.RoomId, session.SessionId.ToString());

        var response = new ResponseMessage
        {
            SenderId = "Server",
            Success = success,
            ErrorMessage = success ? string.Empty : "Không thể rời phòng."
        };
        await session.SendAsync(response);
    }

    private async Task HandleHistoryRequestAsync(ClientSession session, HistoryRequestMessage msg)
    {
        Logger.Info($"[HistoryRequest] User {msg.UserId} yêu cầu lịch sử đấu từ Session: {session.SessionId}");

        // Gọi xuống DB thông qua MatchService
        var dt = _matchService.GetUserMatchHistory(msg.UserId);

        var response = new HistoryResponseMessage();
        if (dt != null)
        {
            foreach (System.Data.DataRow row in dt.Rows)
            {
                response.Matches.Add(new MatchHistoryItem
                {
                    MatchId = Convert.ToInt32(row["MatchId"]),
                    Player1Id = row["Player1Id"] != DBNull.Value ? Convert.ToInt32(row["Player1Id"]) : 0,
                    Player2Id = row["Player2Id"] != DBNull.Value ? Convert.ToInt32(row["Player2Id"]) : 0,
                    StartTime = Convert.ToDateTime(row["StartTime"]),
                    EndTime = row["EndTime"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["EndTime"]) : null,
                    WinnerId = row["WinnerId"] != DBNull.Value ? (int?)Convert.ToInt32(row["WinnerId"]) : null,
                    Result = row["Result"]?.ToString() ?? "",
                    Status = row["Status"]?.ToString() ?? ""
                });
            }
        }
        await session.SendAsync(response);
    }

    private void HandlePongMessage(ClientSession session)
    {
        // Cập nhật thời gian nhận Pong cuối cùng
        session.LastPongTime = DateTime.Now;
    }
}