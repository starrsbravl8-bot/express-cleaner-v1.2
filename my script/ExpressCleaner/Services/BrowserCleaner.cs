using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExpressCleaner.Models;

namespace ExpressCleaner.Services
{
    /// <summary>
    /// Сервис для очистки браузеров
    /// </summary>
    public class BrowserCleaner
    {
        private readonly Logger _logger;

        public BrowserCleaner()
        {
            _logger = new Logger();
        }

        /// <summary>
        /// Анализ кэша браузеров
        /// </summary>
        public async Task<List<CleaningCategory>> AnalyzeBrowsers()
        {
            var categories = new List<CleaningCategory>();

            await Task.Run(() =>
            {
                categories.Add(AnalyzeChrome());
                categories.Add(AnalyzeEdge());
                categories.Add(AnalyzeFirefox());
                categories.Add(AnalyzeOpera());
                categories.Add(AnalyzeBrave());
            });

            return categories;
        }

        private CleaningCategory AnalyzeChrome()
        {
            var category = new CleaningCategory
            {
                Name = "Google Chrome",
                Description = "Кэш и временные файлы Chrome",
                CategoryType = "Browser"
            };

            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string chromePath = Path.Combine(localAppData, @"Google\Chrome\User Data\Default\Cache");

                if (Directory.Exists(chromePath))
                {
                    var files = Directory.GetFiles(chromePath, "*.*", SearchOption.AllDirectories);
                    category.FileCount = files.Length;
                    category.EstimatedSize = files.Sum(f => new FileInfo(f).Length);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка анализа Chrome: {ex.Message}");
            }

            return category;
        }

        private CleaningCategory AnalyzeEdge()
        {
            var category = new CleaningCategory
            {
                Name = "Microsoft Edge",
                Description = "Кэш и временные файлы Edge",
                CategoryType = "Browser"
            };

            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string edgePath = Path.Combine(localAppData, @"Microsoft\Edge\User Data\Default\Cache");
                
                if (Directory.Exists(edgePath))
                {
                    var files = Directory.GetFiles(edgePath, "*.*", SearchOption.AllDirectories);
                    category.FileCount = files.Length;
                    category.EstimatedSize = files.Sum(f => new FileInfo(f).Length);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка анализа Edge: {ex.Message}");
            }

            return category;
        }

