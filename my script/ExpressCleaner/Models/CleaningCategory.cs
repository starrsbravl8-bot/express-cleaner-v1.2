namespace ExpressCleaner.Models
{
    /// <summary>
    /// Категория очистки
    /// </summary>
    public class CleaningCategory
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
        public long EstimatedSize { get; set; } = 0;
        public int FileCount { get; set; } = 0;
        public string CategoryType { get; set; } = string.Empty; // System, Browser, Temp
    }
}
