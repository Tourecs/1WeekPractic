using System;
using System.IO;
using CommandLib;

namespace FileSystemCommands
{
    /// <summary>
    /// Команда для вычисления размера каталога
    /// </summary>
    public class DirectorySizeCommand : ICommand
    {
        private readonly string _directoryPath;
        
        /// <summary>
        /// Результат последнего выполнения команды
        /// </summary>
        public long LastCalculatedSize { get; private set; }

        /// <summary>
        /// Имя команды
        /// </summary>
        public string Name => "DirectorySize";

        /// <summary>
        /// Описание команды
        /// </summary>
        public string Description => $"Вычисляет размер каталога: {_directoryPath}";

        /// <summary>
        /// Конструктор команды
        /// </summary>
        /// <param name="directoryPath">Путь к каталогу</param>
        public DirectorySizeCommand(string directoryPath)
        {
            _directoryPath = directoryPath ?? throw new ArgumentNullException(nameof(directoryPath));
        }

        /// <summary>
        /// Выполняет вычисление размера каталога
        /// </summary>
        public void Execute()
        {
            try
            {
                if (!Directory.Exists(_directoryPath))
                {
                    Console.WriteLine($"❌ Каталог не найден: {_directoryPath}");
                    LastCalculatedSize = 0;
                    return;
                }

                Console.WriteLine($"📊 Вычисление размера каталога: {_directoryPath}");
                
                long totalSize = CalculateDirectorySize(_directoryPath);
                LastCalculatedSize = totalSize;

                Console.WriteLine($"📁 Общий размер каталога: {FormatBytes(totalSize)}");
                Console.WriteLine($"📈 Размер в байтах: {totalSize:N0}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"❌ Нет доступа к каталогу: {ex.Message}");
                LastCalculatedSize = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка при вычислении размера: {ex.Message}");
                LastCalculatedSize = 0;
            }
        }

        /// <summary>
        /// Рекурсивно вычисляет размер каталога
        /// </summary>
        /// <param name="directoryPath">Путь к каталогу</param>
        /// <returns>Размер в байтах</returns>
        private long CalculateDirectorySize(string directoryPath)
        {
            long size = 0;

            try
            {
                // Размер всех файлов в текущем каталоге
                var files = Directory.GetFiles(directoryPath);
                foreach (var file in files)
                {
                    try
                    {
                        var fileInfo = new FileInfo(file);
                        size += fileInfo.Length;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Пропускаем файлы без доступа
                        Console.WriteLine($"⚠️  Пропущен файл (нет доступа): {file}");
                    }
                }

                // Рекурсивно для всех подкаталогов
                var directories = Directory.GetDirectories(directoryPath);
                foreach (var directory in directories)
                {
                    try
                    {
                        size += CalculateDirectorySize(directory);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Пропускаем каталоги без доступа
                        Console.WriteLine($"⚠️  Пропущен каталог (нет доступа): {directory}");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Нет доступа к каталогу
                throw;
            }

            return size;
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
        /// Получает детальную информацию о каталоге
        /// </summary>
        /// <returns>Информация о каталоге</returns>
        public DirectoryInfo GetDirectoryInfo()
        {
            if (!Directory.Exists(_directoryPath))
                throw new DirectoryNotFoundException($"Каталог не найден: {_directoryPath}");

            return new DirectoryInfo(_directoryPath);
        }
    }
}