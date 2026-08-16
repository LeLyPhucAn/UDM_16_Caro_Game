using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    internal static class Program
    {
        [STAThread]
        static async Task Main()
        {
            ApplicationConfiguration.Initialize();

            // 🟢 CODE TEST KẾT NỐI SANG SERVER (127.0.0.1:5000)
            _ = Task.Run(async () =>
            {
                await Task.Delay(1000); // Chờ 1 giây cho Server khởi động xong
                try
                {
                    using TcpClient client = new TcpClient();
                    await client.ConnectAsync("127.0.0.1", 5000);
                    MessageBox.Show("Client đã kết nối thành công tới Server!", "Thông báo Test Tuần 1");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi kết nối: {ex.Message}");
                }
            });

            Application.Run(new Form());
        }
    }
}
