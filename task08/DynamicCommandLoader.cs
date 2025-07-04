using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLib;

namespace task08
{
    /// <summary>
    /// Утилита для динамической загрузки команд из сборок
    /// </summary>
    public class DynamicCommandLoader
    {
        /// <summary>
        /// Загружает все команды из указанной сборки
        /// </summary>
        /// <param name="assemblyPath">Путь к сборке</param>
        /// <returns>Список найденных типов команд</returns>
        public static List<Type> LoadCommandTypesFromAssembly(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath))
                throw new ArgumentException("Assembly path cannot be null or empty", nameof(assemblyPath));

            if (!File.Exists(assemblyPath))
                throw new FileNotFoundException($"Assembly not found: {assemblyPath}");

            try
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                
                return assembly.GetTypes()
                    .Where(t => typeof(ICommand).IsAssignableFrom(t) && 
                               !t.IsInterface && 
                               !t.IsAbstract)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load commands from assembly: {assemblyPath}", ex);
            }
        }

        /// <summary>
        /// Создает экземпляр команды с помощью рефлексии
        /// </summary>
        /// <param name="commandType">Тип команды</param>
        /// <param name="parameters">Параметры конструктора</param>
        /// <returns>Экземпляр команды</returns>
        public static ICommand CreateCommandInstance(Type commandType, params object[] parameters)
        {
            if (commandType == null)
                throw new ArgumentNullException(nameof(commandType));

            if (!typeof(ICommand).IsAssignableFrom(commandType))
                throw new ArgumentException($"Type {commandType.Name} does not implement ICommand", nameof(commandType));

            try
            {
                return (ICommand)Activator.CreateInstance(commandType, parameters);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create instance of {commandType.Name}", ex);
            }
        }

        /// <summary>
        /// Ищет сборки с командами в указанной папке
        /// </summary>
        /// <param name="searchPath">Путь для поиска</param>
        /// <param name="searchPattern">Маска поиска DLL файлов</param>
        /// <returns>Список путей к найденным сборкам</returns>
        public static List<string> FindCommandAssemblies(string searchPath, string searchPattern = "*.dll")
        {
            if (string.IsNullOrEmpty(searchPath))
                throw new ArgumentException("Search path cannot be null or empty", nameof(searchPath));

            if (!Directory.Exists(searchPath))
                return new List<string>();

            var assemblies = new List<string>();

            try
            {
                var dllFiles = Directory.GetFiles(searchPath, searchPattern, SearchOption.AllDirectories);
                
                foreach (var dllFile in dllFiles)
                {
                    try
                    {
                        // Проверяем, содержит ли сборка команды
                        var assembly = Assembly.LoadFrom(dllFile);
                        var hasCommands = assembly.GetTypes()
                            .Any(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                        if (hasCommands)
                        {
                            assemblies.Add(dllFile);
                        }
                    }
                    catch
                    {
                        // Пропускаем некорректные сборки
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to search for command assemblies in: {searchPath}", ex);
            }

            return assemblies;
        }

        /// <summary>
        /// Получает информацию о командах в сборке без их загрузки
        /// </summary>
        /// <param name="assemblyPath">Путь к сборке</param>
        /// <returns>Информация о командах</returns>
        public static List<CommandInfo> GetCommandInfo(string assemblyPath)
        {
            var commandTypes = LoadCommandTypesFromAssembly(assemblyPath);
            var commandInfos = new List<CommandInfo>();

            foreach (var type in commandTypes)
            {
                commandInfos.Add(new CommandInfo
                {
                    TypeName = type.Name,
                    FullTypeName = type.FullName,
                    AssemblyPath = assemblyPath,
                    Constructors = type.GetConstructors()
                        .Select(c => c.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}").ToArray())
                        .ToList()
                });
            }

            return commandInfos;
        }
    }

    /// <summary>
    /// Информация о команде
    /// </summary>
    public class CommandInfo
    {
        public string TypeName { get; set; } = string.Empty;
        public string FullTypeName { get; set; } = string.Empty;
        public string AssemblyPath { get; set; } = string.Empty;
        public List<string[]> Constructors { get; set; } = new List<string[]>();

        public override string ToString()
        {
            return $"{TypeName} ({Constructors.Count} constructors)";
        }
    }
}