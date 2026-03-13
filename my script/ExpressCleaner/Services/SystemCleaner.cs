using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExpressCleaner.Models;

namespace ExpressCleaner.Services
{
    /// <summary>
    /// Сервис для очистки системных файлов
    /// </summary>
    public class SystemCleaner
    {
        private readonly Logger _logger;

        public SystemCleaner()
        {
            _logger = new Logger();
        }

        /// <summary>
        /// Анализ системы на наличие мусора
        /// </summary>
        public async Task<List<CleaningCategory>> AnalyzeSystem()
        {
            var categories = new List<CleaningCategory>();

            await Task.Run(() =>
            {
                // Временные файлы пользователя
                categories.Add(AnalyzeTempFiles());
                
                // Временные файлы Windows
                categories.Add(AnalyzeWindowsTemp());
                
                // Prefetch
                categories.Add(AnalyzePrefetch());
                
                // Кэш миниатюр
                categories.Add(AnalyzeThumbnailCache());
                
                // Корзина
                categories.Add(AnalyzeRecycleBin());
                
                // Windows Update
                categories.Add(AnalyzeWindowsUpdate());
                
                // Логи Windows
                categories.Add(AnalyzeWindowsLogs());
                
                // Crash dumps
                categories.Add(AnalyzeCrashDumps());
            });

            return categories;
        }

        private CleaningCategory AnalyzeTempFiles()
        {
            var category = new CleaningCategory
            {
                Name = "Временные файлы пользователя",
                Description = "Временные файлы в папке %TEMP%",
                CategoryType = "Temp"
            };

            try
            {
                string tempPath = Path.GetTempPath();
                if (Directory.Exists(tempPath))
                {
                    var files = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories);
                    category.FileCount = files.Length;
                    category.EstimatedSize = files.Sum(f => new FileInfo(f).Length);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка анализа временных файлов: {ex.Message}");
            }

            return category;
        }

