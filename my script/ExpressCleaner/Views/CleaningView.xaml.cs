using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ExpressCleaner.Services;
using ExpressCleaner.Models;

namespace ExpressCleaner.Views
{
    public partial class CleaningView : UserControl
    {
        private readonly SystemCleaner _systemCleaner;
        private readonly BrowserCleaner _browserCleaner;
        private List<CleaningCategory> _categories = new();

        public CleaningView()
        {
            InitializeComponent();
            _systemCleaner = new SystemCleaner();
            _browserCleaner = new BrowserCleaner();
            LoadCategories();
        }

        private async void LoadCategories()
        {
            try
            {
                var systemCategories = await _systemCleaner.AnalyzeSystem();
                var browserCategories = await _browserCleaner.AnalyzeBrowsers();
                _categories = systemCategories.Concat(browserCategories).ToList();

                foreach (var category in _categories)
                {
                    var checkBox = new CheckBox
                    {
                        Content = $"{category.Name} - {CleaningResult.FormatBytes(category.EstimatedSize)} ({category.FileCount} файлов)",
                        IsChecked = category.IsEnabled,
                        Margin = new Thickness(0, 5, 0, 5),
                        FontSize = 14,
                        Tag = category
                    };
                    checkBox.Checked += CategoryCheckBox_Changed;
                    checkBox.Unchecked += CategoryCheckBox_Changed;
                    
                    CategoriesPanel.Children.Add(checkBox);
                }

                UpdateTotalSize();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CategoryCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.Tag is CleaningCategory category)
            {
                category.IsEnabled = checkBox.IsChecked == true;
                UpdateTotalSize();
            }
        }

        private void UpdateTotalSize()
        {
            long totalSize = _categories.Where(c => c.IsEnabled).Sum(c => c.EstimatedSize);
            int totalFiles = _categories.Where(c => c.IsEnabled).Sum(c => c.FileCount);
            TotalSizeText.Text = $"Будет освобождено: {CleaningResult.FormatBytes(totalSize)} ({totalFiles} файлов)";
        }

        private async void Clean_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategories = _categories.Where(c => c.IsEnabled).ToList();
            
            if (selectedCategories.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одну категорию для очистки", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите выполнить очистку?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            BtnClean.IsEnabled = false;
            ProgressBar.Visibility = Visibility.Visible;
            ProgressBar.Value = 0;
            StatusText.Text = "Очистка...";

            try
            {
                var progress = new Progress<int>(value => ProgressBar.Value = value);
                
                var systemCategories = selectedCategories.Where(c => c.CategoryType == "System" || c.CategoryType == "Temp").ToList();
                var browserCategories = selectedCategories.Where(c => c.CategoryType == "Browser").ToList();

                CleaningResult cleaningResult = new CleaningResult();

                if (systemCategories.Any())
                {
                    var sysResult = await _systemCleaner.CleanCategories(systemCategories, progress);
                    cleaningResult.FilesDeleted += sysResult.FilesDeleted;
                    cleaningResult.FoldersCleared += sysResult.FoldersCleared;
                    cleaningResult.SpaceFreed += sysResult.SpaceFreed;
                }

                if (browserCategories.Any())
                {
                    var browserResult = await _browserCleaner.CleanBrowsers(browserCategories, progress);
                    cleaningResult.FilesDeleted += browserResult.FilesDeleted;
                    cleaningResult.FoldersCleared += browserResult.FoldersCleared;
                    cleaningResult.SpaceFreed += browserResult.SpaceFreed;
                }

                StatusText.Text = "Очистка завершена";
                MessageBox.Show($"Очистка завершена!\n\nУдалено файлов: {cleaningResult.FilesDeleted}\nОчищено папок: {cleaningResult.FoldersCleared}\nОсвобождено места: {cleaningResult.GetFormattedSize()}", 
                    "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка очистки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Ошибка очистки";
            }
            finally
            {
                ProgressBar.Visibility = Visibility.Collapsed;
                BtnClean.IsEnabled = true;
            }
        }
    }
}
