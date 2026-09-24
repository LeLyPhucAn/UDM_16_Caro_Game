using System;
using System.Threading.Tasks;
using CaroGame.Protocol;
using CaroGame.Protocol.Messages;
using CaroGame.Protocol.Messages.Room;
using CaroGame.Protocol.Messages.Game;
using CaroGame.Protocol.Messages.History;
using CaroGame.Protocol.Messages.Response;
using Shared.Models;
using Server.Managers;
using Server.Services;
using Server.Utils;
using Server.AI;

namespace Server.Network;

/// <summary>
/// Xử lý và phân loại các gói tin nhận từ Client.
/// </summary>
public class MessageHandler
{
    private readonly UserService _userService;
    private readonly RoomManager _roomManager;
    private readonly MatchManager _matchManager;
    private readonly ConnectionManager _connectionManager;
    private readonly MatchService _matchService;
    private readonly ReconnectManager _reconnectManager;

    public MessageHandler(UserService userService, RoomManager roomManager, MatchManager matchManager, ConnectionManager connectionManager, ReconnectManager reconnectManager)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _roomManager = roomManager ?? throw new ArgumentNullException(nameof(roomManager));
        _matchManager = matchManager ?? throw new ArgumentNullException(nameof(matchManager));
        _connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));
        _reconnectManager = reconnectManager ?? throw new ArgumentNullException(nameof(reconnectManager));
        _matchService = new MatchService();
        _matchManager.OnMatchTimeout += HandleMatchTimeout;
        _reconnectManager.OnGraceExpired += HandleGraceExpiredAsync;
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
                        await HandlePlayMoveAsync(session, moveMsg);
                    break;

                case MessageType.Chat:
                    if (message is ChatMessage chatMsg)
                        await HandleChatAsync(session, chatMsg);
                    break;

                case MessageType.HistoryRequest:
                    if (message is HistoryRequestMessage historyReq)
                        await HandleHistoryRequestAsync(session, historyReq);
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
        bool isValid = _userService.Login(loginMsg.Username, loginMsg.Password, out System.Data.DataRow? userRow);

        // Chuẩn bị Response gửi trả Client
        ResponseMessage response = new ResponseMessage
        {
            SenderId = "Server",
            Success = isValid,
            Action = "Login",
            ErrorMessage = isValid ? string.Empty : "Sai tên đăng nhập hoặc mật khẩu.",
            Data = isValid ? "Login thành công" : string.Empty
        };

        // Gửi kết quả lại cho Client
        await session.SendAsync(response);

        if (isValid)
        {
            // Update Session PlayerName & UserId
            session.PlayerName = loginMsg.Username;
            if (userRow != null && userRow.Table.Columns.Contains("Id") && userRow["Id"] != DBNull.Value)
            {
                session.UserId = Convert.ToInt32(userRow["Id"]);
            }

            // Kiểm tra xem người dùng có đang trong grace period (chờ reconnect) không
            bool reconnected = await TryReconnectAsync(session);
            if (!reconnected)
            {
                // Không phải reconnect → lobby state bình thường
                await BroadcastLobbyStateAsync();
            }
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
            Action = "Register",
            ErrorMessage = isValid ? string.Empty : "Tên đăng nhập đã tồn tại hoặc có lỗi xảy ra.",
            Data = isValid ? "Register thành công" : string.Empty
        };

        await session.SendAsync(response);
    }

    private async Task HandleCreateRoomAsync(ClientSession session, CreateRoomMessage msg)
    {
        Logger.Info($"[CreateRoom] Yêu cầu từ Session: {session.SessionId}");

        // Kiểm tra nếu session đang ở trong phòng khác thì rời phòng cũ trước
        var existingRoom = _roomManager.FindPlayerRoom(session.SessionId.ToString());
        if (existingRoom != null)
        {
            await ProcessPlayerLeaveAsync(session, existingRoom.RoomId);
        }

        var room = _roomManager.CreateRoom(msg.RoomName);
        room.BoardSize = 15;

        // Tự động add chủ phòng vào phòng
        string pName = !string.IsNullOrEmpty(session.PlayerName) ? session.PlayerName : "Player_" + session.SessionId.ToString().Substring(0, 4);
        var hostPlayer = new Player(session.SessionId.ToString(), pName);
        hostPlayer.DatabaseId = session.UserId;
        _roomManager.JoinRoom(room.RoomId, hostPlayer);

        var response = new ResponseMessage
        {
            SenderId = "Server",
            Success = true,
            Action = "CreateRoom",
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
        player.DatabaseId = session.UserId;

        bool success = false;
        var targetRoom = _roomManager.GetRoom(msg.RoomId);
        bool isAlreadyInRoom = targetRoom != null && targetRoom.Players.Any(p => p.Id == player.Id);

        if (isAlreadyInRoom && !msg.IsSpectator)
        {
            success = true; // Người chơi đang ở trong phòng (có thể đã LeaveRoom về Lobby lúc đang đấu)
        }
        else
        {
            success = _roomManager.JoinRoom(msg.RoomId, player, msg.IsSpectator);
        }

        var response = new ResponseMessage
        {
            SenderId = "Server",
            Success = success,
            Action = "JoinRoom",
            ErrorMessage = success ? string.Empty : "Không thể tham gia phòng. Phòng đã đầy hoặc không tồn tại."
        };
        await session.SendAsync(response);
        if (success)
        {
            await BroadcastLobbyStateAsync();
            var room = _roomManager.GetRoom(msg.RoomId);
            if (room != null)
            {
                await BroadcastRoomStateAsync(room);

                // Neu phong dang choi, xử lý reconnect cho player hoặc cho spectator vào xem
                if (room.IsPlaying)
                {
                    var match = _matchManager.FindRoomMatch(room.RoomId);
                    if (match != null)
                    {
                        bool isReconnectingPlayer = (match.PlayerX?.Id == session.SessionId.ToString() || match.PlayerO?.Id == session.SessionId.ToString());

                        if (msg.IsSpectator || isReconnectingPlayer)
                        {
                            if (isReconnectingPlayer && match.State == MatchState.Suspended)
                            {
                                // Hủy đếm ngược grace period, tiếp tục trận đấu
                                _reconnectManager.CancelGrace(session.SessionId.ToString());
                                match.Resume();
                                _matchManager.ResetTimer(match.MatchId);

                                var opp = room.Players.FirstOrDefault(p => p.Id != session.SessionId.ToString());
                                if (opp != null)
                                {
                                    await _connectionManager.SendMessageToClientAsync(opp.Id, new ResponseMessage { Action = "OpponentReconnected" });
                                }
                            }

                            var sb = new System.Text.StringBuilder(match.Board.Rows * match.Board.Columns);
                            for (int r = 0; r < match.Board.Rows; r++)
                            {
                                for (int c = 0; c < match.Board.Columns; c++)
                                {
                                    var cell = match.Board.GetCell(r, c);
                                    sb.Append(cell == Shared.Models.CellState.X ? 'X' : (cell == Shared.Models.CellState.O ? 'O' : '-'));
                                }
                            }

                            Player? pX = match.PlayerX ?? (room.Players.Count > 0 ? room.Players[0] : null);
                            Player? pO = match.PlayerO ?? (room.Players.Count > 1 ? room.Players[1] : null);

                            var syncMsg = new GameStateMessage
                            {
                                RoomId = room.RoomId,
                                BoardState = sb.ToString(),
                                BoardSize = room.BoardSize,
                                CurrentPlayerId = match.CurrentTurn == Shared.Models.CellState.X ? (pX?.Id ?? "") : (pO?.Id ?? ""),
                                CurrentTurnName = match.CurrentTurn == Shared.Models.CellState.X ? (pX?.Username ?? "") : (pO?.Username ?? ""),
                                PlayerXName = pX?.Username ?? "",
                                PlayerOName = pO?.Username ?? "",
                                Status = "Playing",
                                MySymbol = "S",
                                SpectatorCount = room.Spectators.Count
                            };

                            await session.SendAsync(syncMsg);
                        }
                    }
                }
            }
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

                Player playerX = (room.HostSymbol == "X") ? room.Players[0] : room.Players[1];
                Player playerO = (room.HostSymbol == "O") ? room.Players[0] : room.Players[1];

                var match = _matchManager.CreateMatch(room.RoomId, playerX, playerO, room.BoardSize);
                if (match != null)
                {
                    int p1DbId = playerX.DatabaseId > 0
                        ? playerX.DatabaseId
                        : _userService.GetUserId(playerX.Username);
                    int p2DbId = playerO.DatabaseId > 0
                        ? playerO.DatabaseId
                        : _userService.GetUserId(playerO.Username);

                    playerX.DatabaseId = p1DbId;
                    playerO.DatabaseId = p2DbId;

                    if (match.PlayerX != null) match.PlayerX.DatabaseId = p1DbId;
                    if (match.PlayerO != null) match.PlayerO.DatabaseId = p2DbId;

                    if (p1DbId > 0 && p2DbId > 0)
                    {
                        match.DbMatchId = _matchService.StartNewMatch(p1DbId, p2DbId);
                        Logger.Info($"[Match] Da tao ban ghi Match #{match.DbMatchId} trong CSDL cho {playerX.Username}({p1DbId}) vs {playerO.Username}({p2DbId})");
                    }

                    _matchManager.StartMatch(match.MatchId);
                    Logger.Info($"[Match] Da tao va bat dau tran dau {match.MatchId} cho phong {room.RoomId}");

                    string hostSymbol = room.HostSymbol;
                    string guestSymbol = (hostSymbol == "X") ? "O" : "X";

                    var gameStateHost = new GameStateMessage
                    {
                        RoomId = room.RoomId,
                        BoardState = string.Empty,
                        BoardSize = room.BoardSize,
                        CurrentPlayerId = match.CurrentTurn == CellState.X ? playerX.Id : playerO.Id,
                        CurrentTurnName = match.CurrentTurn == CellState.X ? playerX.Username : playerO.Username,
                        PlayerXName = playerX.Username,
                        PlayerOName = playerO.Username,
                        Status = "Playing",
                        MySymbol = hostSymbol,
                        SpectatorCount = room.Spectators.Count
                    };

                    var gameStateGuest = new GameStateMessage
                    {
                        RoomId = room.RoomId,
                        BoardState = string.Empty,
                        BoardSize = room.BoardSize,
                        CurrentPlayerId = match.CurrentTurn == CellState.X ? playerX.Id : playerO.Id,
                        CurrentTurnName = match.CurrentTurn == CellState.X ? playerX.Username : playerO.Username,
                        PlayerXName = playerX.Username,
                        PlayerOName = playerO.Username,
                        Status = "Playing",
                        MySymbol = guestSymbol,
                        SpectatorCount = room.Spectators.Count
                    };

                    await _connectionManager.SendMessageToClientAsync(room.Players[0].Id, gameStateHost);
                    await _connectionManager.SendMessageToClientAsync(room.Players[1].Id, gameStateGuest);

                    var gameStateSpectator = new GameStateMessage
                    {
                        RoomId = room.RoomId,
                        BoardState = string.Empty,
                        BoardSize = room.BoardSize,
                        CurrentPlayerId = match.CurrentTurn == CellState.X ? playerX.Id : playerO.Id,
                        CurrentTurnName = match.CurrentTurn == CellState.X ? playerX.Username : playerO.Username,
                        PlayerXName = playerX.Username,
                        PlayerOName = playerO.Username,
                        Status = "Playing",
                        MySymbol = "S",
                        SpectatorCount = room.Spectators.Count
                    };
                    foreach (var spectator in room.Spectators)
                    {
                        await _connectionManager.SendMessageToClientAsync(spectator.Id, gameStateSpectator);
                    }
                }
            }
        }
    }

    private async Task HandleLeaveRoomAsync(ClientSession session, LeaveRoomMessage msg)
    {
        Logger.Info($"[LeaveRoom] Yeu cau tu Session: {session.SessionId} roi phong {msg.RoomId}");
        await ProcessPlayerLeaveAsync(session, msg.RoomId);
    }

    private async Task HandleHistoryRequestAsync(ClientSession session, HistoryRequestMessage msg)
    {
        Logger.Info($"[HistoryRequest] User {msg.Username} yêu cầu lịch sử đấu từ Session: {session.SessionId}");

        // Gọi xuống DB thông qua MatchService
        int userId = _userService.GetUserId(msg.Username);
        var dt = _matchService.GetUserMatchHistory(userId);

        var response = new HistoryResponseMessage();
        if (dt != null)
        {
            foreach (System.Data.DataRow row in dt.Rows)
            {
                int matchWinnerId = row["WinnerId"] != DBNull.Value ? Convert.ToInt32(row["WinnerId"]) : 0;
                string outcome = "Chưa rõ";
                string rawRes = row["Result"]?.ToString() ?? "";
                if (row["WinnerId"] != DBNull.Value)
                {
                    outcome = (matchWinnerId == userId) ? "Thắng" : "Thua";
                }
                else
                {
                    if (rawRes.Contains("Draw", StringComparison.OrdinalIgnoreCase))
                        outcome = "Hòa";
                    else if (rawRes.Contains("Cancel", StringComparison.OrdinalIgnoreCase))
                        outcome = "Hủy";
                    else
                        outcome = rawRes;
                }

                // Map mã kết quả sang ghi chú tiếng Việt thân thiện
                string note;
                if (rawRes.Equals("Timeout", StringComparison.OrdinalIgnoreCase))
                    note = "Hết giờ";
                else if (rawRes.Equals("Surrender", StringComparison.OrdinalIgnoreCase))
                    note = "Đầu hàng";
                else if (rawRes.Equals("Disconnect", StringComparison.OrdinalIgnoreCase))
                    note = "Mất kết nối";
                else if (rawRes.Equals("Win", StringComparison.OrdinalIgnoreCase))
                    note = "5 quân thẳng hàng";
                else if (rawRes.Equals("Draw", StringComparison.OrdinalIgnoreCase))
                    note = "Hòa cờ";
                else
                    note = string.IsNullOrWhiteSpace(rawRes) ? (row["Status"]?.ToString() ?? "Hoàn thành") : rawRes;

                response.Matches.Add(new MatchHistoryItem
                {
                    MatchId = Convert.ToInt32(row["MatchId"]),
                    Player1Id = row["Player1Id"] != DBNull.Value ? Convert.ToInt32(row["Player1Id"]) : 0,
                    Player2Id = row["Player2Id"] != DBNull.Value ? Convert.ToInt32(row["Player2Id"]) : 0,
                    StartTime = Convert.ToDateTime(row["StartTime"]),
                    EndTime = row["EndTime"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["EndTime"]) : null,
                    WinnerId = row["WinnerId"] != DBNull.Value ? (int?)Convert.ToInt32(row["WinnerId"]) : null,
                    Result = outcome,
                    Status = note
                });
            }
        }
        await session.SendAsync(response);
    }

    private async void HandleMatchTimeout(Match match, string winnerId, string winnerName)
    {
        try
        {
            var room = _roomManager.GetRoom(match.RoomId);
            var gameOverMsg = new GameOverMessage
            {
                RoomId = match.MatchId,
                ResultType = "Timeout",
                WinnerId = winnerId,
                WinnerName = winnerName,
                WinningLine = new string[0]
            };

            if (room != null)
            {
                foreach (var p in room.Players)
                    await _connectionManager.SendMessageToClientAsync(p.Id, gameOverMsg);
                foreach (var p in room.Spectators)
                    await _connectionManager.SendMessageToClientAsync(p.Id, gameOverMsg);
            }

            int? dbWinnerId = (match.PlayerX?.Id == winnerId) ? match.PlayerX?.DatabaseId : match.PlayerO?.DatabaseId;
            _matchService.SaveMatchResult(match.DbMatchId, dbWinnerId, "Timeout");
        }
        catch (Exception ex)
        {
            Logger.Error($"[MatchTimeout Error] {ex.Message}");
        }
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
            // Kiểm tra xem đối phương có đang bận (đang trong phòng khác) không
            var busyRoom = _roomManager.FindPlayerRoom(targetSession.SessionId.ToString());
            if (busyRoom != null)
            {
                var busyResponse = new ResponseMessage
                {
                    SenderId = "Server",
                    Success = false,
                    Action = "Invite",
                    ErrorMessage = $"Người chơi '{request.TargetPlayerId}' hiện đang trong một phòng khác."
                };
                await session.SendAsync(busyResponse);
                return;
            }

            await targetSession.SendAsync(request);
        }
        else
        {
            var errorResponse = new ResponseMessage
            {
                SenderId = "Server",
                Success = false,
                Action = "Invite",
                ErrorMessage = $"Người chơi không online hoặc không tồn tại."
            };
            await session.SendAsync(errorResponse);
        }
    }


    public async Task BroadcastLobbyStateAsync()
    {
        var players = _connectionManager.GetAllPlayerNames();
        var roomInfos = new System.Collections.Generic.List<CaroGame.Protocol.RoomInfo>();
        foreach (var room in _roomManager.GetRooms())
        {
            roomInfos.Add(new CaroGame.Protocol.RoomInfo
            {
                RoomId = room.RoomId,
                RoomName = room.RoomName,
                CurrentPlayers = room.Players.Count,
                MaxPlayers = room.MaxPlayers,
                IsPlaying = room.IsPlaying
            });
        }

        var lobbyData = new CaroGame.Protocol.LobbyStateDto
        {
            OnlineCount = _connectionManager.Count,
            OnlinePlayers = players,
            Rooms = roomInfos
        };

        var response = new ResponseMessage
        {
            SenderId = "Server",
            Success = true,
            Action = "LobbyStateUpdate",
            Data = System.Text.Json.JsonSerializer.Serialize(lobbyData)
        };

        await _connectionManager.BroadcastAsync(response);
    }

    private async Task BroadcastRoomStateAsync(Room room)
    {
        if (room == null) return;

        string hostName = room.Players.Count > 0 ? room.Players[0].Username : "";
        string guestName = room.Players.Count > 1 ? room.Players[1].Username : "";
        string hostSymbol = room.HostSymbol;
        string guestSymbol = (hostSymbol == "X") ? "O" : "X";
        bool isGuestReady = room.Players.Count > 1 && room.Players[1].IsReady;

        var stateDto = new CaroGame.Protocol.Messages.RoomStateDto
        {
            RoomId = room.RoomId,
            RoomName = room.RoomName,
            HostName = hostName,
            GuestName = guestName,
            HostSymbol = hostSymbol,
            GuestSymbol = guestSymbol,
            IsGuestReady = isGuestReady,
            PlayerX = (hostSymbol == "X") ? hostName : guestName,
            PlayerO = (hostSymbol == "O") ? hostName : guestName,
            IsPlayerOReady = isGuestReady,
            BoardSize = room.BoardSize,
            SpectatorCount = room.Spectators.Count
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
        foreach (var spectator in room.Spectators)
        {
            await _connectionManager.SendMessageToClientAsync(spectator.Id, response);
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
        if (msg.Action == "UpdateBoardSize")
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
        else if (msg.Action == "SwapSymbol")
        {
            var room = _roomManager.FindPlayerRoom(session.SessionId.ToString());
            if (room != null)
            {
                room.SwapPlayers();
                Logger.Info($"[Room] Doi quan trong phong {room.RoomId}");
                await BroadcastRoomStateAsync(room);
            }
        }
        else if (msg.Action == "LeaveRoom")
        {
            await ProcessPlayerLeaveAsync(session);
        }
        else if (msg.Action == "RematchRequest")
        {
            var room = _roomManager.FindPlayerRoom(session.SessionId.ToString());
            if (room != null)
            {
                var match = _matchManager.FindRoomMatch(room.RoomId);
                if (match != null && match.IsFinished())
                {
                    // Phòng Bot: tự động chấp nhận tái đấu
                    if (room.IsBotRoom)
                    {
                        await HandleBotRematchAsync(session, room, match);
                        return;
                    }

                    string opponentId = (match.PlayerX?.Id.ToString() == session.SessionId.ToString())
                        ? match.PlayerO?.Id.ToString() ?? ""
                        : match.PlayerX?.Id.ToString() ?? "";

                    var req = new ResponseMessage { SenderId = "Server", Success = true, Action = "OpponentRematchRequest" };
                    if (!string.IsNullOrEmpty(opponentId))
                        await _connectionManager.SendMessageToClientAsync(opponentId, req);
                }
            }
        }
        else if (msg.Action == "RematchAccepted")
        {
            var room = _roomManager.FindPlayerRoom(session.SessionId.ToString());
            if (room != null)
            {
                _roomManager.SetPlaying(room.RoomId, false);
                room.HostSymbol = (room.HostSymbol == "X") ? "O" : "X"; // Swap roles for next match

                // Đồng bộ: Đặt lại trạng thái Sẵn Sàng của tất cả người chơi trong phòng về false
                // Bắt buộc người chơi phải tự click Sẵn Sàng/Bắt đầu lại, tránh việc StartGame sớm khi chưa chuyển cảnh.
                foreach (var p in room.Players)
                {
                    p.IsReady = false;
                }

                await BroadcastRoomStateAsync(room);

                var response = new ResponseMessage
                {
                    SenderId = "Server",
                    Success = true,
                    Action = "RematchAccepted",
                    Data = room.RoomId
                };
                foreach (var p in room.Players)
                    await _connectionManager.SendMessageToClientAsync(p.Id, response);
                foreach (var p in room.Spectators)
                    await _connectionManager.SendMessageToClientAsync(p.Id, response);
            }
        }
        else if (msg.Action == "RematchDeclined")
        {
            var room = _roomManager.FindPlayerRoom(session.SessionId.ToString());
            if (room != null)
            {
                var match = _matchManager.FindRoomMatch(room.RoomId);
                if (match != null && match.IsFinished())
                {
                    string opponentId = (match.PlayerX?.Id.ToString() == session.SessionId.ToString())
                        ? match.PlayerO?.Id.ToString() ?? ""
                        : match.PlayerX?.Id.ToString() ?? "";

                    var req = new ResponseMessage { SenderId = "Server", Success = true, Action = "RematchDeclined" };
                    if (!string.IsNullOrEmpty(opponentId))
                        await _connectionManager.SendMessageToClientAsync(opponentId, req);
                }
            }
        }
        else if (msg.Action == "Surrender")
        {
            var room = _roomManager.FindPlayerRoom(session.SessionId.ToString());
            if (room != null)
            {
                var match = _matchManager.FindRoomMatch(room.RoomId);
                if (match != null && match.IsPlaying())
                {
                    string opponentId = (match.PlayerX?.Id.ToString() == session.SessionId.ToString())
                        ? match.PlayerO?.Id.ToString() ?? ""
                        : match.PlayerX?.Id.ToString() ?? "";

                    string loserId = (match.PlayerX?.Id.ToString() == session.SessionId.ToString())
                        ? match.PlayerX?.Id.ToString() ?? ""
                        : match.PlayerO?.Id.ToString() ?? "";

                    string opponentName = (match.PlayerX?.Id.ToString() == opponentId) ? (match.PlayerX?.Username ?? string.Empty) : (match.PlayerO?.Username ?? string.Empty);

                    _matchManager.EndMatch(match.MatchId, opponentId, loserId, Shared.Enums.GameResultType.Surrender, "Surrender");

                    var gameOverMsg = new GameOverMessage
                    {
                        RoomId = room.RoomId,
                        ResultType = "Surrender",
                        WinnerId = opponentId,
                        WinnerName = opponentName,
                        WinningLine = new string[0]
                    };

                    foreach (var p in room.Players) await _connectionManager.SendMessageToClientAsync(p.Id, gameOverMsg);
                    foreach (var p in room.Spectators) await _connectionManager.SendMessageToClientAsync(p.Id, gameOverMsg);

                    int? dbWinnerId = (match.PlayerX?.Id.ToString() == opponentId) ? match.PlayerX?.DatabaseId : match.PlayerO?.DatabaseId;
                    _matchService.SaveMatchResult(match.DbMatchId, dbWinnerId, "Surrender");
                }
            }
        }
        else if (msg.Action == "DrawRequest")
        {
            var room = _roomManager.FindPlayerRoom(session.SessionId.ToString());
            if (room != null)
            {
                var match = _matchManager.FindRoomMatch(room.RoomId);
                if (match != null && match.IsPlaying())
                {
                    string opponentId = (match.PlayerX?.Id.ToString() == session.SessionId.ToString())
                        ? match.PlayerO?.Id.ToString() ?? ""
                        : match.PlayerX?.Id.ToString() ?? "";

                    var req = new ResponseMessage { SenderId = "Server", Success = true, Action = "OpponentDrawRequest" };
                    if (!string.IsNullOrEmpty(opponentId))
                        await _connectionManager.SendMessageToClientAsync(opponentId, req);
                }
            }
        }
        else if (msg.Action == "DrawAccepted")
        {
            var room = _roomManager.FindPlayerRoom(session.SessionId.ToString());
            if (room != null)
            {
                var match = _matchManager.FindRoomMatch(room.RoomId);
                if (match != null && match.IsPlaying())
                {
                    _matchManager.EndMatch(match.MatchId, null, null, Shared.Enums.GameResultType.Draw, "Draw");

                    var gameOverMsg = new GameOverMessage
                    {
                        RoomId = room.RoomId,
                        ResultType = "Draw",
                        WinnerId = string.Empty,
                        WinnerName = string.Empty,
                        WinningLine = new string[0]
                    };

                    foreach (var p in room.Players) await _connectionManager.SendMessageToClientAsync(p.Id, gameOverMsg);
                    foreach (var p in room.Spectators) await _connectionManager.SendMessageToClientAsync(p.Id, gameOverMsg);

                    _matchService.SaveMatchResult(match.DbMatchId, null, "Draw");
                }
            }
        }
        else if (msg.Action == "DrawDeclined")
        {
            var room = _roomManager.FindPlayerRoom(session.SessionId.ToString());
            if (room != null)
            {
                var match = _matchManager.FindRoomMatch(room.RoomId);
                if (match != null && match.IsPlaying())
                {
                    string opponentId = (match.PlayerX?.Id.ToString() == session.SessionId.ToString())
                        ? match.PlayerO?.Id.ToString() ?? ""
                        : match.PlayerX?.Id.ToString() ?? "";

                    var req = new ResponseMessage { SenderId = "Server", Success = true, Action = "DrawDeclined" };
                    if (!string.IsNullOrEmpty(opponentId))
                        await _connectionManager.SendMessageToClientAsync(opponentId, req);
                }
            }
        }
        else if (msg.Action == "AnswerWaitOpponent")
        {
            var room = _roomManager.FindPlayerRoom(session.SessionId.ToString());
            if (room != null)
            {
                var match = _matchManager.FindRoomMatch(room.RoomId);
                if (match != null && match.IsSuspended())
                {
                    if (msg.Data == "Yes")
                    {
                        // Đối thủ đồng ý chờ, bắt đầu 120s extended grace
                        if (match.DisconnectedPlayerId != null)
                        {
                            _reconnectManager.StartGrace(match.DisconnectedPlayerId, GraceType.Extended, ReconnectManager.ExtendedGraceSeconds);

                            // Báo cho các client biết để UI hiển thị tiếp 120s chờ
                            var suspendMsg = new ResponseMessage
                            {
                                SenderId = "Server",
                                Success = true,
                                Action = "OpponentDisconnected",
                                Data = ReconnectManager.ExtendedGraceSeconds.ToString()
                            };

                            foreach (var p in room.Players)
                            {
                                if (p.Id != match.DisconnectedPlayerId)
                                    await _connectionManager.SendMessageToClientAsync(p.Id, suspendMsg);
                            }
                            foreach (var s in room.Spectators)
                            {
                                await _connectionManager.SendMessageToClientAsync(s.Id, suspendMsg);
                            }
                            Logger.Info($"[Reconnect] Doi thu dong y cho them {ReconnectManager.ExtendedGraceSeconds}s cho match {match.MatchId}");
                        }
                    }
                    else
                    {
                        // Đối thủ từ chối, kết thúc trận ngay
                        if (match.DisconnectedPlayerId != null)
                        {
                            _reconnectManager.CancelGrace(match.DisconnectedPlayerId);
                            HandleGraceExpiredAsync(match.DisconnectedPlayerId, GraceType.Extended);
                        }
                    }
                }
            }
        }
        else if (msg.Action == "JoinRoom")
        {
            var joinMsg = new JoinRoomMessage
            {
                SenderId = session.PlayerName,
                RoomId = msg.Data,
                PlayerId = session.PlayerName,
                PlayerName = session.PlayerName,
                IsSpectator = false
            };
            await HandleJoinRoomAsync(session, joinMsg);
        }
        else if (msg.Action == "GetProfile")
        {
            string username = !string.IsNullOrEmpty(msg.Data) ? msg.Data : session.PlayerName;
            var profile = _userService.GetUserProfile(username);
            var response = new ResponseMessage
            {
                SenderId = "Server",
                Success = profile != null,
                Action = "ProfileResponse",
                Data = profile != null ? System.Text.Json.JsonSerializer.Serialize(profile) : string.Empty
            };
            await session.SendAsync(response);
        }
        else if (msg.Action == "PlayWithBot")
        {
            await HandlePlayWithBotAsync(session, msg.Data ?? "Medium");
        }
    }

    private async Task ProcessPlayerLeaveAsync(ClientSession session, string? roomId = null)
    {
        var room = !string.IsNullOrEmpty(roomId) ? _roomManager.GetRoom(roomId) : _roomManager.FindPlayerRoom(session.SessionId.ToString());
        if (room == null)
        {
            room = _roomManager.FindPlayerRoom(session.SessionId.ToString());
        }

        if (room != null)
        {
            bool isActualPlayer = room.Players.Any(p => p.Id == session.SessionId.ToString());
            if (room.IsPlaying && isActualPlayer)
            {
                var match = _matchManager.FindRoomMatch(room.RoomId);
                if (match != null && match.State == MatchState.Playing)
                {
                    // Người chơi chủ động rời phòng khi đang đấu -> xử thua (Đầu hàng/Surrender)
                    Logger.Info($"[LeaveRoom] Player {session.SessionId} chu dong roi phong khi dang dau -> Xu thua (Surrender)");
                    var opponent = room.Players.FirstOrDefault(p => p.Id != session.SessionId.ToString());

                    _matchManager.StopTimer(match.MatchId);

                    var gameOverMsg = new GameOverMessage
                    {
                        RoomId = room.RoomId,
                        ResultType = "Surrender",
                        WinnerId = opponent?.Id ?? string.Empty,
                        WinnerName = opponent?.Username ?? string.Empty,
                        WinningLine = Array.Empty<string>()
                    };

                    if (opponent != null)
                        await _connectionManager.SendMessageToClientAsync(opponent.Id, gameOverMsg);

                    foreach (var spec in room.Spectators)
                        await _connectionManager.SendMessageToClientAsync(spec.Id, gameOverMsg);

                    int? dbWinnerId = opponent?.DatabaseId > 0 ? opponent.DatabaseId : (int?)null;
                    _matchService.SaveMatchResult(match.DbMatchId, dbWinnerId, "Surrender");

                    _matchManager.EndMatch(match.MatchId, null);
                    _roomManager.SetPlaying(room.RoomId, false);
                }
            }

            bool success = _roomManager.LeaveRoom(room.RoomId, session.SessionId.ToString());
            if (success)
            {
                Logger.Info($"[LeaveRoom] Session {session.SessionId} da roi phong {room.RoomId}");
                if (_roomManager.RoomExists(room.RoomId))
                {
                    await BroadcastRoomStateAsync(room);
                }
            }
        }

        var response = new ResponseMessage
        {
            SenderId = "Server",
            Success = true,
            Action = "LeaveRoom",
            ErrorMessage = string.Empty
        };
        await session.SendAsync(response);

        await BroadcastLobbyStateAsync();
    }

    private async Task HandlePlayMoveAsync(ClientSession session, MoveMessage msg)
    {
        Logger.Info($"[PlayMove] Session {session.SessionId} đánh cờ tại ({msg.Row}, {msg.Column}) trong phòng {msg.RoomId}");

        var match = _matchManager.FindRoomMatch(msg.RoomId);
        if (match == null || match.State != Shared.Models.MatchState.Playing)
        {
            var failResponse = new ResponseMessage
            {
                RequestMessageId = msg.MessageId,
                SenderId = "Server",
                Success = false,
                ErrorMessage = "Trận đấu không tồn tại hoặc đã kết thúc."
            };
            await session.SendAsync(failResponse);
            return;
        }

        var moveResult = _matchManager.TryMakeMove(match.MatchId, session.SessionId.ToString(), msg.Row, msg.Column);
        bool success = moveResult.IsValid;

        var response = new ResponseMessage
        {
            RequestMessageId = msg.MessageId,
            SenderId = "Server",
            Success = success,
            ErrorMessage = success ? string.Empty : moveResult.Message
        };

        await session.SendAsync(response);

        if (success)
        {
            // 1. Gửi broadcast nước đi cho tất cả người trong phòng
            var broadcastMove = new MoveMessage
            {
                RoomId = match.MatchId,
                Row = msg.Row,
                Column = msg.Column,
                Symbol = moveResult.Piece.ToString() // "X" hoặc "O"
            };

            var room = _roomManager.GetRoom(match.MatchId);
            if (room != null)
            {
                foreach (var p in room.Players)
                    await _connectionManager.SendMessageToClientAsync(p.Id, broadcastMove);
                foreach (var p in room.Spectators)
                    await _connectionManager.SendMessageToClientAsync(p.Id, broadcastMove);
            }

            // 2. Gửi kết quả ván đấu nếu thắng hoặc hòa
            if (moveResult.IsWin || moveResult.IsDraw)
            {
                string resultType = moveResult.IsWin ? "Win" : "Draw";
                string sessionX = match.PlayerX?.Id ?? string.Empty;
                string sessionO = match.PlayerO?.Id ?? string.Empty;

                string winnerId = moveResult.IsWin ? ((moveResult.Piece == Shared.Models.CellState.X) ? sessionX : sessionO) : string.Empty;
                string winnerName = moveResult.IsWin ? ((moveResult.Piece == Shared.Models.CellState.X) ? (match.PlayerX?.Username ?? string.Empty) : (match.PlayerO?.Username ?? string.Empty)) : string.Empty;

                var gameOverMsg = new GameOverMessage
                {
                    RoomId = match.MatchId,
                    ResultType = resultType,
                    WinnerId = winnerId,
                    WinnerName = winnerName,
                    WinningLine = moveResult.WinningLine != null ? moveResult.WinningLine.ToArray() : new string[0]
                };

                if (room != null)
                {
                    foreach (var p in room.Players)
                        await _connectionManager.SendMessageToClientAsync(p.Id, gameOverMsg);
                    foreach (var p in room.Spectators)
                        await _connectionManager.SendMessageToClientAsync(p.Id, gameOverMsg);
                }

                // Lưu kết quả vào cơ sở dữ liệu
                int? dbWinnerId = null;
                if (moveResult.IsWin)
                {
                    dbWinnerId = (moveResult.Piece == Shared.Models.CellState.X) ? match.PlayerX?.DatabaseId : match.PlayerO?.DatabaseId;
                }
                var matchService = new Server.Services.MatchService();
                matchService.SaveMatchResult(match.DbMatchId, dbWinnerId, resultType);
            }
            else
            {
                // Nếu ván chưa kết thúc và là phòng Bot -> Bot đánh tiếp
                if (room != null && room.IsBotRoom)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await BotPlayMoveAsync(room, match);
                        }
                        catch (Exception botEx)
                        {
                            Logger.Error($"[Bot] Lỗi khi Bot đánh: {botEx.Message}", botEx);
                        }
                    });
                }
            }
        }
    }

    private async Task HandleChatAsync(ClientSession session, ChatMessage chatMsg)
    {
        var room = _roomManager.GetRoom(chatMsg.RoomId);
        if (room != null)
        {
            var connections = _connectionManager.GetAll();
            foreach (var conn in connections)
            {
                // Kiểm tra xem user có trong phòng này không (Player hoặc Khán giả)
                if (room.Players.Any(p => p.Id == conn.SessionId.ToString()) ||
                    room.Spectators.Any(p => p.Id == conn.SessionId.ToString()))
                {
                    await conn.SendAsync(chatMsg);
                }
            }
        }
    }

    public async Task HandleClientDisconnectedAsync(ClientSession session)
    {
        Logger.Info($"[Disconnect] Client ngat ket noi: {session.SessionId}");

        var room = _roomManager.FindPlayerRoom(session.SessionId.ToString());
        if (room != null)
        {
            bool isActualPlayer = room.Players.Any(p => p.Id == session.SessionId.ToString());
            if (room.IsPlaying && isActualPlayer)
            {
                var match = _matchManager.FindRoomMatch(room.RoomId);
                if (match != null && match.State == MatchState.Playing)
                {
                    // Tạm dừng Match, bắt đầu đếm ngược grace period thay vì kết thúc ngay
                    _matchManager.StopTimer(match.MatchId); // Dừng timer lượt đi
                    match.Suspend(session.SessionId.ToString());

                    // Thông báo cho đối thủ biết đối phương bị mất kết nối
                    var opponent = room.Players.FirstOrDefault(p => p.Id != session.SessionId.ToString());
                    if (opponent != null)
                    {
                        var suspendMsg = new ResponseMessage
                        {
                            SenderId = "Server",
                            Success = true,
                            Action = "OpponentDisconnected",
                            Data = ReconnectManager.InitialGraceSeconds.ToString() // Thông báo có bao nhiêu giây chờ
                        };
                        await _connectionManager.SendMessageToClientAsync(opponent.Id, suspendMsg);
                    }

                    // Thông báo cho khán giả
                    foreach (var spec in room.Spectators)
                    {
                        var specNotify = new ResponseMessage
                        {
                            SenderId = "Server",
                            Success = true,
                            Action = "OpponentDisconnected",
                            Data = ReconnectManager.InitialGraceSeconds.ToString()
                        };
                        await _connectionManager.SendMessageToClientAsync(spec.Id, specNotify);
                    }

                    // Bắt đầu đếm ngược grace period lần đầu (60s)
                    _reconnectManager.StartGrace(session.SessionId.ToString(), GraceType.Initial, ReconnectManager.InitialGraceSeconds);
                    Logger.Info($"[Reconnect] Match {match.MatchId} suspended. Initial grace period: {ReconnectManager.InitialGraceSeconds}s");

                    // QUAN TRỌNG: Không xóa người chơi khỏi phòng, không gọi BroadcastLobbyStateAsync
                    return;
                }
            }

            // Chỉ xử lý rời phòng nếu không đang trong trận (ở lobby/waiting room)
            _roomManager.LeaveRoom(room.RoomId, session.SessionId.ToString());

            if (_roomManager.RoomExists(room.RoomId))
            {
                await BroadcastRoomStateAsync(room);
            }
        }

        await BroadcastLobbyStateAsync();
    }

    /// <summary>
    /// Xử lý khi grace period hết hạn.
    /// - Nếu là lần đầu (Initial): Gửi AskWaitOpponent cho đối thủ.
    /// - Nếu là gia hạn (Extended): Kết thúc trận đấu.
    /// </summary>
    private async void HandleGraceExpiredAsync(string disconnectedPlayerId, GraceType type)
    {
        try
        {
            Logger.Warn($"[Reconnect] Grace expired ({type}) cho {disconnectedPlayerId}.");

            var room = _roomManager.FindPlayerRoom(disconnectedPlayerId);
            if (room == null) return;

            var match = _matchManager.FindRoomMatch(room.RoomId);
            if (match == null || !match.IsSuspended()) return;

            // Xác định đối thủ còn lại
            var opponent = room.Players.FirstOrDefault(p => p.Id != disconnectedPlayerId);

            if (type == GraceType.Initial)
            {
                // Hết 60s đầu, hỏi ý kiến đối thủ
                if (opponent != null)
                {
                    var askMsg = new ResponseMessage
                    {
                        SenderId = "Server",
                        Success = true,
                        Action = "AskWaitOpponent",
                        Data = string.Empty
                    };
                    await _connectionManager.SendMessageToClientAsync(opponent.Id, askMsg);
                    Logger.Info($"[Reconnect] Sent AskWaitOpponent to {opponent.Username}");
                }
                else
                {
                    // Nếu không có đối thủ thì tự kết thúc
                    HandleGraceExpiredAsync(disconnectedPlayerId, GraceType.Extended);
                }
                return; // KHÔNG kết thúc trận đấu, chờ đối thủ trả lời
            }

            // Nếu type == GraceType.Extended (hoặc đối thủ từ chối chờ thêm) -> Kết thúc trận
            Logger.Info($"[Reconnect] {disconnectedPlayerId} that bai reconnect hoan toan. Ket thuc tran.");

            var gameOverMsg = new GameOverMessage
            {
                RoomId = room.RoomId,
                ResultType = "Disconnect",
                WinnerId = opponent?.Id ?? string.Empty,
                WinnerName = opponent?.Username ?? string.Empty,
                WinningLine = Array.Empty<string>()
            };

            if (opponent != null)
                await _connectionManager.SendMessageToClientAsync(opponent.Id, gameOverMsg);

            foreach (var spec in room.Spectators)
                await _connectionManager.SendMessageToClientAsync(spec.Id, gameOverMsg);

            // Lưu kết quả vào Database
            int? dbWinnerId = opponent?.DatabaseId > 0 ? opponent?.DatabaseId : (int?)null;
            _matchService.SaveMatchResult(match.DbMatchId, dbWinnerId, "Win");

            _matchManager.EndMatch(match.MatchId, null);
            _roomManager.LeaveRoom(room.RoomId, disconnectedPlayerId);

            if (_roomManager.RoomExists(room.RoomId))
                await BroadcastRoomStateAsync(room);

            await BroadcastLobbyStateAsync();
        }
        catch (Exception ex)
        {
            Logger.Error($"[Reconnect] Loi khi xu ly grace expired: {ex.Message}");
        }
    }

    /// <summary>
    /// Kiểm tra và thực hiện reconnect sau khi Player login lại.
    /// Tìm Match đang Suspended có cùng Username → hủy grace period → resume match.
    /// </summary>
    /// <returns>true nếu reconnect thành công, false nếu không có match nào để reconnect.</returns>
    private async Task<bool> TryReconnectAsync(ClientSession newSession)
    {
        // Tìm tất cả player IDs đang trong grace period
        var pendingPlayers = _reconnectManager.GetAllPendingPlayers();
        if (pendingPlayers.Count == 0) return false;

        // Tìm Match đang Suspended có Username trùng với người vừa login
        Match? match = null;
        string? oldPlayerId = null;

        foreach (var pid in pendingPlayers)
        {
            var m = _matchManager.GetAllMatches()
                .FirstOrDefault(x => x.IsSuspended() && x.DisconnectedPlayerId == pid &&
                    (x.PlayerX?.Username == newSession.PlayerName ||
                     x.PlayerO?.Username == newSession.PlayerName));

            if (m != null)
            {
                match = m;
                oldPlayerId = pid;
                break;
            }
        }

        if (match == null || string.IsNullOrEmpty(oldPlayerId)) return false;

        // Hủy grace period — Player đã reconnect kịp thời
        bool cancelled = _reconnectManager.CancelGrace(oldPlayerId);
        if (!cancelled)
        {
            Logger.Warn($"[Reconnect] Grace period da het han cho {oldPlayerId} truoc khi xu ly reconnect.");
            return false;
        }

        // Tìm phòng và cập nhật Player ID từ old sang new session
        var room = _roomManager.FindPlayerRoom(oldPlayerId);
        if (room == null) return false;

        var player = room.Players.FirstOrDefault(p => p.Id == oldPlayerId);
        if (player != null)
        {
            player.Id = newSession.SessionId.ToString(); // Cập nhật sang Session ID mới
        }

        // Cập nhật mapping trong ConnectionManager
        _connectionManager.UpdateSessionId(oldPlayerId, newSession);

        // Cập nhật Player ID trong Match
        if (match.PlayerX?.Id == oldPlayerId) match.PlayerX.Id = newSession.SessionId.ToString();
        if (match.PlayerO?.Id == oldPlayerId) match.PlayerO.Id = newSession.SessionId.ToString();

        // Resume match và khởi động lại timer lượt đi
        match.Resume();
        _matchManager.ResetTimer(match.MatchId);

        // Xây dựng chuỗi BoardState để gửi cho Client
        var sb = new System.Text.StringBuilder(match.Board.Rows * match.Board.Columns);
        for (int r = 0; r < match.Board.Rows; r++)
            for (int c = 0; c < match.Board.Columns; c++)
            {
                var cell = match.Board.GetCell(r, c);
                sb.Append(cell == CellState.X ? 'X' : (cell == CellState.O ? 'O' : '-'));
            }

        string mySymbol = (match.PlayerX?.Id == newSession.SessionId.ToString()) ? "X" : "O";

        // Gửi GameStateMessage để Client đồng bộ lại bàn cờ
        var syncMsg = new GameStateMessage
        {
            RoomId = room.RoomId,
            BoardState = sb.ToString(),
            BoardSize = room.BoardSize,
            CurrentPlayerId = match.GetCurrentPlayerId() ?? string.Empty,
            CurrentTurnName = match.GetCurrentPlayer()?.Username ?? string.Empty,
            PlayerXName = match.PlayerX?.Username ?? string.Empty,
            PlayerOName = match.PlayerO?.Username ?? string.Empty,
            Status = "Playing",
            MySymbol = mySymbol,
            SpectatorCount = room.Spectators.Count
        };
        await newSession.SendAsync(syncMsg);

        // Thông báo cho đối thủ biết Player đã reconnect
        var opponentPlayer = room.Players.FirstOrDefault(p => p.Id != newSession.SessionId.ToString());
        if (opponentPlayer != null)
        {
            var reconnectedMsg = new ResponseMessage
            {
                SenderId = "Server",
                Success = true,
                Action = "OpponentReconnected",
                Data = newSession.PlayerName
            };
            await _connectionManager.SendMessageToClientAsync(opponentPlayer.Id, reconnectedMsg);
        }

        // Thông báo khán giả
        foreach (var spec in room.Spectators)
        {
            var specMsg = new ResponseMessage
            {
                SenderId = "Server",
                Success = true,
                Action = "OpponentReconnected",
                Data = newSession.PlayerName
            };
            await _connectionManager.SendMessageToClientAsync(spec.Id, specMsg);
        }

        Logger.Info($"[Reconnect] {newSession.PlayerName} da reconnect thanh cong vao Match {match.MatchId}.");
        await BroadcastLobbyStateAsync();
        return true;
    }
    // BOT AI: TẠO PHÒNG ĐẤU VỚI MÁY

    /// <summary>
    /// Xử lý yêu cầu đấu với Bot AI.
    /// Tạo phòng, thêm Bot player ảo, bắt đầu trận ngay lập tức.
    /// </summary>
    private async Task HandlePlayWithBotAsync(ClientSession session, string difficultyStr)
    {
        Logger.Info($"[Bot] Yêu cầu đấu với Bot ({difficultyStr}) từ Session: {session.SessionId}");

        // Kiểm tra nếu session đang ở trong phòng khác thì rời trước
        var existingRoom = _roomManager.FindPlayerRoom(session.SessionId.ToString());
        if (existingRoom != null)
        {
            await ProcessPlayerLeaveAsync(session, existingRoom.RoomId);
        }

        var difficulty = CaroBot.ParseDifficulty(difficultyStr);
        string difficultyLabel = difficulty switch
        {
            CaroBot.Difficulty.Easy => "Dễ",
            CaroBot.Difficulty.Medium => "Trung bình",
            CaroBot.Difficulty.Hard => "Khó",
            _ => "Trung bình"
        };

        // Tạo phòng Bot
        var room = _roomManager.CreateRoom($"Bot AI ({difficultyLabel})");
        room.IsBotRoom = true;
        room.BotDifficulty = difficultyStr;
        room.BoardSize = 15;

        // Thêm người chơi vào phòng (Host, cầm X, đánh trước)
        string pName = !string.IsNullOrEmpty(session.PlayerName)
            ? session.PlayerName
            : "Player_" + session.SessionId.ToString().Substring(0, 4);
        var humanPlayer = new Player(session.SessionId.ToString(), pName);
        humanPlayer.DatabaseId = session.UserId;
        _roomManager.JoinRoom(room.RoomId, humanPlayer);

        // Tạo Bot player ảo (không có session thật)
        string botId = "BOT_" + Guid.NewGuid().ToString().Substring(0, 8);
        var botPlayer = new Player(botId, $"Bot AI ({difficultyLabel})");
        botPlayer.DatabaseId = 0; // Bot không có trong DB
        _roomManager.JoinRoom(room.RoomId, botPlayer);

        // Đặt trạng thái phòng
        room.HostSymbol = "X"; // Người chơi luôn cầm X
        _roomManager.SetPlaying(room.RoomId, true);

        // Tạo Match: Người chơi = X, Bot = O
        Player playerX = humanPlayer;
        Player playerO = botPlayer;

        var match = _matchManager.CreateMatch(room.RoomId, playerX, playerO, room.BoardSize);
        if (match == null)
        {
            Logger.Error($"[Bot] Không thể tạo match cho phòng Bot {room.RoomId}");
            return;
        }

        _matchManager.StartMatch(match.MatchId);
        Logger.Info($"[Bot] Đã tạo trận Bot: {match.MatchId} | {playerX.Username} (X) vs {playerO.Username} (O)");

        // Gửi GameState cho người chơi
        var gameState = new GameStateMessage
        {
            RoomId = room.RoomId,
            BoardState = string.Empty,
            BoardSize = room.BoardSize,
            CurrentPlayerId = playerX.Id,
            CurrentTurnName = playerX.Username,
            PlayerXName = playerX.Username,
            PlayerOName = playerO.Username,
            Status = "Playing",
            MySymbol = "X",
            SpectatorCount = 0
        };

        await session.SendAsync(gameState);
        await BroadcastLobbyStateAsync();
    }
    // BOT AI: BOT ĐÁNH NƯỚC ĐI

    /// <summary>
    /// Bot tính toán và đánh nước đi tiếp theo sau khi người chơi đã đánh xong.
    /// </summary>
    private async Task BotPlayMoveAsync(Room room, Match match)
    {
        if (match == null || !match.IsPlaying()) return;
        if (room == null || !room.IsBotRoom) return;

        // Xác định quân cờ của Bot
        string botId = "";
        CellState botPiece = CellState.Empty;

        if (match.PlayerO != null && match.PlayerO.Id.StartsWith("BOT_"))
        {
            botId = match.PlayerO.Id;
            botPiece = CellState.O;
        }
        else if (match.PlayerX != null && match.PlayerX.Id.StartsWith("BOT_"))
        {
            botId = match.PlayerX.Id;
            botPiece = CellState.X;
        }

        if (string.IsNullOrEmpty(botId) || botPiece == CellState.Empty) return;

        // Kiểm tra đúng lượt Bot
        if (match.CurrentTurn != botPiece) return;

        var difficulty = CaroBot.ParseDifficulty(room.BotDifficulty);

        // Delay mô phỏng suy nghĩ
        int delayMs = CaroBot.GetThinkDelayMs(difficulty);
        await Task.Delay(delayMs);

        // Kiểm tra lại trạng thái sau delay (có thể người chơi đã rời phòng)
        if (!match.IsPlaying()) return;

        // Tìm nước đi tốt nhất
        var bestMove = CaroBot.FindBestMove(match.Board, botPiece, difficulty);
        if (bestMove == null)
        {
            Logger.Warn($"[Bot] Không tìm được nước đi cho match {match.MatchId}");
            return;
        }

        int botRow = bestMove.Value.row;
        int botCol = bestMove.Value.col;

        Logger.Info($"[Bot] Bot đánh tại ({botRow}, {botCol}) trong match {match.MatchId}");

        // Thực hiện nước đi
        var moveResult = _matchManager.TryMakeMove(match.MatchId, botId, botRow, botCol);
        if (!moveResult.IsValid)
        {
            Logger.Warn($"[Bot] Nước đi không hợp lệ: {moveResult.Message}");
            return;
        }

        // Gửi broadcast nước đi của Bot cho người chơi
        var broadcastMove = new MoveMessage
        {
            RoomId = match.MatchId,
            Row = botRow,
            Column = botCol,
            Symbol = moveResult.Piece.ToString()
        };

        foreach (var p in room.Players)
        {
            if (!p.Id.StartsWith("BOT_"))
                await _connectionManager.SendMessageToClientAsync(p.Id, broadcastMove);
        }

        // Kiểm tra kết quả
        if (moveResult.IsWin || moveResult.IsDraw)
        {
            string resultType = moveResult.IsWin ? "Win" : "Draw";
            string sessionX = match.PlayerX?.Id ?? string.Empty;
            string sessionO = match.PlayerO?.Id ?? string.Empty;

            string winnerId = moveResult.IsWin
                ? ((moveResult.Piece == CellState.X) ? sessionX : sessionO)
                : string.Empty;
            string winnerName = moveResult.IsWin
                ? ((moveResult.Piece == CellState.X) ? (match.PlayerX?.Username ?? string.Empty) : (match.PlayerO?.Username ?? string.Empty))
                : string.Empty;

            var gameOverMsg = new GameOverMessage
            {
                RoomId = match.MatchId,
                ResultType = resultType,
                WinnerId = winnerId,
                WinnerName = winnerName,
                WinningLine = moveResult.WinningLine != null ? moveResult.WinningLine.ToArray() : new string[0]
            };

            foreach (var p in room.Players)
            {
                if (!p.Id.StartsWith("BOT_"))
                    await _connectionManager.SendMessageToClientAsync(p.Id, gameOverMsg);
            }
        }
    }
    // BOT AI: TÁI ĐẤU VỚI BOT

    /// <summary>
    /// Tự động chấp nhận tái đấu khi đấu với Bot.
    /// Reset match, đổi quân, bắt đầu lại.
    /// </summary>
    private async Task HandleBotRematchAsync(ClientSession session, Room room, Match match)
    {
        Logger.Info($"[Bot] Tái đấu với Bot trong phòng {room.RoomId}");

        // Reset trạng thái phòng
        _roomManager.SetPlaying(room.RoomId, false);
        room.HostSymbol = (room.HostSymbol == "X") ? "O" : "X"; // Đổi quân

        foreach (var p in room.Players)
            p.IsReady = false;

        // Xác định lại Player X và O sau khi đổi quân
        Player playerX, playerO;
        if (room.HostSymbol == "X")
        {
            playerX = room.Players[0]; // Host (người chơi)
            playerO = room.Players[1]; // Bot
        }
        else
        {
            playerX = room.Players[1]; // Bot
            playerO = room.Players[0]; // Host (người chơi)
        }

        // Xóa match cũ (phải RemoveMatch để giải phóng key trong dictionary), tạo match mới
        _matchManager.RemoveMatch(match.MatchId);
        _roomManager.SetPlaying(room.RoomId, true);

        var newMatch = _matchManager.CreateMatch(room.RoomId, playerX, playerO, room.BoardSize);
        if (newMatch == null)
        {
            Logger.Error($"[Bot] Không thể tạo match mới cho phòng Bot {room.RoomId}");
            return;
        }

        _matchManager.StartMatch(newMatch.MatchId);

        // Xác định symbol của người chơi
        string humanId = session.SessionId.ToString();
        string mySymbol = (playerX.Id == humanId) ? "X" : "O";

        // Gửi GameState cho người chơi
        var gameState = new GameStateMessage
        {
            RoomId = room.RoomId,
            BoardState = string.Empty,
            BoardSize = room.BoardSize,
            CurrentPlayerId = playerX.Id,
            CurrentTurnName = playerX.Username,
            PlayerXName = playerX.Username,
            PlayerOName = playerO.Username,
            Status = "Playing",
            MySymbol = mySymbol,
            SpectatorCount = 0
        };

        // Gửi RematchAccepted trước để GameForm đóng và quay về RoomForm
        var rematchResponse = new ResponseMessage
        {
            SenderId = "Server",
            Success = true,
            Action = "RematchAccepted",
            Data = room.RoomId
        };
        await session.SendAsync(rematchResponse);

        // Chờ một chút để GameForm xử lý RematchAccepted trước
        await Task.Delay(200);

        // Gửi GameState cho người chơi (RoomForm sẽ nhận và mở GameForm mới)
        await session.SendAsync(gameState);

        // Nếu Bot cầm X (đánh trước), gọi Bot đánh ngay
        if (playerX.Id.StartsWith("BOT_"))
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await BotPlayMoveAsync(room, newMatch);
                }
                catch (Exception ex)
                {
                    Logger.Error($"[Bot] Lỗi khi Bot đánh nước đầu: {ex.Message}", ex);
                }
            });
        }

        Logger.Info($"[Bot] Tái đấu thành công: {newMatch.MatchId} | {playerX.Username} (X) vs {playerO.Username} (O)");
    }
}
