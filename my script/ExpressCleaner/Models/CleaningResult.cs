using System;

namespace ExpressCleaner.Models
{
    /// <summary>
    /// Результат очистки
    /// </summary>
    public class CleaningResult
    {
        public int FilesDeleted { get; set; }
        public int FoldersCleared { get; set; }
        public long SpaceFreed { get; set; }
        public TimeSpan Duration { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CompletedAt { get; set; }

        public string GetFormattedSize()
        {
            return FormatBytes(SpaceFreed);
        }

        public static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
