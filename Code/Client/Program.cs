using System;
using System.Windows.Forms;

namespace Client
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Khởi động giao diện Đăng nhập chính thức (Không tạo kết nối ngầm nữa)
            Application.Run(new Client.Forms.LoginForm());
        }
    }
}