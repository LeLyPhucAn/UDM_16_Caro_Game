# UDM_16 - Game Caro trực tuyến

## Mã course: 012012301303

## Mã nhóm: Net3_Group_08

## Thành viên

| STT | MSSV         | Họ và tên               | Vai trò |
|-----|--------------|-------------------------|---------|
| 1   | 080206002048 | Lê Lý Phúc An           | Architecture & Core Backend |
| 2   | 068305006610 | Nguyễn Trọng Vân Khuyên | Network Message & Protocol |
| 3   | 054206002957 | Lê Quốc Kim             | Frontend / Client UI |
| 4   | 052306013525 | Võ Thị Kim Kiều         | Database & Repositories |
| 5   | 054206001474 | Lê Thế Kiệt             | Game Logic Engine |

---

## Phân công công việc

### 1. Phân công tổng thể vai trò & module

| Thành viên | Vai trò | Module Code chính | Công việc DOCX |
|------------|---------|-------------------|----------------|
| **Lê Lý Phúc An (Leader)** | Kiến trúc & Tích hợp hệ thống | Thiết kế kiến trúc, Socket TCP Server, Connection Manager, Session Manager, Shared Library, tích hợp các module, review code, merge nhánh | Chương 1 (Tổng quan), Chương 3 (Kiến trúc hệ thống), rà soát toàn bộ báo cáo |
| **Lê Thế Kiệt** | Backend - Game Logic | Board, Move, Room Manager, Match Manager, Validate Move, kiểm tra thắng/thua/hòa, Timer | Chương 2 (Yêu cầu chức năng), Activity Diagram, Sequence Diagram (Luồng chơi game) |
| **Lê Quốc Kim** | Frontend - Client | Login UI, Lobby UI, Game UI, Spectator UI, hiển thị Timer, cập nhật trạng thái bàn cờ | Thiết kế GUI, Use Case Diagram |
| **Võ Thị Kim Kiều** | Database & History | Database Layer, Repository, Match History, Config (IP/Port), màn hình lịch sử trận đấu | Thiết kế CSDL, ERD, Data Dictionary |
| **Nguyễn Trọng Vân Khuyên** | Lobby & Network Message | Online Player List, Invite/Accept/Reject, Join/Leave Room, Message Packet, JSON Serializer, Connection Status | Protocol Message, Test Case |

### 2. Phân công chi tiết nhiệm vụ tuần 1

| Thành viên | Công việc CODE | Công việc DOCX |
|------------|----------------|----------------|
| **Lê Lý Phúc An** | - Tạo Solution và cấu trúc Project (Server, Client, Shared)<br>- Xây dựng TCP Server cơ bản (Start/Stop Server)<br>- Cho phép Client kết nối đến Server<br>- Thiết lập cấu hình IP/Port | - Chương 1: Lý do chọn đề tài<br>- Mục tiêu<br>- Phạm vi<br>- Lý thuyết tóm tắt (Socket, TCP, Client–Server) |
| **Lê Thế Kiệt** | - Tạo các Model: Player, Board, Room, Match, Move<br>- Khai báo thuộc tính, constructor và method (xử lý logic bàn cờ) | - Liệt kê Functional Requirements<br>- Mô tả các chức năng chính của hệ thống |
| **Lê Quốc Kim** | - Tạo Login Form<br>- Tạo Lobby Form<br>- Tạo Game Form (khung giao diện)<br>- Chuyển đổi giữa các Form | - Phác thảo giao diện (Mockup)<br>- Vẽ Use Case Diagram |
| **Võ Thị Kim Kiều** | - Tạo Database Layer (`DatabaseHelper`, `DatabaseConfig`)<br>- Tạo Repository: UserRepository, MatchRepository, HistoryRepository<br>- Thiết kế lớp Config đọc IP/Port (khung) | - Thiết kế Database<br>- ERD sơ bộ |
| **Nguyễn Trọng Vân Khuyên** | - Tạo Message Packet<br>- Tạo MessageType (Enum)<br>- Tạo JsonSerializer<br>- Khởi tạo các lớp LoginMessage, InviteMessage, ResponseMessage | - Thiết kế Protocol Message V1<br>- Liệt kê các loại Message sẽ sử dụng |


### 3. Lộ trình phát triển (Timeline 6 tuần)

| Tuần | Mục tiêu chính |
|------|----------------|
| **Tuần 1** | Dựng nền dự án, hoàn thành khung Server, Client, Model và tài liệu cơ bản |
| **Tuần 2** | Hoàn thiện Lobby, Login, Online List, Protocol và thiết kế hệ thống |
| **Tuần 3** | Xây dựng Game Logic và đồng bộ trận đấu |
| **Tuần 4** | Hoàn thiện Spectator, Timer, Match History |
| **Tuần 5** | Reconnect, xử lý lỗi, kiểm thử, tối ưu |
| **Tuần 6** | Hoàn thiện báo cáo, quay video, chuẩn bị demo |

---

## Tiến độ & Các hạng mục đã hoàn thành 

