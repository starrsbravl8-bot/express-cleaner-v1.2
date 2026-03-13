using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ExpressCleaner.Services;
using ExpressCleaner.Models;

namespace ExpressCleaner.Views
{
    public partial class AnalysisView : UserControl
    {
        private readonly SystemCleaner _systemCleaner;
        private readonly BrowserCleaner _browserCleaner;

        public AnalysisView()
        {
            InitializeComponent();
            _systemCleaner = new SystemCleaner();
            _browserCleaner = new BrowserCleaner();
        }

        private async void Analyze_Click(object sender, RoutedEventArgs e)
        {
            BtnAnalyze.IsEnabled = false;
            StatusText.Text = "Анализ системы...";
            ProgressBar.Visibility = Visibility.Visible;
            ProgressBar.IsIndeterminate = true;
            CategoriesPanel.Children.Clear();

            try
            {
                var systemCategories = await _systemCleaner.AnalyzeSystem();
                var browserCategories = await _browserCleaner.AnalyzeBrowsers();
                
                var allCategories = systemCategories.Concat(browserCategories).ToList();
                
                long totalSize = 0;
                int totalFiles = 0;

                foreach (var category in allCategories)
                {
                    totalSize += category.EstimatedSize;
                    totalFiles += category.FileCount;

                    var panel = new StackPanel { Margin = new Thickness(0, 5, 0, 5) };
                    
                    var header = new TextBlock
                    {
                        Text = $"{category.Name} - {CleaningResult.FormatBytes(category.EstimatedSize)} ({category.FileCount} файлов)",
                        FontWeight = FontWeights.Bold,
                        FontSize = 14
                    };
                    
                    var description = new TextBlock
                    {
                        Text = category.Description,
                        Foreground = System.Windows.Media.Brushes.Gray,
                        Margin = new Thickness(0, 2, 0, 0)
                    };
                    
                    panel.Children.Add(header);
                    panel.Children.Add(description);
                    panel.Children.Add(new Separator { Margin = new Thickness(0, 5, 0, 0) });
                    
                    CategoriesPanel.Children.Add(panel);
                }

                ResultText.Text = $"Найдено: {totalFiles} файлов, можно освободить: {CleaningResult.FormatBytes(totalSize)}";
                StatusText.Text = "Анализ завершен";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка анализа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Ошибка анализа";
            }
            finally
            {
                ProgressBar.IsIndeterminate = false;
                ProgressBar.Visibility = Visibility.Collapsed;
                BtnAnalyze.IsEnabled = true;
            }
        }
    }
}
