using System;

namespace ExpressCleaner.Models
{
    /// <summary>
    /// Информация о системе
    /// </summary>
    public class SystemInfo
    {
        public string OSVersion { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public long TotalRAM { get; set; }
        public long AvailableRAM { get; set; }
        public DriveInfo[] Drives { get; set; } = Array.Empty<DriveInfo>();
    }

    public class DriveInfo
    {
        public string Name { get; set; } = string.Empty;
        public long TotalSize { get; set; }
        public long FreeSpace { get; set; }
        public string DriveType { get; set; } = string.Empty;
    }
}
