using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLib;

namespace FileSystemCommands
{
    /// <summary>
    /// Команда для поиска файлов по маске
    /// </summary>
    public class FindFilesCommand : ICommand
    {
        private readonly string _directoryPath;
        private readonly string _searchPattern;
        private readonly bool _recursive;
        
        /// <summary>
        /// Результат последнего поиска
        /// </summary>
        public List<string> LastFoundFiles { get; private set; } = new List<string>();

        /// <summary>
        /// Имя команды
        /// </summary>
        public string Name => "FindFiles";

        /// <summary>
        /// Описание команды
        /// </summary>
        public string Description => $"Поиск файлов по маске '{_searchPattern}' в каталоге: {_directoryPath}";

        /// <summary>
        /// Конструктор команды
        /// </summary>
        /// <param name="directoryPath">Путь к каталогу для поиска</param>
        /// <param name="searchPattern">Маска поиска (например, *.txt)</param>
        /// <param name="recursive">Искать в подкаталогах</param>
        public FindFilesCommand(string directoryPath, string searchPattern, bool recursive = false)
        {
            _directoryPath = directoryPath ?? throw new ArgumentNullException(nameof(directoryPath));
            _searchPattern = searchPattern ?? throw new ArgumentNullException(nameof(searchPattern));
            _recursive = recursive;
        }

        /// <summary>
        /// Выполняет поиск файлов
        /// </summary>
        public void Execute()
        {
            try
            {
                if (!Directory.Exists(_directoryPath))
                {
                    Console.WriteLine($"❌ Каталог не найден: {_directoryPath}");
                    LastFoundFiles.Clear();
                    return;
                }

                Console.WriteLine($"🔍 Поиск файлов по маске '{_searchPattern}' в каталоге: {_directoryPath}");
                if (_recursive)
                {
                    Console.WriteLine("📁 Поиск включает подкаталоги");
                }

                var searchOption = _recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                var foundFiles = Directory.GetFiles(_directoryPath, _searchPattern, searchOption).ToList();
                
                LastFoundFiles = foundFiles;

                Console.WriteLine($"📊 Найдено файлов: {foundFiles.Count}");
                
                if (foundFiles.Count > 0)
                {
                    Console.WriteLine("📄 Найденные файлы:");
                    for (int i = 0; i < foundFiles.Count; i++)
                    {
                        var file = foundFiles[i];
                        var fileInfo = new FileInfo(file);
                        Console.WriteLine($"   {i + 1}. {file}");
                        Console.WriteLine($"      Размер: {FormatBytes(fileInfo.Length)}, Изменен: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}");
                    }
                }
                else
                {
                    Console.WriteLine("   Файлы не найдены");
                }

                // Дополнительная статистика
                if (foundFiles.Count > 0)
                {
                    var totalSize = foundFiles.Sum(f => new FileInfo(f).Length);
                    Console.WriteLine($"📈 Общий размер найденных файлов: {FormatBytes(totalSize)}");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"❌ Нет доступа к каталогу: {ex.Message}");
                LastFoundFiles.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка при поиске файлов: {ex.Message}");
                LastFoundFiles.Clear();
            }
        }

        /// <summary>
        /// Форматирует размер в человекочитаемый вид
        /// </summary>
        /// <param name="bytes">Размер в байтах</param>
        /// <returns>Отформатированная строка</returns>
        private static string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int suffixIndex = 0;
            double size = bytes;

            while (size >= 1024 && suffixIndex < suffixes.Length - 1)
            {
                size /= 1024;
                suffixIndex++;
            }

            return $"{size:F2} {suffixes[suffixIndex]}";
        }

        /// <summary>
        /// Получает детальную информацию о найденных файлах
        /// </summary>
        /// <returns>Список информации о файлах</returns>
        public List<FileInfo> GetFoundFilesInfo()
        {
            return LastFoundFiles.Select(f => new FileInfo(f)).ToList();
        }

        /// <summary>
        /// Фильтрует найденные файлы по размеру
        /// </summary>
        /// <param name="minSize">Минимальный размер в байтах</param>
        /// <param name="maxSize">Максимальный размер в байтах</param>
        /// <returns>Отфильтрованный список файлов</returns>
        public List<string> FilterBySize(long minSize = 0, long maxSize = long.MaxValue)
        {
            return LastFoundFiles
                .Where(f =>
                {
                    var size = new FileInfo(f).Length;
                    return size >= minSize && size <= maxSize;
                })
                .ToList();
        }

        /// <summary>
        /// Группирует найденные файлы по расширению
        /// </summary>
        /// <returns>Словарь: расширение -> список файлов</returns>
        public Dictionary<string, List<string>> GroupByExtension()
        {
            return LastFoundFiles
                .GroupBy(f => Path.GetExtension(f).ToLowerInvariant())
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}