        private CleaningCategory AnalyzeFirefox()
        {
            var category = new CleaningCategory
            {
                Name = "Mozilla Firefox",
                Description = "Кэш и временные файлы Firefox",
                CategoryType = "Browser"
            };

            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string firefoxPath = Path.Combine(localAppData, @"Mozilla\Firefox\Profiles");
                
                if (Directory.Exists(firefoxPath))
                {
                    var profiles = Directory.GetDirectories(firefoxPath);
                    foreach (var profile in profiles)
                    {
                        string cachePath = Path.Combine(profile, "cache2");
                        if (Directory.Exists(cachePath))
                        {
                            var files = Directory.GetFiles(cachePath, "*.*", SearchOption.AllDirectories);
                            category.FileCount += files.Length;
                            category.EstimatedSize += files.Sum(f => new FileInfo(f).Length);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка анализа Firefox: {ex.Message}");
            }

            return category;
        }

        private CleaningCategory AnalyzeOpera()
        {
            var category = new CleaningCategory
            {
                Name = "Opera",
                Description = "Кэш и временные файлы Opera",
                CategoryType = "Browser"
            };

            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string operaPath = Path.Combine(appData, @"Opera Software\Opera Stable\Cache");
                
                if (Directory.Exists(operaPath))
                {
                    var files = Directory.GetFiles(operaPath, "*.*", SearchOption.AllDirectories);
                    category.FileCount = files.Length;
                    category.EstimatedSize = files.Sum(f => new FileInfo(f).Length);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка анализа Opera: {ex.Message}");
            }

            return category;
        }

        private CleaningCategory AnalyzeBrave()
        {
            var category = new CleaningCategory
            {
                Name = "Brave",
                Description = "Кэш и временные файлы Brave",
                CategoryType = "Browser"
            };

            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string bravePath = Path.Combine(localAppData, @"BraveSoftware\Brave-Browser\User Data\Default\Cache");
                
                if (Directory.Exists(bravePath))
                {
                    var files = Directory.GetFiles(bravePath, "*.*", SearchOption.AllDirectories);
                    category.FileCount = files.Length;
                    category.EstimatedSize = files.Sum(f => new FileInfo(f).Length);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка анализа Brave: {ex.Message}");
            }

            return category;
        }

        /// <summary>
        /// Очистка выбранных браузеров
        /// </summary>
        public async Task<CleaningResult> CleanBrowsers(List<CleaningCategory> categories, IProgress<int> progress)
        {
            var result = new CleaningResult
            {
                Success = true,
                CompletedAt = DateTime.Now
            };

            var startTime = DateTime.Now;
            int totalCategories = categories.Count(c => c.IsEnabled);
            int currentCategory = 0;

            await Task.Run(() =>
            {
                foreach (var category in categories.Where(c => c.IsEnabled))
                {
                    try
                    {
                        switch (category.Name)
                        {
                            case "Google Chrome":
                                CleanChrome(result);
                                break;
                            case "Microsoft Edge":
                                CleanEdge(result);
                                break;
                            case "Mozilla Firefox":
                                CleanFirefox(result);
                                break;
                            case "Opera":
                                CleanOpera(result);
                                break;
                            case "Brave":
                                CleanBrave(result);
                                break;
                        }

                        currentCategory++;
                        progress?.Report((currentCategory * 100) / totalCategories);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Ошибка очистки {category.Name}: {ex.Message}");
                    }
                }
            });

            result.Duration = DateTime.Now - startTime;
            result.Message = "Очистка браузеров завершена";
            
            return result;
        }

        private void CleanChrome(CleaningResult result)
        {
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string chromePath = Path.Combine(localAppData, @"Google\Chrome\User Data\Default\Cache");
                DeleteBrowserCache(chromePath, result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка очистки Chrome: {ex.Message}");
            }
        }

        private void CleanEdge(CleaningResult result)
        {
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string edgePath = Path.Combine(localAppData, @"Microsoft\Edge\User Data\Default\Cache");
                DeleteBrowserCache(edgePath, result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка очистки Edge: {ex.Message}");
            }
        }

        private void CleanFirefox(CleaningResult result)
        {
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string firefoxPath = Path.Combine(localAppData, @"Mozilla\Firefox\Profiles");
                
                if (Directory.Exists(firefoxPath))
                {
                    var profiles = Directory.GetDirectories(firefoxPath);
                    foreach (var profile in profiles)
                    {
                        string cachePath = Path.Combine(profile, "cache2");
                        DeleteBrowserCache(cachePath, result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка очистки Firefox: {ex.Message}");
            }
        }

        private void CleanOpera(CleaningResult result)
        {
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string operaPath = Path.Combine(appData, @"Opera Software\Opera Stable\Cache");
                DeleteBrowserCache(operaPath, result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка очистки Opera: {ex.Message}");
            }
        }

        private void CleanBrave(CleaningResult result)
        {
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string bravePath = Path.Combine(localAppData, @"BraveSoftware\Brave-Browser\User Data\Default\Cache");
                DeleteBrowserCache(bravePath, result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка очистки Brave: {ex.Message}");
            }
        }

        private void DeleteBrowserCache(string path, CleaningResult result)
        {
            if (!Directory.Exists(path)) return;

            try
            {
                var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    try
                    {
                        var fileInfo = new FileInfo(file);
                        long size = fileInfo.Length;
                        File.Delete(file);
                        result.FilesDeleted++;
                        result.SpaceFreed += size;
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка удаления кэша браузера: {ex.Message}");
            }
        }
    }
}
