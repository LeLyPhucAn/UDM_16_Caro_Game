# UDM_16 - Game Caro trực tuyến

## Mã course: 012012301303

## Mã nhóm: Net3_Group_08

## Video Demo: https://youtu.be/7Rs3WFidfCY

## Thành viên

| STT | MSSV         | Họ và tên               | Vai trò |
|-----|--------------|-------------------------|---------|
| 1   | 080206002048 | Lê Lý Phúc An           | Architecture & Core Backend |
| 2   | 068305006610 | Nguyễn Trọng Vân Khuyên | Network Message & Protocol |
| 3   | 054206002957 | Lê Quốc Kim             | Frontend / Client UI |
| 4   | 052306013525 | Võ Thị Kim Kiều         | Database & Repositories |
| 5   | 054206001474 | Lê Thế Kiệt             | Game Logic Engine |
| 6   | 052206005364 | Nguyễn Văn Hiếu         | Client Network & Infrastructure |

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
| **Nguyễn Văn Hiếu** | Client Network & Infrastructure | TcpClientService, ClientConnection, Client Config, Logger, giao tiếp TCP phía Client | Component/Deployment Diagram, mô tả giao tiếp Client–Server |

### 2. Phân công chi tiết nhiệm vụ tuần
### 2.1 Phân công chi tiết nhiệm vụ tuần 1

| Thành viên | Công việc CODE | Công việc DOCX |
|------------|----------------|----------------|
| **Lê Lý Phúc An** | - Tạo Solution và cấu trúc Project (Server, Client, Shared)<br>- Xây dựng TCP Server cơ bản (Start/Stop Server)<br>- Cho phép Client kết nối đến Server<br>- Thiết lập cấu hình IP/Port | - Chương 1: Lý do chọn đề tài<br>- Mục tiêu<br>- Phạm vi<br>- Lý thuyết tóm tắt (Socket, TCP, Client–Server) |
| **Lê Thế Kiệt** | - Tạo các Model: Player, Board, Room, Match, Move<br>- Khai báo thuộc tính, constructor và method (xử lý logic bàn cờ) | - Liệt kê Functional Requirements<br>- Mô tả các chức năng chính của hệ thống |
| **Lê Quốc Kim** | - Tạo Login Form<br>- Tạo Lobby Form<br>- Tạo Game Form (khung giao diện)<br>- Chuyển đổi giữa các Form | - Phác thảo giao diện (Mockup)<br>- Vẽ Use Case Diagram |
| **Võ Thị Kim Kiều** | - Tạo Database Layer (`DatabaseHelper`, `DatabaseConfig`)<br>- Tạo Repository: UserRepository, MatchRepository, HistoryRepository<br>- Thiết kế lớp Config đọc IP/Port (khung) | - Thiết kế Database<br>- ERD sơ bộ |
| **Nguyễn Trọng Vân Khuyên** | - Tạo Message Packet<br>- Tạo MessageType (Enum)<br>- Tạo JsonSerializer<br>- Khởi tạo các lớp LoginMessage, InviteMessage, ResponseMessage | - Thiết kế Protocol Message V1<br>- Liệt kê các loại Message sẽ sử dụng |

### 2.2 Phân công chi tiết nhiệm vụ tuần 2
| Thành viên | Công việc CODE | Công việc DOCX |
|---|---|---|
| **Lê Lý Phúc An** | Network Layer Server, Connection Manager, Logger Server | Chương 2, yêu cầu phi chức năng, kiến trúc hệ thống, Sequence Diagram kết nối |
| **Lê Thế Kiệt** | Board Logic, RoomManager, MatchManager | Use Case Specification, Activity Diagram |
| **Lê Quốc Kim** | Login UI, Lobby UI, Game UI, tích hợp Network | Wireframe, Mockup, mô tả Login/Lobby/Game |
| **Võ Thị Kim Kiều** | Database Layer, Repository, Service | Database Design, ERD, mô tả bảng |
| **Nguyễn Trọng Vân Khuyên** | Message Protocol, Packet Parser, JSON Serializer | Protocol Message Version 1, Message Format, Packet Structure, Sequence Diagram Login |
| **Nguyễn Văn Hiếu** | Client Network Layer, Logger & Config Manager | Component Diagram/Deployment Diagram, mô tả giao tiếp Client–Server |

