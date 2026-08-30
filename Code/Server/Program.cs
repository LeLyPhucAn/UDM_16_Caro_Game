using System.Text;
using Server.Config;
using Server.Network;

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
}
