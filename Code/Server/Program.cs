using Server.Config;
using Server.Network;

Console.Title = "UDM_16 - Caro Server";

try
{
    ServerConfig config = ConfigLoader.Load();

    TcpServer server = new TcpServer();

    server.Start(config);

    Console.WriteLine();
    Console.WriteLine("Press ENTER to stop server...");

    Console.ReadLine();

    server.Stop();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}