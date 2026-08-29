using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Client.Network;

namespace Client.Services
{
    public class InternalPacket
    {
        public string Action { get; set; }
        public JsonElement Data { get; set; }
    }

    public class ClientMessageService
    {
        private readonly ClientConnection _clientConnection;
        private readonly SynchronizationContext _uiContext;

        public event Action<string> OnLoginResponse;
        public event Action<string> OnGameState;
        public event Action<string> OnTurn;
        public event Action<string> OnGameResult;
        public event Action<string> OnError;

        public ClientMessageService(ClientConnection clientConnection)
        {
            _clientConnection = clientConnection;
            _clientConnection.OnMessageReceived += ProcessMessage;
            _uiContext = SynchronizationContext.Current;
        }

        // --- CÁC HÀM GỬI DATA BẤT ĐỒNG BỘ ---

        public async Task SendLoginRequest(string username, string password) =>
            await SendPacketAsync("LoginRequest", new { Username = username, Password = password });

        public async Task SendCreateRoomRequest(string roomName) =>
            await SendPacketAsync("CreateRoomRequest", new { RoomName = roomName });

        public async Task SendJoinRoomRequest(string roomId) =>
            await SendPacketAsync("JoinRoomRequest", new { RoomId = roomId });

        public async Task SendLeaveRoomRequest(string roomId) =>
            await SendPacketAsync("LeaveRoomRequest", new { RoomId = roomId });

        public async Task SendMoveRequest(int row, int col) =>
            await SendPacketAsync("MoveRequest", new { Row = row, Col = col });

        private async Task SendPacketAsync(string action, object dataPayload)
        {
            try
            {
                var packet = new { Action = action, Data = dataPayload };
                string jsonPayload = JsonSerializer.Serialize(packet);
                await _clientConnection.SendMessage(jsonPayload);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Send Error]: {ex.Message}");
            }
        }

        // --- XỬ LÝ NHẬN DATA THỜI GIAN THỰC ĐẢM BẢO UI THREAD ---

        private void ProcessMessage(string jsonPayload)
        {
            try
            {
                var packet = JsonSerializer.Deserialize<InternalPacket>(jsonPayload);
                if (packet == null) return;

                string dataString = packet.Data.ToString();

                // Dispatch sự kiện sang UI Thread để Cập nhật UI an toàn
                switch (packet.Action)
                {
                    case "LoginResponse": SafeInvoke(OnLoginResponse, dataString); break;
                    case "GameState": SafeInvoke(OnGameState, dataString); break;
                    case "Turn": SafeInvoke(OnTurn, dataString); break;
                    case "GameResult": SafeInvoke(OnGameResult, dataString); break;
                    case "Error": SafeInvoke(OnError, dataString); break;
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[Parse Error]: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Process Error]: {ex.Message}");
            }
        }

        // Hàm hỗ trợ đẩy Event sang đúng UI Thread
        private void SafeInvoke(Action<string> action, string data)
        {
            if (_uiContext != null)
            {
                _uiContext.Post(_ => action?.Invoke(data), null);
            }
            else
            {
                action?.Invoke(data);
            }
        }
    }
}