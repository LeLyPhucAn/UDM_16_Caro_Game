using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Server.Config
{
    public static class ConfigLoader
    {
        public static ServerConfig Load()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, "Config", "ServerConfig.json");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Không tìm thấy file ServerConfig.json");
            }

            string json = File.ReadAllText(filePath);

            using JsonDocument document = JsonDocument.Parse(json);

            JsonElement server = document.RootElement.GetProperty("Server");

            return new ServerConfig
            {
                Ip = server.GetProperty("Ip").GetString()!,
                Port = server.GetProperty("Port").GetInt32(),
                MaxClients = server.GetProperty("MaxClients").GetInt32()
            };
        }
    }
}