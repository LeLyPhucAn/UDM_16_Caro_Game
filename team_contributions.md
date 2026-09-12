# Báo Cáo Đóng Góp Thành Viên Nhóm (UDM_16_Caro_Game)

Báo cáo này được tạo dựa trên lịch sử commit Git trong suốt 5 tuần thực hiện đồ án môn Lập Trình Mạng.

## Danh sách thành viên và Tóm tắt công việc

### 1. Lê Lý Phúc An (Nhóm trưởng - GitHub: LeLyPhucAn)
- **Công việc:** Khởi tạo project, cấu trúc thư mục, quản lý Repository. Thiết lập nền tảng Server Core (TcpServer, ClientSession). Triển khai Connection Manager, Message Handler. Tối ưu, refactor packet parser và xử lý chung các merge request. 
- **Đóng góp nổi bật:** Thiết kế Server đa luồng (Multi-threading) xử lý nhiều kết nối TCP cùng lúc.

### 2. Nguyễn Hữu Khuyến (GitHub: khuyen nguyen)
- **Công việc:** Thiết kế và xây dựng chuẩn giao thức giao tiếp giữa Client và Server (Protocol). Xây dựng cấu trúc các Message (Models) và viết Packet Parser ban đầu (Tuần 2).
- **Đóng góp nổi bật:** Tạo bộ khung tuần tự hóa/giải tuần tự hóa JSON giúp truyền thông tin chuẩn xác.

### 3. Đỗ Trọng Kiệt (GitHub: lekiet78)
- **Công việc:** Thực hiện logic game (Board Control, kiểm tra thắng thua), xây dựng hệ thống Room Manager và Match Manager trên Server.
- **Đóng góp nổi bật:** Xây dựng cơ chế phòng chơi động (Room) và quản lý trạng thái trận đấu (Match) trên bộ nhớ.

### 4. Võ Văn Kiều (GitHub: kieuvo241-3525)
- **Công việc:** Thiết kế Database Layer (SQL Server). Vẽ sơ đồ ERD. Xây dựng Repository pattern cho User, Match, History. Viết các UserService và MatchService.
- **Đóng góp nổi bật:** Lưu trữ dữ liệu an toàn, xử lý logic đăng ký, đăng nhập và ghi nhận lịch sử thi đấu vào CSDL.

### 5. Võ Thị Lệ Kim (GitHub: kim83863979)
- **Công việc:** Thiết kế toàn bộ giao diện Client (Windows Forms): Login Form, Lobby Form, Room Form, Game Form. Tích hợp UI với các sự kiện Network ở phía Client.
- **Đóng góp nổi bật:** Tạo trải nghiệm người dùng mượt mà, xử lý đa luồng an toàn khi cập nhật giao diện từ luồng mạng.

### 6. Nguyễn Văn Hiếu (GitHub: hieunv5364-hue)
- **Công việc:** Xây dựng Client Network Layer (ClientConnection, TcpClientService). Quản lý kết nối TCP từ phía Client gửi lên Server.
- **Đóng góp nổi bật:** Duy trì kết nối ổn định, xử lý các sự kiện ngắt kết nối và truyền nhận byte từ luồng TCP lên tầng logic.
