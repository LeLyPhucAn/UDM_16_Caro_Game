using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Config
{
    public class ServerConfig
    {
        public string Ip { get; set; } = "127.0.0.1";

        public int Port { get; set; } = 5000;

        public int MaxClients { get; set; } = 100;
    }
}