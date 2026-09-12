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
        _gameRequestHandler = new GameRequestHandler(_matchManager, _roomManager, _connectionManager);
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

                case MessageType.Register:
                    if (message is RegisterMessage registerMsg)
                        await HandleRegisterAsync(session, registerMsg);
                    else
                        Logger.Warn($"[Network] Gói tin không đúng định dạng RegisterMessage từ {session.SessionId}");
                    break;

                case MessageType.Request:
                    if (message is RequestMessage reqMsg)
                        await HandleRequestAsync(session, reqMsg);
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

                case MessageType.StartMatch:
                    if (message is StartMatchMessage startMatchMsg)
                        await HandleStartMatchAsync(session, startMatchMsg);
                    break;

                case MessageType.Invite:
                    if (message is InviteMessage inviteMsg)
                        await HandleInviteAsync(session, inviteMsg);
                    break;

                case MessageType.Ready:
                    if (message is ReadyMessage readyMsg)
                        await HandleReadyAsync(session, readyMsg);
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

        if (isValid)
        {
            // Update Session PlayerName
            session.PlayerName = loginMsg.Username;
            await BroadcastLobbyStateAsync();
        }
    }

    /// <summary>
    /// Xử lý yêu cầu đăng ký
    /// </summary>
    private async Task HandleRegisterAsync(ClientSession session, RegisterMessage registerMsg)
    {
        Logger.Info($"[Register] Processing registration for user '{registerMsg.Username}' (Session: {session.SessionId})");

        bool isValid = _userService.Register(registerMsg.Username, registerMsg.Password);

        ResponseMessage response = new ResponseMessage
        {
            SenderId = "Server",
            Success = isValid,
            ErrorMessage = isValid ? string.Empty : "Tên đăng nhập đã tồn tại hoặc có lỗi xảy ra.",
            Data = isValid ? "Register thành công" : string.Empty
        };

        await session.SendAsync(response);
    }

    private async Task HandleCreateRoomAsync(ClientSession session, CreateRoomMessage msg)
    {
        Logger.Info($"[CreateRoom] Yêu cầu từ Session: {session.SessionId}");
        var room = _roomManager.CreateRoom(msg.RoomName);

        // Tự động add chủ phòng vào phòng
        string pName = !string.IsNullOrEmpty(session.PlayerName) ? session.PlayerName : "Player_" + session.SessionId.ToString().Substring(0, 4);
        var hostPlayer = new Player(session.SessionId.ToString(), pName);
        _roomManager.JoinRoom(room.RoomId, hostPlayer);

        var response = new ResponseMessage
        {
            SenderId = "Server",
            Success = true,
            Data = room.RoomId // Trả về RoomId để Client biết
        };
        await session.SendAsync(response);
        await BroadcastLobbyStateAsync();
        await BroadcastRoomStateAsync(room);
    }

    private async Task HandleJoinRoomAsync(ClientSession session, JoinRoomMessage msg)
    {
        Logger.Info($"[JoinRoom] Yêu cầu từ Session: {session.SessionId} vào phòng {msg.RoomId}");

        string pName = !string.IsNullOrEmpty(session.PlayerName) ? session.PlayerName : "Player_" + session.SessionId.ToString().Substring(0, 4);
        var player = new Player(session.SessionId.ToString(), pName);
        bool success = _roomManager.JoinRoom(msg.RoomId, player);

        var response = new ResponseMessage
        {
            SenderId = "Server",
            Success = success,
            ErrorMessage = success ? string.Empty : "Không thể tham gia phòng. Phòng đã đầy hoặc không tồn tại."
        };
        await session.SendAsync(response);
        if (success)
        {
            await BroadcastLobbyStateAsync();
            var room = _roomManager.GetRoom(msg.RoomId);
            if (room != null)
                await BroadcastRoomStateAsync(room);
        }
    }

    private async Task HandleStartMatchAsync(ClientSession session, StartMatchMessage msg)
    {
        Logger.Info($"[StartMatch] Yêu cầu từ Session: {session.SessionId} bắt đầu phòng {msg.RoomId}");

        if (_roomManager.CanStartGame(msg.RoomId))
        {
            var room = _roomManager.GetRoom(msg.RoomId);
            if (room != null && room.Players.Count == 2)
            {
                // Verify that the sender is the host (Player 1)
                if (room.Players[0].Id != session.SessionId.ToString())
                {
                    Logger.Warn($"[StartMatch] Từ chối: Session {session.SessionId} không phải chủ phòng {msg.RoomId}");
                    return;
                }

                if (!room.Players[1].IsReady)
                {
                    Logger.Warn($"[StartMatch] Từ chối: Người chơi O chưa sẵn sàng.");
                    return;
                }

                _roomManager.SetPlaying(room.RoomId, true);
                var match = _matchManager.CreateMatch(room.RoomId, room.Players[0], room.Players[1], room.BoardSize);
                if (match != null)
                {
                    _matchManager.StartMatch(match.MatchId);
                    Logger.Info($"[Match] Đã tạo và bắt đầu trận đấu {match.MatchId} cho phòng {room.RoomId}");

                    var gameStateX = new GameStateMessage
                    {
                        RoomId = room.RoomId,
                        BoardState = string.Empty, // Bàn cờ trống lúc mới bắt đầu
                        BoardSize = room.BoardSize,
                        CurrentPlayerId = match.CurrentTurn == CellState.X ? room.Players[0].Id : room.Players[1].Id,
                        CurrentTurnName = match.CurrentTurn == CellState.X ? room.Players[0].Username : room.Players[1].Username,
                        PlayerXName = room.Players[0].Username,
                        PlayerOName = room.Players[1].Username,
                        Status = "Playing",
                        MySymbol = "X"
                    };

                    var gameStateO = new GameStateMessage
                    {
                        RoomId = room.RoomId,
                        BoardState = string.Empty,
                        BoardSize = room.BoardSize,
                        CurrentPlayerId = match.CurrentTurn == CellState.X ? room.Players[0].Id : room.Players[1].Id,
                        CurrentTurnName = match.CurrentTurn == CellState.X ? room.Players[0].Username : room.Players[1].Username,
                        PlayerXName = room.Players[0].Username,
                        PlayerOName = room.Players[1].Username,
                        Status = "Playing",
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
        
        var room = _roomManager.GetRoom(msg.RoomId);
        bool success = _roomManager.LeaveRoom(msg.RoomId, session.SessionId.ToString());

        var response = new ResponseMessage
        {
            SenderId = "Server",
            Success = success,
            ErrorMessage = success ? string.Empty : "Không thể rời phòng."
        };
        await session.SendAsync(response);
        if (success)
        {
            await BroadcastLobbyStateAsync();
            // Nếu phòng vẫn tồn tại sau khi người này rời (nghĩa là còn người ở lại)
            if (_roomManager.RoomExists(msg.RoomId) && room != null)
            {
                await BroadcastRoomStateAsync(room);
            }
        }
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

    private async Task HandleInviteAsync(ClientSession session, InviteMessage request)
    {
        Logger.Info($"[Invite] User {session.SessionId} mời {request.TargetPlayerId} vào phòng {request.RoomId}");

        // Chuyển tiếp lời mời tới người nhận nếu họ đang online
        ClientSession? targetSession = null;
        if (Guid.TryParse(request.TargetPlayerId, out Guid targetGuid))
        {
            targetSession = _connectionManager.Get(targetGuid);
        }
        
        if (targetSession == null)
        {
            targetSession = _connectionManager.GetAll().FirstOrDefault(s => s.PlayerName == request.TargetPlayerId);
        }

        if (targetSession != null)
        {
            await targetSession.SendAsync(request);
        }
        else
        {
            var errorResponse = new ResponseMessage
            {
                SenderId = "Server",
                Success = false,
                ErrorMessage = $"Người chơi không online hoặc không tồn tại."
            };
            await session.SendAsync(errorResponse);
        }
    }

    private void HandlePongMessage(ClientSession session)
    {
        // Cập nhật thời gian nhận Pong cuối cùng
        session.LastPongTime = DateTime.Now;
    }

    public async Task BroadcastLobbyStateAsync()
    {
        var players = _connectionManager.GetAllPlayerNames();
        var roomInfos = new System.Collections.Generic.List<Shared.Models.RoomInfo>();
        foreach (var room in _roomManager.GetRooms())
        {
            roomInfos.Add(new Shared.Models.RoomInfo {
                RoomId = room.RoomId,
                RoomName = room.RoomName,
                CurrentPlayers = room.Players.Count,
                MaxPlayers = room.MaxPlayers,
                IsPlaying = room.IsPlaying
            });
        }

        var lobbyData = new Shared.Models.LobbyStateDto
        {
            OnlineCount = _connectionManager.Count,
            OnlinePlayers = players,
            Rooms = roomInfos
        };

        var response = new ResponseMessage
        {
            SenderId = "Server",
            Success = true,
            Data = System.Text.Json.JsonSerializer.Serialize(lobbyData)
        };

        await _connectionManager.BroadcastAsync(response);
    }

    private async Task BroadcastRoomStateAsync(Room room)
    {
        if (room == null) return;

        var stateDto = new CaroGame.Protocol.Messages.RoomStateDto
        {
            RoomId = room.RoomId,
            RoomName = room.RoomName,
            PlayerX = room.Players.Count > 0 ? room.Players[0].Username : "",
            PlayerO = room.Players.Count > 1 ? room.Players[1].Username : "",
            IsPlayerOReady = room.Players.Count > 1 ? room.Players[1].IsReady : false,
            BoardSize = room.BoardSize
        };

        var response = new ResponseMessage
        {
            SenderId = "Server",
            Success = true,
            Action = "RoomStateUpdate",
            Data = System.Text.Json.JsonSerializer.Serialize(stateDto)
        };

        foreach (var player in room.Players)
        {
            await _connectionManager.SendMessageToClientAsync(player.Id, response);
        }
    }

    private async Task HandleReadyAsync(ClientSession session, ReadyMessage msg)
    {
        Logger.Info($"[Ready] Yêu cầu từ Session: {session.SessionId} trong phòng {msg.RoomId}, trạng thái: {msg.IsReady}");
        var room = _roomManager.GetRoom(msg.RoomId);
        if (room != null)
        {
            var player = room.GetPlayer(session.SessionId.ToString());
            if (player != null)
            {
                player.IsReady = msg.IsReady;
                await BroadcastRoomStateAsync(room);
            }
        }
    }

    private async Task HandleRequestAsync(ClientSession session, RequestMessage msg)
    {
        if (msg.Action == "SpectatorJoin")
        {
            var room = _roomManager.GetRoom(msg.Data);
            if (room == null)
                return;

            string sessionId = session.SessionId.ToString();
            if (!room.SpectatorSessionIds.Contains(sessionId))
                room.SpectatorSessionIds.Add(sessionId);

            var match = _matchManager.GetMatch(room.RoomId);
            if (match != null && room.Players.Count == 2)
            {
                var syncMsg = new GameSyncMessage
                {
                    PlayerXName = room.Players[0].Username,
                    PlayerOName = room.Players[1].Username,
                    CurrentTurnName = match.CurrentTurn == CellState.X
                        ? room.Players[0].Username
                        : room.Players[1].Username,
                    MySymbol = "Spectator"
                };
                await _connectionManager.SendMessageToClientAsync(sessionId, syncMsg);
            }
        }
        else if (msg.Action == "SpectatorLeave")
        {
            var room = _roomManager.GetRoom(msg.Data);
            room?.SpectatorSessionIds.Remove(session.SessionId.ToString());
        }
        else if (msg.Action == "UpdateBoardSize")
        {
            var room = _roomManager.FindPlayerRoom(session.SessionId.ToString());
            if (room != null && room.Players.Count > 0 && room.Players[0].Id == session.SessionId.ToString())
            {
                if (int.TryParse(msg.Data, out int newSize))
                {
                    room.BoardSize = newSize;
                    Logger.Info($"[Room] Cập nhật BoardSize={newSize} cho phòng {room.RoomId}");
                    await BroadcastRoomStateAsync(room);
                }
            }
        }
    }
}