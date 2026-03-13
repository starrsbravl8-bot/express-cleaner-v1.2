using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ExpressCleaner.Views
{
    public partial class ToolsView : UserControl
    {
        public ToolsView()
        {
            InitializeComponent();
        }

        private void ClearDNS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c ipconfig /flushdns",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                };

                var process = Process.Start(psi);
                process?.WaitForExit();

                MessageBox.Show("DNS кэш успешно очищен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка очистки DNS: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DiskCleanup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("cleanmgr.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка запуска очистки диска: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SystemInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("msinfo32.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка запуска информации о системе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
