using System;
using System.Threading.Tasks;
using CaroGame.Protocol;
using CaroGame.Protocol.Messages;
using CaroGame.Protocol.Messages.Game;
using CaroGame.Protocol.Messages.Response;
using Server.Managers;
using Server.Network;
using Server.Utils;

namespace Server.Services
{
    public class GameRequestHandler
    {
        private readonly MatchManager _matchManager;
        private readonly ConnectionManager _connectionManager;
        private readonly RoomManager _roomManager;

        public GameRequestHandler(MatchManager matchManager, ConnectionManager connectionManager, RoomManager roomManager)
        {
            _matchManager = matchManager ?? throw new ArgumentNullException(nameof(matchManager));
            _connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));
            _roomManager = roomManager ?? throw new ArgumentNullException(nameof(roomManager));
        }

        public async Task HandlePlayMoveAsync(ClientSession session, MoveMessage msg)
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
                // 1. Broadcast MoveMessage
                var broadcastMove = new MoveMessage
                {
                    RoomId = match.MatchId,
                    Row = msg.Row,
                    Column = msg.Column,
                    Symbol = moveResult.Piece.ToString() // "X" or "O"
                };

                var room = _roomManager.GetRoom(match.MatchId);
                if (room != null)
                {
                    foreach (var p in room.Players)
                        await _connectionManager.SendMessageToClientAsync(p.Id, broadcastMove);
                    foreach (var p in room.Spectators)
                        await _connectionManager.SendMessageToClientAsync(p.Id, broadcastMove);
                }

                // 2. Broadcast GameOverMessage if applicable
                if (moveResult.IsWin || moveResult.IsDraw)
                {
                    string resultType = moveResult.IsWin ? "Win" : "Draw";
                    string sessionX = match.PlayerX?.Id ?? string.Empty;
                    string sessionO = match.PlayerO?.Id ?? string.Empty;

                    string winnerId = moveResult.IsWin ? ((moveResult.Piece == Shared.Models.CellState.X) ? sessionX : sessionO) : string.Empty;
                    string winnerName = moveResult.IsWin ? ((moveResult.Piece == Shared.Models.CellState.X) ? match.PlayerX?.Username : match.PlayerO?.Username) : string.Empty;

                    var gameOverMsg = new GameOverMessage
                    {
                        RoomId = match.MatchId,
                        ResultType = resultType,
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

                    // LƯU KẾT QUẢ VÀO DATABASE
                    int? dbWinnerId = null;
                    if (moveResult.IsWin)
                    {
                        dbWinnerId = (moveResult.Piece == Shared.Models.CellState.X) ? match.PlayerX?.DatabaseId : match.PlayerO?.DatabaseId;
                    }
                    var matchService = new Server.Services.MatchService();
                    matchService.SaveMatchResult(match.DbMatchId, dbWinnerId, resultType);
                }
            }
        }
    }
}
