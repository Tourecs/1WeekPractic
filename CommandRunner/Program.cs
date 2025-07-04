using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLib;

namespace CommandRunner
{
    /// <summary>
    /// Консольное приложение для динамической загрузки и выполнения команд
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🚀 Command Runner - Динамическая загрузка команд");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine();

            try
            {
                // Загружаем команды из DLL
                var commands = LoadCommandsFromAssembly();
                
                if (commands.Count == 0)
                {
                    Console.WriteLine("❌ Команды не найдены!");
                    return;
                }

                Console.WriteLine($"✅ Загружено команд: {commands.Count}");
                Console.WriteLine();

                // Интерактивный режим
                RunInteractiveMode(commands);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Критическая ошибка: {ex.Message}");
                Console.WriteLine($"Детали: {ex}");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        /// <summary>
        /// Загружает команды из сборки FileSystemCommands
        /// </summary>
        /// <returns>Список доступных команд</returns>
        static List<ICommand> LoadCommandsFromAssembly()
        {
            var commands = new List<ICommand>();

            try
            {
                // Ищем DLL FileSystemCommands
                var assemblyPath = FindFileSystemCommandsAssembly();
                
                if (string.IsNullOrEmpty(assemblyPath))
                {
                    Console.WriteLine("❌ Не найдена сборка FileSystemCommands.dll");
                    return commands;
                }

                Console.WriteLine($"📦 Загрузка сборки: {assemblyPath}");
                
                // Загружаем сборку
                var assembly = Assembly.LoadFrom(assemblyPath);
                
                // Находим все типы, реализующие ICommand
                var commandTypes = assembly.GetTypes()
                    .Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                    .ToList();

                Console.WriteLine($"🔍 Найдено типов команд: {commandTypes.Count}");

                // Создаем демонстрационные экземпляры команд
                commands.AddRange(CreateDemoCommands(commandTypes));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка загрузки сборки: {ex.Message}");
            }

            return commands;
        }

        /// <summary>
        /// Ищет сборку FileSystemCommands.dll
        /// </summary>
        /// <returns>Путь к сборке или null</returns>
        static string? FindFileSystemCommandsAssembly()
        {
            var currentDir = Directory.GetCurrentDirectory();
            var possiblePaths = new[]
            {
                Path.Combine(currentDir, "FileSystemCommands.dll"),
                Path.Combine(currentDir, "bin", "Debug", "net9.0", "FileSystemCommands.dll"),
                Path.Combine(currentDir, "bin", "Release", "net9.0", "FileSystemCommands.dll"),
                Path.Combine(Directory.GetParent(currentDir)?.FullName ?? "", "FileSystemCommands", "bin", "Debug", "net9.0", "FileSystemCommands.dll"),
                Path.Combine(Directory.GetParent(currentDir)?.FullName ?? "", "FileSystemCommands", "bin", "Release", "net9.0", "FileSystemCommands.dll")
            };

            return possiblePaths.FirstOrDefault(File.Exists);
        }

        /// <summary>
        /// Создает демонстрационные экземпляры команд
        /// </summary>
        /// <param name="commandTypes">Типы команд</param>
        /// <returns>Список команд</returns>
        static List<ICommand> CreateDemoCommands(List<Type> commandTypes)
        {
            var commands = new List<ICommand>();

            foreach (var type in commandTypes)
            {
                try
                {
                    ICommand? command = null;

                    if (type.Name == "DirectorySizeCommand")
                    {
                        // Создаем команду для текущего каталога
                        command = (ICommand?)Activator.CreateInstance(type, Directory.GetCurrentDirectory());
                    }
                    else if (type.Name == "FindFilesCommand")
                    {
                        // Создаем команду для поиска .cs файлов
                        command = (ICommand?)Activator.CreateInstance(type, Directory.GetCurrentDirectory(), "*.cs", false);
                    }

                    if (command != null)
                    {
                        commands.Add(command);
                        Console.WriteLine($"✅ Создана команда: {command.Name} - {command.Description}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Ошибка создания команды {type.Name}: {ex.Message}");
                }
            }

            return commands;
        }

        /// <summary>
        /// Запускает интерактивный режим выполнения команд
        /// </summary>
        /// <param name="commands">Список доступных команд</param>
        static void RunInteractiveMode(List<ICommand> commands)
        {
            while (true)
            {
                Console.WriteLine("\n" + new string('=', 50));
                Console.WriteLine("📋 Доступные команды:");
                
                for (int i = 0; i < commands.Count; i++)
                {
                    Console.WriteLine($"  {i + 1}. {commands[i].Name} - {commands[i].Description}");
                }
                
                Console.WriteLine("  0. Выход");
                Console.WriteLine("  c. Создать пользовательскую команду");
                Console.WriteLine();
                Console.Write("Выберите команду (0-{0}, c): ", commands.Count);

                var input = Console.ReadLine()?.Trim().ToLower();

                if (input == "0")
                {
                    Console.WriteLine("👋 До свидания!");
                    break;
                }
                else if (input == "c")
                {
                    var customCommand = CreateCustomCommand();
                    if (customCommand != null)
                    {
                        commands.Add(customCommand);
                        Console.WriteLine("✅ Пользовательская команда добавлена!");
                    }
                }
                else if (int.TryParse(input, out int choice) && choice >= 1 && choice <= commands.Count)
                {
                    var selectedCommand = commands[choice - 1];
                    
                    Console.WriteLine($"\n🚀 Выполнение команды: {selectedCommand.Name}");
                    Console.WriteLine(new string('-', 40));
                    
                    var startTime = DateTime.Now;
                    selectedCommand.Execute();
                    var endTime = DateTime.Now;
                    
                    Console.WriteLine(new string('-', 40));
                    Console.WriteLine($"⏱️  Время выполнения: {(endTime - startTime).TotalMilliseconds:F2} мс");
                }
                else
                {
                    Console.WriteLine("❌ Неверный выбор!");
                }
            }
        }

        /// <summary>
        /// Создает пользовательскую команду по запросу
        /// </summary>
        /// <returns>Новая команда или null</returns>
        static ICommand? CreateCustomCommand()
        {
            Console.WriteLine("\n📝 Создание пользовательской команды:");
            Console.WriteLine("1. DirectorySizeCommand - вычислить размер каталога");
            Console.WriteLine("2. FindFilesCommand - найти файлы по маске");
            Console.Write("Выберите тип команды (1-2): ");

            var choice = Console.ReadLine()?.Trim();

            try
            {
                var assemblyPath = FindFileSystemCommandsAssembly();
                if (string.IsNullOrEmpty(assemblyPath))
                {
                    Console.WriteLine("❌ Сборка FileSystemCommands не найдена!");
                    return null;
                }

                if (choice == "1")
                {
                    Console.Write("Введите путь к каталогу: ");
                    var path = Console.ReadLine()?.Trim();
                    
                    if (!string.IsNullOrEmpty(path))
                    {
                        // Используем рефлексию для создания команды
                        var assembly = Assembly.LoadFrom(assemblyPath);
                        var type = assembly.GetType("FileSystemCommands.DirectorySizeCommand");
                        if (type != null)
                        {
                            return (ICommand?)Activator.CreateInstance(type, path);
                        }
                    }
                }
                else if (choice == "2")
                {
                    Console.Write("Введите путь к каталогу: ");
                    var path = Console.ReadLine()?.Trim();
                    Console.Write("Введите маску поиска (например, *.txt): ");
                    var pattern = Console.ReadLine()?.Trim();
                    Console.Write("Искать в подкаталогах? (y/n): ");
                    var recursive = Console.ReadLine()?.Trim().ToLower() == "y";
                    
                    if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(pattern))
                    {
                        var assembly = Assembly.LoadFrom(assemblyPath);
                        var type = assembly.GetType("FileSystemCommands.FindFilesCommand");
                        if (type != null)
                        {
                            return (ICommand?)Activator.CreateInstance(type, path, pattern, recursive);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка создания команды: {ex.Message}");
            }

            return null;
        }
    }
}