### 2.3 Phân công chi tiết nhiệm vụ tuần 3
| Thành viên | Module | Công việc CODE (Task 1 & Task 2) | Công việc DOCX |
|---|---|---|---|
| **Lê Lý Phúc An** | Server Integration | - Xử lý Request/Response cơ bản<br>- Tích hợp Room & Match trên Server | Kiến trúc Server & Sequence Diagram |
| **Lê Thế Kiệt** | Game Logic | - Hoàn thiện Match Logic cơ bản<br>- Xây dựng luật Game & Validate nước đi | Activity Diagram & Game Flow |
| **Lê Quốc Kim** | Client UI | - Tích hợp Login/Lobby với Server<br>- Tích hợp giao diện Game cơ bản | GUI & User Flow |
| **Võ Thị Kim Kiều** | Database | - Xử lý Database User (Persistence)<br>- Thiết lập lưu Match/History | ERD, Data Dictionary & Database Design |
| **Nguyễn Trọng Vân Khuyên** | Protocol | - Thiết kế Game/Lobby Messages<br>- Routing Packet & Validation sơ bộ | Protocol Design & Message Specification |
| **Nguyễn Văn Hiếu** | Client Network | - Tích hợp Network Message Client<br>- Xử lý Connection & Event cơ bản | Mô tả giao tiếp Client–Server |

### 2.4 Phân công chi tiết nhiệm vụ tuần 4
| Thành viên | Module | Công việc CODE (Task 1 & Task 2) | Công việc DOCX |
|---|---|---|---|
| **Lê Lý Phúc An** | Server Integration | - Hoàn thiện Server Request Handler & Game Flow<br>- Tích hợp toàn bộ Server và đồng bộ với Client | Kiến trúc tích hợp hệ thống |
| **Lê Thế Kiệt** | Game Logic | - Hoàn thiện Game Rule & Match State<br>- Hoàn thiện Logic Timer, Game Result & End Game | Luồng xử lý Game |
| **Lê Quốc Kim** | Client UI | - Hoàn thiện Lobby/Room UI<br>- Hoàn thiện Game UI và luồng chơi hoàn chỉnh | Giao diện hoàn chỉnh |
| **Võ Thị Kim Kiều** | Database | - Hoàn thiện Match Persistence<br>- Tích hợp toàn diện History vào Database | Bổ sung Database & History |
| **Nguyễn Trọng Vân Khuyên** | Protocol | - Hoàn thiện Game Protocol<br>- Đồng bộ trạng thái Game & Error Protocol | Protocol hoàn chỉnh |
| **Nguyễn Văn Hiếu** | Client Network | - Hoàn thiện Client Network Integration<br>- Xử lý Connection/Event chuyên sâu | Cập nhật giao tiếp Client–Server |

