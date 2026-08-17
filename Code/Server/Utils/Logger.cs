using System;

namespace Server.Utils;

public static class Logger
{
    private static readonly object _lock = new();

    public static void Info(string message)
    {
        Log("INFO", message, ConsoleColor.Green);
    }

    public static void Warn(string message)
    {
        Log("WARN", message, ConsoleColor.Yellow);
    }

    public static void Error(string message, Exception? ex = null)
    {
        string fullMessage = ex == null ? message : $"{message} | Chi tiết: {ex.Message}";
        Log("ERROR", fullMessage, ConsoleColor.Red);
    }

    public static void Debug(string message)
    {
        Log("DEBUG", message, ConsoleColor.Cyan);
    }

    private static void Log(string level, string message, ConsoleColor color)
    {
        lock (_lock)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // In thời gian màu xám
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{timestamp}] ");

            // In cấp độ log theo màu tương ứng
            Console.ForegroundColor = color;
            Console.Write($"[{level,-5}] ");

            // In nội dung thông báo
            Console.ForegroundColor = originalColor;
            Console.WriteLine(message);
        }
    }
}
