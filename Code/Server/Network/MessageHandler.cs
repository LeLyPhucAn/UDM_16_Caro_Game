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
        _gameRequestHandler = new GameRequestHandler(_matchManager, _connectionManager, _roomManager);
        _matchService = new MatchService();
        _matchManager.OnMatchTimeout += HandleMatchTimeout;
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

                case MessageType.Chat:
                    if (message is ChatMessage chatMsg)
                        await HandleChatAsync(session, chatMsg);
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
        bool isValid = _userService.Login(loginMsg.Username, loginMsg.Password, out System.Data.DataRow? userRow);

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
            // Update Session PlayerName & UserId
            session.PlayerName = loginMsg.Username;
            if (userRow != null && userRow.Table.Columns.Contains("Id") && userRow["Id"] != DBNull.Value)
            {
                session.UserId = Convert.ToInt32(userRow["Id"]);
            }
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
        bool success = _roomManager.JoinRoom(msg.RoomId, player, msg.IsSpectator);

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
            {
                await BroadcastRoomStateAsync(room);

                // Neu phong dang choi va nguoi nay la khan gia, gui GameStateMessage ngay lap tuc de vao xem
                if (room.IsPlaying && msg.IsSpectator)
                {
                    var match = _matchManager.FindRoomMatch(room.RoomId);
                    if (match != null)
                    {
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
                if (row["WinnerId"] != DBNull.Value)
                {
                    outcome = (matchWinnerId == userId) ? "Thắng" : "Thua";
                }
                else
                {
                    string rawRes = row["Result"]?.ToString() ?? "";
                    if (rawRes.Contains("Draw", StringComparison.OrdinalIgnoreCase))
                        outcome = "Hòa";
                    else if (rawRes.Contains("Cancel", StringComparison.OrdinalIgnoreCase))
                        outcome = "Hủy";
                    else
                        outcome = rawRes;
                }

                response.Matches.Add(new MatchHistoryItem
                {
                    MatchId = Convert.ToInt32(row["MatchId"]),
                    Player1Id = row["Player1Id"] != DBNull.Value ? Convert.ToInt32(row["Player1Id"]) : 0,
                    Player2Id = row["Player2Id"] != DBNull.Value ? Convert.ToInt32(row["Player2Id"]) : 0,
                    StartTime = Convert.ToDateTime(row["StartTime"]),
                    EndTime = row["EndTime"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["EndTime"]) : null,
                    WinnerId = row["WinnerId"] != DBNull.Value ? (int?)Convert.ToInt32(row["WinnerId"]) : null,
                    Result = outcome,
                    Status = row["Status"]?.ToString() ?? ""
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
        var roomInfos = new System.Collections.Generic.List<CaroGame.Protocol.RoomInfo>();
        foreach (var room in _roomManager.GetRooms())
        {
            roomInfos.Add(new CaroGame.Protocol.RoomInfo {
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
                    var opponent = room.Players.FirstOrDefault(p => p.Id != session.SessionId.ToString());
                    if (opponent != null)
                    {
                        var gameOverMsg = new GameOverMessage
                        {
                            RoomId = room.RoomId,
                            ResultType = "Surrender",
                            WinnerId = opponent.Id,
                            WinnerName = opponent.Username,
                            WinningLine = new string[0]
                        };

                        await _connectionManager.SendMessageToClientAsync(opponent.Id, gameOverMsg);
                        foreach (var spec in room.Spectators)
                        {
                            await _connectionManager.SendMessageToClientAsync(spec.Id, gameOverMsg);
                        }

                        int? dbWinnerId = opponent.DatabaseId;
                        _matchService.SaveMatchResult(match.DbMatchId, dbWinnerId, "Win");
                    }
                    _matchManager.EndMatch(match.MatchId, null);
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
            ErrorMessage = string.Empty
        };
        await session.SendAsync(response);

        await BroadcastLobbyStateAsync();
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
                    var opponent = room.Players.FirstOrDefault(p => p.Id != session.SessionId.ToString());
                    if (opponent != null)
                    {
                        var gameOverMsg = new GameOverMessage
                        {
                            RoomId = room.RoomId,
                            ResultType = "Disconnect",
                            WinnerId = opponent.Id,
                            WinnerName = opponent.Username,
                            WinningLine = new string[0]
                        };

                        await _connectionManager.SendMessageToClientAsync(opponent.Id, gameOverMsg);
                        foreach (var spec in room.Spectators)
                        {
                            await _connectionManager.SendMessageToClientAsync(spec.Id, gameOverMsg);
                        }

                        int? dbWinnerId = opponent.DatabaseId;
                        _matchService.SaveMatchResult(match.DbMatchId, dbWinnerId, "Win");
                    }
                    _matchManager.EndMatch(match.MatchId, null);
                }
            }

            _roomManager.LeaveRoom(room.RoomId, session.SessionId.ToString());

            if (_roomManager.RoomExists(room.RoomId))
            {
                await BroadcastRoomStateAsync(room);
            }
        }

        await BroadcastLobbyStateAsync();
    }
}