### 2.5 Phân công chi tiết nhiệm vụ tuần 5 (Tích hợp & Kiểm thử)
| Thành viên | Module | Công việc CODE (Final Code & Bug Fix) | Công việc DOCX |
|---|---|---|---|
| **Lê Lý Phúc An** | Architecture & Core Backend | - Hoàn thiện Server Integration, Network, Connection, Request/Response<br>- Test Server, Multi-Client, Disconnect & Exception Handling | Cập nhật kiến trúc và triển khai Server |
| **Lê Thế Kiệt** | Game Logic Engine | - Hoàn thiện Board, Match, Move, Win/Lose/Draw, Timer<br>- Test toàn bộ Game Logic và sửa lỗi luật chơi | Cập nhật Game Logic, Activity & Sequence |
| **Lê Quốc Kim** | Frontend / Client UI | - Hoàn thiện UI Login, Lobby, Room, Game, Timer, Result<br>- Test toàn bộ UI, User Flow và sửa lỗi UI/UX | Cập nhật GUI và User Flow |
| **Võ Thị Kim Kiều** | Database & Repositories | - Hoàn thiện User, Match, History, Repository & Persistence<br>- Test CRUD, lưu Match, truy xuất History sau trận | Cập nhật Database Design và Data Dictionary |
| **Nguyễn Trọng Vân Khuyên** | Network Message & Protocol | - Hoàn thiện toàn bộ Message, Packet, Routing, Validation<br>- Test toàn bộ Protocol, Packet lỗi, đồng bộ Message | Cập nhật Protocol Specification |
| **Nguyễn Văn Hiếu** | Client Network | - Hoàn thiện Client Network, Connection, Reconnect, Events<br>- Test Connect/Disconnect/Reconnect và xử lý lỗi Network | Cập nhật Client-Server Communication |

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

- **Mô hình**: Client–Server (TCP Socket đa luồng bất đồng bộ)
- **Protocol**: Custom Binary Framing kết hợp JSON UTF-8 payload
- **Port mặc định**: `5000` (cấu hình trong `Code/Server/Config/ServerConfig.json`)
- **Cấu trúc message**: Binary Length-Prefix Framing: `[4 bytes MessageType] [4 bytes BodyLength] [Body JSON UTF-8]`

## Yêu cầu môi trường

