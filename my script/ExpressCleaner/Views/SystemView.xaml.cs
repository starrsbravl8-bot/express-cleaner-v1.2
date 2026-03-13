using System.Windows.Controls;
using ExpressCleaner.Services;

namespace ExpressCleaner.Views
{
    public partial class SystemView : UserControl
    {
        private readonly DiskAnalyzer _diskAnalyzer;

        public SystemView()
        {
            InitializeComponent();
            _diskAnalyzer = new DiskAnalyzer();
            LoadSystemInfo();
        }

        private void LoadSystemInfo()
        {
            var systemInfo = _diskAnalyzer.GetSystemInfo();
            
            SystemInfoText.Text = $"Операционная система: {systemInfo.OSVersion}\n";
            SystemInfoText.Text += $"Имя компьютера: {systemInfo.MachineName}\n\n";
            SystemInfoText.Text += "Диски:\n";
            
            foreach (var drive in systemInfo.Drives)
            {
                SystemInfoText.Text += $"\n{drive.Name}\n";
                SystemInfoText.Text += $"  Тип: {drive.DriveType}\n";
                SystemInfoText.Text += $"  Всего: {Models.CleaningResult.FormatBytes(drive.TotalSize)}\n";
                SystemInfoText.Text += $"  Свободно: {Models.CleaningResult.FormatBytes(drive.FreeSpace)}\n";
                SystemInfoText.Text += $"  Занято: {Models.CleaningResult.FormatBytes(drive.TotalSize - drive.FreeSpace)}\n";
            }
        }
    }
}
