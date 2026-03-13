using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExpressCleaner.Models;

namespace ExpressCleaner.Services
{
    /// <summary>
    /// Сервис анализа дисков
    /// </summary>
    public class DiskAnalyzer
    {
        public SystemInfo GetSystemInfo()
        {
            var systemInfo = new SystemInfo
            {
                OSVersion = Environment.OSVersion.ToString(),
                MachineName = Environment.MachineName
            };

            var drives = new List<DriveInfo>();
            foreach (var drive in System.IO.DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    drives.Add(new DriveInfo
                    {
                        Name = drive.Name,
                        TotalSize = drive.TotalSize,
                        FreeSpace = drive.AvailableFreeSpace,
                        DriveType = drive.DriveType.ToString()
                    });
                }
            }

            systemInfo.Drives = drives.ToArray();
            return systemInfo;
        }
    }
}