- **Hệ điều hành**: Windows 10 / Windows 11 (64-bit)
- **Ngôn ngữ và phiên bản**: C# 13 / .NET 10.0 SDK (`net10.0` và `net10.0-windows`), Python 3.10+ (cho script kiểm thử)
- **Công cụ hoặc dependency**:
  - Visual Studio 2022 (v17.12+) hoặc VS Code (có C# Dev Kit extension)
  - Hệ quản trị CSDL: Microsoft SQL Server (Express hoặc LocalDB `(localdb)\mssqllocaldb`)
  - Thư viện: `Microsoft.Data.SqlClient`, `System.Text.Json`

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

### Server

- **Cách 1 (Bằng CLI / Terminal)**:
  ```bash
  dotnet run --project Code/Server
  ```
- **Cách 2 (Visual Studio)**:
  - Chọn project `Server` làm Startup Project và nhấn `F5` hoặc `Ctrl + F5`.

### Client

- **Cách 1 (Bằng CLI / Terminal)**:
  ```bash
  dotnet run --project Code/Client
  ```
- **Cách 2 (Visual Studio)**:
  - Chọn project `Client` làm Startup Project và nhấn `F5`.
  - Có thể khởi chạy nhiều phiên bản Client cùng lúc trên một hoặc nhiều máy để thử nghiệm thi đấu.

## Cấu hình

File cấu hình Server nằm tại `Code/Server/Config/ServerConfig.json`:
```json
{
  "Server": {
    "Ip": "0.0.0.0",
    "Port": 5000,
    "MaxClients": 100
  }
}
```
- **Thay đổi IP & Port**: Sửa trường `"Ip"` và `"Port"` trong file `ServerConfig.json` (phía Server) hoặc nhập IP trực tiếp trên giao diện màn hình `LoginForm` (phía Client).
- **Lưu ý bảo mật**: Không ghi password, chuỗi kết nối chứa mật khẩu hoặc dữ liệu nhạy cảm vào repository.

## Chức năng

- [x] Đăng nhập / Đăng ký tài khoản người chơi (kiểm tra tính hợp lệ dữ liệu)
- [x] Quản lý sảnh (Lobby): danh sách người chơi online, danh sách phòng chơi
- [x] Tạo phòng chơi mới, đặt tên phòng và tham gia phòng
- [x] Gửi lời mời thách đấu và chấp nhận / từ chối lời mời
- [x] Chơi game Caro thời gian thực (vẽ bàn cờ, đánh cờ theo lượt, xét duyệt thắng thua 5 nước tự động)
- [x] Tính giờ đếm ngược nước đi 30 giây (Timer) và tự động xử thua khi hết giờ
- [x] Chế độ đấu với máy (Bot AI nhiều cấp độ: Dễ, Vừa, Khó)
- [x] Cầu hòa, đầu hàng và nhắn tin trò chuyện trong phòng đấu (Chat)
- [x] Chế độ khán giả theo dõi trận đấu (Spectator mode)
- [x] Lưu trữ kết quả trận đấu và điểm số người chơi vào SQL Server
- [x] Tra cứu và hiển thị lịch sử thi đấu (Match History)
- [x] Xử lý mất kết nối & đếm ngược 90 giây giữ phòng chờ kết nối lại (Reconnect)

## Kiểm thử

- **Functional test**: Đã xây dựng và thực thi thành công 32 ca kiểm thử (TC_01 -> TC_32) bao phủ toàn diện: Xác thực tài khoản, Sảnh chờ Lobby, Thách đấu, Ván đấu thời gian thực, Tính giờ 30s, Luật thắng 5 nước, Lưu CSDL và Lịch sử đấu. Chi tiết lưu tại [`Extra/Test Cases Template.xlsx`](Extra/Test%20Cases%20Template.xlsx).
- **Test dữ liệu không hợp lệ**: Kiểm tra và chặn triệt để các trường hợp đăng nhập tài khoản không tồn tại (TC_24), đăng ký thiếu thông tin hoặc sai định dạng (TC_25), đánh cờ sai lượt hoặc click vào ô đã có quân cờ (TC_28).
- **Test mất kết nối**: Phát hiện sự cố rớt mạng đột ngột (End Task / mất socket), kích hoạt overlay khóa bàn cờ và đếm ngược 90s; hỗ trợ Reconnect khôi phục ván đấu nguyên vẹn hoặc xử thua nếu đối thủ bỏ cuộc (TC_17, TC_18, TC_19, TC_31).
- **Stress test**: Kiểm thử bão ngắt kết nối đồng loạt 50 - 60 client cùng lúc, Server dọn dẹp phiên trong 2.39 ms và phục hồi ngay lập tức sau 0.49 ms mà không xảy ra hiện tượng crash hay rò rỉ socket.
- **Performance test**: Thử nghiệm mở rộng kết nối từ 10 đến 200 client đồng thời (tỉ lệ thành công 100%, thời gian bắt tay TCP Handshake chỉ 0.27 - 0.58 ms); Thông lượng xử lý đạt đỉnh ~2,900 - 44,000 requests/giây; Độ trễ phản hồi vòng lặp RTT trung vị cực thấp ~0.03 - 3.74 ms. Chạy kiểm thử tự động bằng [`Extra/stress_test.py`](Extra/stress_test.py).

Bằng chứng kiểm thử lưu tại `Extra/` (ảnh chụp màn hình [`stress_test_result.png`](Extra/stress_test_result.png), nhật ký lệnh [`stress_test_result.txt`](Extra/stress_test_result.txt), và bảng test case [`Test Cases Template.xlsx`](Extra/Test%20Cases%20Template.xlsx)).

## Demo

- **Video**: https://youtu.be/7Rs3WFidfCY
- **Slide**: `PPTX/UDM_16_Caro_Game.pptx`
- **Báo cáo**: `DOCX/Net3_Group_08_UDM_16.docx`

## Giới hạn

- Giao diện hiện tại được xây dựng trên nền tảng Windows Forms cơ bản, chỉ hỗ trợ hệ điều hành Windows.
- Chưa hỗ trợ tính năng Voice Chat (âm thanh trực tiếp).
- Chưa có hệ thống tự động ghép cặp thông minh (Auto-matchmaking) dựa trên xếp hạng điểm Elo.
