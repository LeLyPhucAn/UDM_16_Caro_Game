using System;
using System.Text.Json;
using System.Threading.Tasks;
using Client.Network;

namespace Client.Services
{
    // Cấu trúc gói tin nội bộ để Deserialize
    public class InternalPacket
    {
        public string Action { get; set; }
        public JsonElement Data { get; set; }
    }

    public class ClientMessageService
    {
        private readonly ClientConnection _clientConnection;

        // Các event để đẩy dữ liệu lên UI
        public event Action<string> OnLoginResponse;
        public event Action<string> OnGameState;
        public event Action<string> OnTurn;
        public event Action<string> OnGameResult;
        public event Action<string> OnError;

        public ClientMessageService(ClientConnection clientConnection)
        {
            _clientConnection = clientConnection;
            _clientConnection.OnMessageReceived += ProcessMessage;
        }

        // ================= CÁC HÀM GỬI REQUEST (GỬI LÊN SERVER) =================

        public async Task SendLoginRequest(string username, string password)
        {
            await SendPacketAsync("LoginRequest", new { Username = username, Password = password });
        }

        public async Task SendCreateRoomRequest(string roomName)
        {
            await SendPacketAsync("CreateRoomRequest", new { RoomName = roomName });
        }

        public async Task SendJoinRoomRequest(string roomId)
        {
            await SendPacketAsync("JoinRoomRequest", new { RoomId = roomId });
        }

        public async Task SendLeaveRoomRequest(string roomId)
        {
            await SendPacketAsync("LeaveRoomRequest", new { RoomId = roomId });
        }

        public async Task SendMoveRequest(int row, int col)
        {
            await SendPacketAsync("MoveRequest", new { Row = row, Col = col });
        }

        private async Task SendPacketAsync(string action, object dataPayload)
        {
            try
            {
                // Serialize thành chuỗi JSON
                var packet = new { Action = action, Data = dataPayload };
                string jsonPayload = JsonSerializer.Serialize(packet);
                await _clientConnection.SendMessage(jsonPayload);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi Send]: {ex.Message}");
            }
        }

        // ================= XỬ LÝ RESPONSE/BROADCAST (NHẬN TỪ SERVER) =================

        public void ProcessMessage(string jsonPayload)
        {
            try
            {
                // Deserialize dữ liệu JSON
                var packet = JsonSerializer.Deserialize<InternalPacket>(jsonPayload);
                if (packet == null) return;

                // Lấy phần thân dữ liệu (Data) dưới dạng chuỗi
                string dataString = packet.Data.ToString();

                // Phân luồng sự kiện truyền về UI
                switch (packet.Action)
                {
                    case "LoginResponse":
                        OnLoginResponse?.Invoke(dataString);
                        break;
                    case "GameState":
                        OnGameState?.Invoke(dataString);
                        break;
                    case "Turn":
                        OnTurn?.Invoke(dataString);
                        break;
                    case "GameResult":
                        OnGameResult?.Invoke(dataString);
                        break;
                    case "Error":
                        OnError?.Invoke(dataString);
                        break;
                    default:
                        Console.WriteLine($"[ClientMessageService] Hành động chưa xác định: {packet.Action}");
                        break;
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[Lỗi Deserialize] Dữ liệu rác hoặc sai định dạng: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi không xác định]: {ex.Message}");
            }
        }
    }
}