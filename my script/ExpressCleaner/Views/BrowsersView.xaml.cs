using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ExpressCleaner.Services;

namespace ExpressCleaner.Views
{
    public partial class BrowsersView : UserControl
    {
        private readonly BrowserCleaner _browserCleaner;

        public BrowsersView()
        {
            InitializeComponent();
            _browserCleaner = new BrowserCleaner();
            LoadBrowsers();
        }

        private async void LoadBrowsers()
        {
            try
            {
                var browsers = await _browserCleaner.AnalyzeBrowsers();
                
                foreach (var browser in browsers)
                {
                    var panel = new StackPanel { Margin = new Thickness(0, 10, 0, 10) };
                    
                    var header = new TextBlock
                    {
                        Text = browser.Name,
                        FontSize = 18,
                        FontWeight = FontWeights.Bold
                    };
                    
                    var info = new TextBlock
                    {
                        Text = $"Кэш: {Models.CleaningResult.FormatBytes(browser.EstimatedSize)} ({browser.FileCount} файлов)",
                        FontSize = 14,
                        Margin = new Thickness(0, 5, 0, 0)
                    };
                    
                    panel.Children.Add(header);
                    panel.Children.Add(info);
                    panel.Children.Add(new Separator { Margin = new Thickness(0, 10, 0, 0) });
                    
                    BrowsersPanel.Children.Add(panel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки браузеров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
