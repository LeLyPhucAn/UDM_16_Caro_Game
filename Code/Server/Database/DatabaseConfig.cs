namespace Server.Database;

public class DatabaseConfig
{
    public string ConnectionString { get; set; } = @"Server=(localdb)\mssqllocaldb;Database=CaroGameDb;Trusted_Connection=True;";
}