- [x] **Cấu trúc Solution**: Đã chia lớp hoàn chỉnh gồm 3 dự án `Server` (Console app / Network Host), `Client` / `Player` (WinForms App), và `Shared` (Thư viện dùng chung chứa DTO, Messages, Enums).
- [x] **Database Layer & Persistence**: Tích hợp SQL Server với `DatabaseHelper` (sử dụng `Microsoft.Data.SqlClient`), hỗ trợ kết nối và đọc ghi dữ liệu người dùng, trận đấu.
- [x] **Giao diện Client (WinForms)**: Hoàn thiện `LoginForm`, `LobbyForm`, và `GameForm` tích hợp vẽ bàn cờ Caro mượt mà.
- [x] **Mạng & Protocol**: Xây dựng TCP Socket mã hóa gói tin JSON (`NetworkMessage`, `MessageType`), hỗ trợ gửi nhận gói tin hai chiều giữa Server và nhiều Client cùng lúc.
- [x] **Mô hình hóa dữ liệu (Domain Models)**: Hoàn thành các đối tượng `Player`, `Room`, `Match`, `Board`, `Move`.

---

## Giới thiệu

Đề tài **UDM_16 - Game Caro trực tuyến** xây dựng hệ thống trò chơi Caro (Gomoku) trực tuyến chạy trên nền tảng .NET 10.0 / Windows Forms. Hệ thống hỗ trợ đa người chơi kết nối qua mạng IP/TCP, hỗ trợ quản lý phòng chơi, danh sách người chơi online, tính giờ nước đi, kiểm tra thắng thua tự động và lưu trữ lịch sử trận đấu vào SQL Server.

## Kiến trúc hệ thống

- **Mô hình**: Client – Server (TCP Socket)
- **Protocol**: Custom JSON Message Protocol qua TCP Sockets
- **Port mặc định**: `8888` (hoặc cấu hình tùy chỉnh trong `ServerConfig.json`)
- **Cấu trúc message**: Dạng gói tin JSON chuẩn gồm Header (`MessageType`, `SenderId`, `Timestamp`) và `Payload` data.

## Yêu cầu môi trường

- **Hệ điều hành**: Windows 10 / Windows 11
- **Runtime**: .NET 10.0 SDK (`net10.0` và `net10.0-windows`)
- **Công cụ**: Visual Studio 2022 / VS Code (có C# Dev Kit extension)
- **Database**: Microsoft SQL Server (Express hoặc LocalDB)

## Cài đặt

1. Clone repository về máy:
   ```bash
   git clone https://github.com/LeLyPhucAn/UDM_16_Caro_Game.git
   cd Project_UDM_16_Caro_Game
   ```
2. Mở Solution bằng Visual Studio hoặc VS Code.
3. Khôi phục các gói NuGet dependency:
   ```bash
   dotnet restore Code/UDM_16_Caro_Game.slnx
   ```

## Hướng dẫn chạy

### 1. Khởi chạy Server

- **Cách 1 (Bằng CLI / Terminal)**:
  ```bash
  cd Code/Server
  dotnet run
  ```
- **Cách 2 (Visual Studio)**:
  - Chọn project `Server` làm Startup Project và nhấn `F5` hoặc `Ctrl + F5`.

### 2. Khởi chạy Client / Player

- **Cách 1 (Bằng CLI / Terminal)**:
  ```bash
  cd Code/Client/Player
  dotnet run
  ```
- **Cách 2 (Visual Studio)**:
  - Chọn project `Player` làm Startup Project và nhấn `F5`.
  - Có thể khởi chạy nhiều phiên bản Player (Client) cùng lúc để thử nghiệm chơi 2 người.

## Cấu hình

File cấu hình Server nằm tại `Code/Server/Config/ServerConfig.json`:
```json
{
  "ServerIp": "127.0.0.1",
  "Port": 8888,
  "MaxConnections": 100,
  "ConnectionString": "Server=localhost;Database=CaroDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

## Chức năng

- [x] Đăng nhập / Đăng ký tài khoản người chơi
- [x] Quản lý sảnh (Lobby) và danh sách người chơi online
- [x] Tạo phòng chơi và tham gia phòng
- [x] Gửi lời mời chơi và chấp nhận/từ chối
- [x] Chơi game Caro (vẽ bàn cờ, đánh cờ, kiểm tra luật thắng 5 nước)
- [ ] Tính giờ đếm ngược nước đi (Timer)
- [ ] Chế độ xem người khác chơi (Spectator mode)
- [ ] Lưu lịch sử trận đấu vào SQL Server
- [ ] Xử lý mất kết nối & Reconnect

## Kiểm thử



## Demo

- **Video**: [Đang cập nhật]
- **Slide**: `PPTX/`
- **Báo cáo**: `DOCX/`

## Giới hạn

- Hiện tại hệ thống đang phát triển ở giao diện WinForms cơ bản.
- Chưa hỗ trợ Chat âm thanh (Voice Chat).
