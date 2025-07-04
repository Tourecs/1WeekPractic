using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace task07
{
    /// <summary>
    /// Статический класс для работы с рефлексией и анализа атрибутов
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Анализирует переданный тип и выводит информацию о его атрибутах
        /// </summary>
        /// <param name="type">Тип для анализа</param>
        public static void PrintTypeInfo(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var info = GetTypeInfo(type);
            Console.WriteLine(info);
        }

        /// <summary>
        /// Получает строковую информацию о типе и его атрибутах
        /// </summary>
        /// <param name="type">Тип для анализа</param>
        /// <returns>Строка с информацией о типе</returns>
        public static string GetTypeInfo(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var sb = new StringBuilder();
            
            // Заголовок
            sb.AppendLine("=".PadLeft(60, '='));
            sb.AppendLine($"АНАЛИЗ ТИПА: {type.Name}");
            sb.AppendLine("=".PadLeft(60, '='));

            // Информация о классе
            AnalyzeClassAttributes(type, sb);
            
            // Разделитель
            sb.AppendLine();
            
            // Анализ методов
            AnalyzeMethods(type, sb);
            
            // Разделитель
            sb.AppendLine();
            
            // Анализ свойств
            AnalyzeProperties(type, sb);
            
            // Разделитель
            sb.AppendLine();
            
            // Анализ полей
            AnalyzeFields(type, sb);

            sb.AppendLine("=".PadLeft(60, '='));

            return sb.ToString();
        }

        /// <summary>
        /// Анализирует атрибуты класса
        /// </summary>
        private static void AnalyzeClassAttributes(Type type, StringBuilder sb)
        {
            sb.AppendLine("📋 ИНФОРМАЦИЯ О КЛАССЕ:");
            
            // DisplayName атрибут
            var displayNameAttr = type.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttr != null)
            {
                sb.AppendLine($"   Отображаемое имя: {displayNameAttr.DisplayName}");
            }
            else
            {
                sb.AppendLine($"   Отображаемое имя: {type.Name} (атрибут не найден)");
            }

            // Version атрибут
            var versionAttr = type.GetCustomAttribute<VersionAttribute>();
            if (versionAttr != null)
            {
                sb.AppendLine($"   Версия: {versionAttr.Version} (Major: {versionAttr.Major}, Minor: {versionAttr.Minor})");
            }
            else
            {
                sb.AppendLine("   Версия: не указана");
            }

            // Дополнительная информация о типе
            sb.AppendLine($"   Полное имя: {type.FullName}");
            sb.AppendLine($"   Пространство имен: {type.Namespace}");
            sb.AppendLine($"   Является ли классом: {type.IsClass}");
            sb.AppendLine($"   Является ли абстрактным: {type.IsAbstract}");
            sb.AppendLine($"   Является ли публичным: {type.IsPublic}");
        }

        /// <summary>
        /// Анализирует методы типа
        /// </summary>
        private static void AnalyzeMethods(Type type, StringBuilder sb)
        {
            sb.AppendLine("🔧 МЕТОДЫ:");
            
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | 
                                        BindingFlags.Instance | BindingFlags.Static | 
                                        BindingFlags.DeclaredOnly)
                             .Where(m => !m.IsSpecialName) // Исключаем get/set методы свойств
                             .OrderBy(m => m.Name)
                             .ToArray();

            if (methods.Length == 0)
            {
                sb.AppendLine("   Методы не найдены");
                return;
            }

            foreach (var method in methods)
            {
                var displayNameAttr = method.GetCustomAttribute<DisplayNameAttribute>();
                var methodInfo = $"   {GetMethodSignature(method)}";
                
                if (displayNameAttr != null)
                {
                    methodInfo += $" → \"{displayNameAttr.DisplayName}\"";
                }

                sb.AppendLine(methodInfo);
            }
        }

        /// <summary>
        /// Анализирует свойства типа
        /// </summary>
        private static void AnalyzeProperties(Type type, StringBuilder sb)
        {
            sb.AppendLine("🏷️ СВОЙСТВА:");
            
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | 
                                              BindingFlags.Instance | BindingFlags.Static | 
                                              BindingFlags.DeclaredOnly)
                                .OrderBy(p => p.Name)
                                .ToArray();

            if (properties.Length == 0)
            {
                sb.AppendLine("   Свойства не найдены");
                return;
            }

            foreach (var property in properties)
            {
                var displayNameAttr = property.GetCustomAttribute<DisplayNameAttribute>();
                var accessors = GetPropertyAccessors(property);
                var propertyInfo = $"   {property.PropertyType.Name} {property.Name} {accessors}";
                
                if (displayNameAttr != null)
                {
                    propertyInfo += $" → \"{displayNameAttr.DisplayName}\"";
                }

                sb.AppendLine(propertyInfo);
            }
        }

        /// <summary>
        /// Анализирует поля типа
        /// </summary>
        private static void AnalyzeFields(Type type, StringBuilder sb)
        {
            sb.AppendLine("📦 ПОЛЯ:");
            
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | 
                                       BindingFlags.Instance | BindingFlags.Static | 
                                       BindingFlags.DeclaredOnly)
                            .OrderBy(f => f.Name)
                            .ToArray();

            if (fields.Length == 0)
            {
                sb.AppendLine("   Поля не найдены");
                return;
            }

            foreach (var field in fields)
            {
                var displayNameAttr = field.GetCustomAttribute<DisplayNameAttribute>();
                var visibility = GetFieldVisibility(field);
                var fieldInfo = $"   {visibility} {field.FieldType.Name} {field.Name}";
                
                if (displayNameAttr != null)
                {
                    fieldInfo += $" → \"{displayNameAttr.DisplayName}\"";
                }

                sb.AppendLine(fieldInfo);
            }
        }

        /// <summary>
        /// Получает сигнатуру метода
        /// </summary>
        private static string GetMethodSignature(MethodInfo method)
        {
            var visibility = method.IsPublic ? "public" : 
                           method.IsPrivate ? "private" : 
                           method.IsFamily ? "protected" : "internal";
            
            var isStatic = method.IsStatic ? "static " : "";
            var returnType = method.ReturnType.Name;
            var parameters = string.Join(", ", method.GetParameters()
                .Select(p => $"{p.ParameterType.Name} {p.Name}"));
            
            return $"{visibility} {isStatic}{returnType} {method.Name}({parameters})";
        }

        /// <summary>
        /// Получает информацию о методах доступа свойства
        /// </summary>
        private static string GetPropertyAccessors(PropertyInfo property)
        {
            var accessors = new List<string>();
            
            if (property.CanRead)
                accessors.Add("get");
            if (property.CanWrite)
                accessors.Add("set");
                
            return $"{{ {string.Join("; ", accessors)} }}";
        }

        /// <summary>
        /// Получает видимость поля
        /// </summary>
        private static string GetFieldVisibility(FieldInfo field)
        {
            var visibility = field.IsPublic ? "public" : 
                           field.IsPrivate ? "private" : 
                           field.IsFamily ? "protected" : "internal";
            
            var isStatic = field.IsStatic ? "static " : "";
            
            return $"{visibility} {isStatic}";
        }

        /// <summary>
        /// Получает все типы с указанным атрибутом из сборки
        /// </summary>
        /// <typeparam name="T">Тип атрибута</typeparam>
        /// <param name="assembly">Сборка для поиска</param>
        /// <returns>Коллекция типов с указанным атрибутом</returns>
        public static IEnumerable<Type> GetTypesWithAttribute<T>(Assembly? assembly = null) 
            where T : Attribute
        {
            // Если сборка не указана, используем сборку, содержащую ReflectionHelper
            assembly ??= typeof(ReflectionHelper).Assembly;
            
            return assembly.GetTypes()
                          .Where(type => type.GetCustomAttribute<T>() != null);
        }

        /// <summary>
        /// Получает значение атрибута для указанного типа
        /// </summary>
        /// <typeparam name="T">Тип атрибута</typeparam>
        /// <param name="type">Тип для анализа</param>
        /// <returns>Экземпляр атрибута или null</returns>
        public static T? GetAttributeValue<T>(Type type) where T : Attribute
        {
            return type.GetCustomAttribute<T>();
        }
    }
}