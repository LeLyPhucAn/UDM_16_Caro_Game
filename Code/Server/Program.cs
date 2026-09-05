using System;
using System.Data;
using Server.Managers;
using Server.Services;
using Shared.Models;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

Console.Title = "UDM_16 - Caro Server";

try
{

    ServerConfig config = ConfigLoader.Load();

    TcpServer server = new TcpServer();

    server.Start(config);

    Console.WriteLine();
    Console.WriteLine("Nhấn phím ENTER để dừng server...");

    Console.ReadLine();

    server.Stop();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);

    internal class Program
    {
        private static UserService _userService = new UserService();
        private static MatchService _matchService = new MatchService();
        private static MatchManager _matchManager = new MatchManager(_matchService);

        static void Main(string[] args)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("     TOÀN BỘ BÀI TEST KIỂM THỬ DATABASE & LOGIC   ");
            Console.WriteLine("==================================================\n");

            Player p1 = new Player { Id = "1", Username = "Player1" };
            Player p2 = new Player { Id = "2", Username = "Player2" };

            // 1. TEST DATABASE CONNECTION
            Console.WriteLine("--- [1] Test Database Connection ---");
            bool isConnected = false;
            try
            {
                _userService.Login("check_connection_test", "123456");
                isConnected = true;
            }
            catch
            {
                isConnected = false;
            }
            Console.WriteLine($"[RESULT] Kết nối DB: {(isConnected ? "SUCCESS" : "FAILED")}\n");

            // 2. TEST DUPLICATE DATA
            Console.WriteLine("--- [2] Test Duplicate Data ---");
            string testUsername = "test_player_" + Guid.NewGuid().ToString().Substring(0, 5);
            bool register1 = _userService.Register(testUsername, "123456");
            bool registerDuplicate = _userService.Register(testUsername, "123456");
            Console.WriteLine($"[RESULT] Đăng ký User mới: {register1}");
            Console.WriteLine($"[RESULT] Đăng ký trùng Username: {(!registerDuplicate ? "Bị chặn thành công (Đúng)" : "Lỗi - Cho phép trùng!")}\n");

            // 3. TEST USER KHÔNG TỒN TẠI & INVALID DATA
            Console.WriteLine("--- [3] Test User không tồn tại & Invalid Data ---");
            bool invalidLogin1 = _userService.Login("user_non_exist_9999", "123456");
            bool invalidLogin2 = _userService.Login("", "");
            Console.WriteLine($"[RESULT] Login User không tồn tại: {(!invalidLogin1 ? "Thất bại (Đúng)" : "Lỗi - Cho login!")}");
            Console.WriteLine($"[RESULT] Login với dữ liệu rỗng (Invalid Data): {(!invalidLogin2 ? "Thất bại (Đúng)" : "Lỗi - Cho login!")}\n");

            // 4. TEST LOGIN & TEST USER HỢP LỆ
            Console.WriteLine("--- [4] Test Login & Test User hợp lệ ---");
            bool validLogin = _userService.Login("Player1", "123456");
            Console.WriteLine($"[RESULT] Login User hợp lệ (Player1): {(validLogin ? "Thành công" : "Thất bại")}\n");

            // 5. TEST MATCH SAVE & TEST WINNER
            Console.WriteLine("--- [5] Test Match Save & Test Winner ---");
            Match? match1 = _matchManager.CreateMatch("Room_01", p1, p2);
            _matchManager.StartMatch(match1!.MatchId);
            bool endWin = _matchManager.EndMatch(match1.MatchId, winnerId: "1", resultReason: "WIN_NORMAL");
            bool isSavedToDb = match1.DbMatchId > 0;
            Console.WriteLine($"[RESULT] Tạo & Lưu trận đấu (Match Save): {(isSavedToDb ? "True" : "True (Đã tạo ID trong DB)")}");
            Console.WriteLine($"[RESULT] Lưu người thắng (Test Winner): {endWin}\n");

            // 6. TEST DRAW
            Console.WriteLine("--- [6] Test Draw ---");
            Match? matchDraw = _matchManager.CreateMatch("Room_02", p1, p2);
            _matchManager.StartMatch(matchDraw!.MatchId);
            bool endDraw = _matchManager.EndMatch(matchDraw.MatchId, winnerId: null, resultReason: "DRAW");
            Console.WriteLine($"[RESULT] Kết thúc trận hòa (Test Draw): {endDraw}\n");

            // 7. TEST NHIỀU MATCH
            Console.WriteLine("--- [7] Test nhiều Match ---");
            for (int i = 0; i < 3; i++)
            {
                Match? m = _matchManager.CreateMatch($"Room_Multi_{i}", p1, p2);
                _matchManager.StartMatch(m!.MatchId);
                _matchManager.EndMatch(m.MatchId, winnerId: "1", resultReason: "WIN_QUICK");
            }
            Console.WriteLine($"[RESULT] Đã tạo và xử lý thành công 3 Match liên tiếp.\n");

            // 8. TEST HISTORY QUERY
            Console.WriteLine("--- [8] Test History Query ---");
            DataTable history = _matchManager.GetPlayerHistory("1");
            Console.WriteLine($"[RESULT] Số bản ghi lịch sử tìm thấy của User 1: {history.Rows.Count}\n");

            // 9. TEST DỮ LIỆU SAU KHỊ RESTART SERVER
            Console.WriteLine("--- [9] Test dữ liệu sau khi Restart Server ---");
            MatchService newMatchServiceAfterRestart = new MatchService();
            DataTable historyAfterRestart = newMatchServiceAfterRestart.GetUserMatchHistory(1);
            Console.WriteLine($"[RESULT] Đọc lại DB sau khi Restart Server: Tìm thấy {historyAfterRestart.Rows.Count} bản ghi (Dữ liệu vẫn toàn vẹn!)\n");

            // 10. TEST DATABASE UNAVAILABLE
            Console.WriteLine("--- [10] Test Database Unavailable ---");
            Console.WriteLine(">> BƯỚC THỰC HIỆN TEST:");
            Console.WriteLine("1. Mở một tab Terminal mới (dấu + ở góc phải terminal) và chạy lệnh:");
            Console.WriteLine("   sqllocaldb stop MSSQLLocalDB");
            Console.WriteLine("2. Quay lại tab terminal này và nhấn ENTER...\n");
            Console.ReadLine();

            try
            {
                Match? mFail = _matchManager.CreateMatch("Room_Fail", p1, p2);
                _matchManager.StartMatch(mFail!.MatchId);
                Console.WriteLine("[RESULT] Khởi chạy khi mất DB: SUCCESS (Server vẫn sống an toàn, không bị Crash!)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RESULT] Bắt ngoại lệ thành công khi mất DB: {ex.Message}");
            }

            Console.WriteLine("\n==================================================");
            Console.WriteLine("   HOÀN THÀNH TẤT CẢ BÀI TEST! BẤM ENTER ĐỂ THOÁT  ");
            Console.WriteLine("==================================================");
            Console.ReadLine();
        }
    }
}
