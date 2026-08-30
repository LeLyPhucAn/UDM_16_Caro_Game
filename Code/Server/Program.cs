using System;
using Server.Managers;
using Shared.Models; // Hoặc namespace chứa class Player của bạn

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== BẮT ĐẦU TEST ĐỘC LẬP TASK 2 (DATABASE INTEGRATION) ===");

            MatchManager matchManager = new MatchManager();

            // 1. Tạo 2 Player giả lập (Sửa truyền tham số trực tiếp vào Constructor)
            Player p1 = new Player("1", "UserA");
            Player p2 = new Player("2", "UserB");

            // 2. Test Tạo trận & Bắt đầu trận (Kiểm tra xem có lưu vào DB Matches không)
            Console.WriteLine("\n--- TEST 1: Khởi tạo & Bắt đầu trận đấu ---");
            var match = matchManager.CreateMatch("Room_Test_1", p1, p2);
            if (match != null)
            {
                bool startSuccess = matchManager.StartMatch(match.MatchId);
                Console.WriteLine($"[RESULT] StartMatch Status: {startSuccess} | DbMatchId: {match.DbMatchId}");
            }

            // 3. Test Kết thúc trận & Lưu kết quả
            Console.WriteLine("\n--- TEST 2: Kết thúc trận đấu ---");
            if (match != null)
            {
                bool endSuccess = matchManager.EndMatch(match.MatchId, winnerId: "1", resultReason: "WIN_BY_CHECK");
                Console.WriteLine($"[RESULT] EndMatch Status: {endSuccess}");
            }

            // 4. Test Truy vấn Lịch sử đấu
            Console.WriteLine("\n--- TEST 3: Truy vấn Lịch sử đấu của User 1 ---");
            var historyTable = matchManager.GetPlayerHistory("1");
            Console.WriteLine($"[RESULT] Số bản ghi lịch sử tìm thấy: {historyTable.Rows.Count}");

            // 5. TEST QUAN TRỌNG: Mất kết nối DB có làm crash Server không?
            Console.WriteLine("\n--- TEST 4: Giả lập lỗi DB (Hãy gõ 'sqllocaldb stop MSSQLLocalDB' ở terminal trước khi bấm Enter) ---");
            Console.ReadLine(); // Tạm dừng để bạn ra terminal tắt DB

            var match2 = matchManager.CreateMatch("Room_Test_2", p1, p2);
            if (match2 != null)
            {
                bool startFailSafe = matchManager.StartMatch(match2.MatchId);
                Console.WriteLine($"[RESULT] Khởi chạy khi mất DB: {startFailSafe} (Server vẫn sống an toàn!)");
            }

            Console.WriteLine("\n=== HOÀN THÀNH TEST! BẤM ENTER ĐỂ THOÁT ===");
            Console.ReadLine();
        }
    }
}