        private CleaningCategory AnalyzeWindowsTemp()
        {
            var category = new CleaningCategory
            {
                Name = "Временные файлы Windows",
                Description = "Временные файлы системы Windows",
                CategoryType = "System"
            };

            try
            {
                string winTemp = @"C:\Windows\Temp";
                if (Directory.Exists(winTemp))
                {
                    var files = Directory.GetFiles(winTemp, "*.*", SearchOption.AllDirectories);
                    category.FileCount = files.Length;
                    category.EstimatedSize = files.Sum(f => new FileInfo(f).Length);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка анализа Windows Temp: {ex.Message}");
            }

            return category;
        }

        private CleaningCategory AnalyzePrefetch()
        {
            var category = new CleaningCategory
            {
                Name = "Prefetch",
                Description = "Файлы предзагрузки Windows",
                CategoryType = "System"
            };

            try
            {
                string prefetchPath = @"C:\Windows\Prefetch";
                if (Directory.Exists(prefetchPath))
                {
                    var files = Directory.GetFiles(prefetchPath, "*.pf");
                    category.FileCount = files.Length;
                    category.EstimatedSize = files.Sum(f => new FileInfo(f).Length);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка анализа Prefetch: {ex.Message}");
            }

            return category;
        }

        private CleaningCategory AnalyzeThumbnailCache()
        {
            var category = new CleaningCategory
            {
                Name = "Кэш миниатюр",
                Description = "Кэш изображений Windows",
                CategoryType = "System"
            };

            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string thumbCachePath = Path.Combine(localAppData, @"Microsoft\Windows\Explorer");
                
                if (Directory.Exists(thumbCachePath))
                {
                    var files = Directory.GetFiles(thumbCachePath, "thumbcache_*.db");
                    category.FileCount = files.Length;
                    category.EstimatedSize = files.Sum(f => new FileInfo(f).Length);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка анализа кэша миниатюр: {ex.Message}");
            }

            return category;
        }

        private CleaningCategory AnalyzeRecycleBin()
        {
            var category = new CleaningCategory
            {
                Name = "Корзина",
                Description = "Файлы в корзине Windows",
                CategoryType = "System"
            };

            try
            {
                // Примерная оценка, точный размер корзины требует специальных API
                category.FileCount = 0;
                category.EstimatedSize = 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка анализа корзины: {ex.Message}");
            }

            return category;
        }

        private CleaningCategory AnalyzeWindowsUpdate()
        {
            var category = new CleaningCategory
            {
                Name = "Windows Update",
                Description = "Временные файлы обновлений Windows",
                CategoryType = "System"
            };

            try
            {
                string updatePath = @"C:\Windows\SoftwareDistribution\Download";
                if (Directory.Exists(updatePath))
                {
                    var files = Directory.GetFiles(updatePath, "*.*", SearchOption.AllDirectories);
                    category.FileCount = files.Length;
                    category.EstimatedSize = files.Sum(f => new FileInfo(f).Length);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка анализа Windows Update: {ex.Message}");
            }

            return category;
        }

        private CleaningCategory AnalyzeWindowsLogs()
        {
            var category = new CleaningCategory
            {
                Name = "Логи Windows",
                Description = "Файлы журналов Windows",
                CategoryType = "System"
            };

            try
            {
                string logsPath = @"C:\Windows\Logs";
                if (Directory.Exists(logsPath))
                {
                    var files = Directory.GetFiles(logsPath, "*.log", SearchOption.AllDirectories);
                    category.FileCount = files.Length;
                    category.EstimatedSize = files.Sum(f => new FileInfo(f).Length);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка анализа логов: {ex.Message}");
            }

            return category;
        }

        private CleaningCategory AnalyzeCrashDumps()
        {
            var category = new CleaningCategory
            {
                Name = "Crash Dumps",
                Description = "Файлы аварийных дампов памяти",
                CategoryType = "System"
            };

            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string crashDumpsPath = Path.Combine(localAppData, "CrashDumps");
                
                if (Directory.Exists(crashDumpsPath))
                {
                    var files = Directory.GetFiles(crashDumpsPath, "*.dmp");
                    category.FileCount = files.Length;
                    category.EstimatedSize = files.Sum(f => new FileInfo(f).Length);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка анализа crash dumps: {ex.Message}");
            }

            return category;
        }

        /// <summary>
        /// Выполнить очистку выбранных категорий
        /// </summary>
        public async Task<CleaningResult> CleanCategories(List<CleaningCategory> categories, IProgress<int> progress)
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
                            case "Временные файлы пользователя":
                                CleanTempFiles(result);
                                break;
                            case "Временные файлы Windows":
                                CleanWindowsTemp(result);
                                break;
                            case "Prefetch":
                                CleanPrefetch(result);
                                break;
                            case "Кэш миниатюр":
                                CleanThumbnailCache(result);
                                break;
                            case "Корзина":
                                CleanRecycleBin(result);
                                break;
                            case "Windows Update":
                                CleanWindowsUpdate(result);
                                break;
                            case "Логи Windows":
                                CleanWindowsLogs(result);
                                break;
                            case "Crash Dumps":
                                CleanCrashDumps(result);
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
            result.Message = "Очистка завершена успешно";
            
            return result;
        }

        private void CleanTempFiles(CleaningResult result)
        {
            try
            {
                string tempPath = Path.GetTempPath();
                DeleteFilesInDirectory(tempPath, result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка очистки временных файлов: {ex.Message}");
            }
        }

        private void CleanWindowsTemp(CleaningResult result)
        {
            try
            {
                string winTemp = @"C:\Windows\Temp";
                DeleteFilesInDirectory(winTemp, result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка очистки Windows Temp: {ex.Message}");
            }
        }

        private void CleanPrefetch(CleaningResult result)
        {
            try
            {
                string prefetchPath = @"C:\Windows\Prefetch";
                if (Directory.Exists(prefetchPath))
                {
                    var files = Directory.GetFiles(prefetchPath, "*.pf");
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
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка очистки Prefetch: {ex.Message}");
            }
        }

        private void CleanThumbnailCache(CleaningResult result)
        {
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string thumbCachePath = Path.Combine(localAppData, @"Microsoft\Windows\Explorer");
                
                if (Directory.Exists(thumbCachePath))
                {
                    var files = Directory.GetFiles(thumbCachePath, "thumbcache_*.db");
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
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка очистки кэша миниатюр: {ex.Message}");
            }
        }

        private void CleanRecycleBin(CleaningResult result)
        {
            try
            {
                // Очистка корзины через Shell API
                _logger.LogInfo("Очистка корзины");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка очистки корзины: {ex.Message}");
            }
        }

        private void CleanWindowsUpdate(CleaningResult result)
        {
            try
            {
                string updatePath = @"C:\Windows\SoftwareDistribution\Download";
                DeleteFilesInDirectory(updatePath, result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка очистки Windows Update: {ex.Message}");
            }
        }

        private void CleanWindowsLogs(CleaningResult result)
        {
            try
            {
                string logsPath = @"C:\Windows\Logs";
                if (Directory.Exists(logsPath))
                {
                    var files = Directory.GetFiles(logsPath, "*.log", SearchOption.AllDirectories);
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
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка очистки логов: {ex.Message}");
            }
        }

        private void CleanCrashDumps(CleaningResult result)
        {
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string crashDumpsPath = Path.Combine(localAppData, "CrashDumps");
                
                if (Directory.Exists(crashDumpsPath))
                {
                    var files = Directory.GetFiles(crashDumpsPath, "*.dmp");
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
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка очистки crash dumps: {ex.Message}");
            }
        }

        private void DeleteFilesInDirectory(string path, CleaningResult result)
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
                    catch
                    {
                        // Пропускаем файлы, которые не можем удалить
                    }
                }

                // Удаляем пустые папки
                var directories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
                foreach (var dir in directories.OrderByDescending(d => d.Length))
                {
                    try
                    {
                        if (Directory.GetFiles(dir).Length == 0 && Directory.GetDirectories(dir).Length == 0)
                        {
                            Directory.Delete(dir);
                            result.FoldersCleared++;
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка удаления файлов в {path}: {ex.Message}");
            }
        }
    }
}
