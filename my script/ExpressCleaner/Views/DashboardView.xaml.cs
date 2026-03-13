using System.Windows;
using System.Windows.Controls;
using ExpressCleaner.Services;

namespace ExpressCleaner.Views
{
    public partial class DashboardView : UserControl
    {
        private readonly DiskAnalyzer _diskAnalyzer;

        public DashboardView()
        {
            InitializeComponent();
            _diskAnalyzer = new DiskAnalyzer();
            LoadDashboard();
        }

        private void LoadDashboard()
        {
            var systemInfo = _diskAnalyzer.GetSystemInfo();
            InfoText.Text = $"Система: {systemInfo.OSVersion}\nКомпьютер: {systemInfo.MachineName}\n\nДиски:\n";
            
            foreach (var drive in systemInfo.Drives)
            {
                InfoText.Text += $"{drive.Name} - Свободно: {Models.CleaningResult.FormatBytes(drive.FreeSpace)} из {Models.CleaningResult.FormatBytes(drive.TotalSize)}\n";
            }
        }

        private void QuickAnalysis_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Перейдите в раздел 'Анализ' для полного анализа системы", "Express Cleaner", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CleanTemp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Перейдите в раздел 'Очистка' для очистки системы", "Express Cleaner", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SystemInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Перейдите в раздел 'Инструменты' для просмотра информации о системе", "Express Cleaner", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
