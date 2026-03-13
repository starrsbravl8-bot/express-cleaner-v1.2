using System;
using System.Collections.Generic;

namespace ExpressCleaner.Services
{
    /// <summary>
    /// Сервис логирования
    /// </summary>
    public class Logger
    {
        private static readonly List<string> _logs = new();

        public void LogInfo(string message)
        {
            string logEntry = $"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            _logs.Add(logEntry);
            Console.WriteLine(logEntry);
        }

        public void LogError(string message)
        {
            string logEntry = $"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            _logs.Add(logEntry);
            Console.WriteLine(logEntry);
        }

        public void LogWarning(string message)
        {
            string logEntry = $"[WARNING] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            _logs.Add(logEntry);
            Console.WriteLine(logEntry);
        }

        public List<string> GetLogs()
        {
            return new List<string>(_logs);
        }

        public void ClearLogs()
        {
            _logs.Clear();
        }
    }
}
