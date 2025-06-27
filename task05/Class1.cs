using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace task05
{
    /// <summary>
    /// Анализатор классов с использованием рефлексии
    /// </summary>
    public class ClassAnalyzer
    {
        private readonly Type _type;

        /// <summary>
        /// Конструктор анализатора
        /// </summary>
        /// <param name="type">Тип класса для анализа</param>
        public ClassAnalyzer(Type type)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
        }

        /// <summary>
        /// Получает список имен публичных методов класса
        /// </summary>
        /// <returns>Коллекция имен публичных методов</returns>
        public IEnumerable<string> GetPublicMethods()
        {
            return _type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName) // Исключаем get/set методы свойств
                .Select(m => m.Name);
        }

        /// <summary>
        /// Получает список имен параметров указанного публичного метода
        /// </summary>
        /// <param name="methodName">Имя метода</param>
        /// <returns>Коллекция имен параметров метода</returns>
        public IEnumerable<string> GetMethodParams(string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
                return Enumerable.Empty<string>();

            var method = _type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            
            return method?.GetParameters()
                .Select(p => p.Name) ?? Enumerable.Empty<string>();
        }

        /// <summary>
        /// Получает список имен всех полей класса (включая приватные)
        /// </summary>
        /// <returns>Коллекция имен полей</returns>
        public IEnumerable<string> GetAllFields()
        {
            return _type
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(f => f.Name);
        }

        /// <summary>
        /// Получает список имен всех свойств класса
        /// </summary>
        /// <returns>Коллекция имен свойств</returns>
        public IEnumerable<string> GetProperties()
        {
            return _type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(p => p.Name);
        }

        /// <summary>
        /// Проверяет наличие атрибута указанного типа у класса
        /// </summary>
        /// <typeparam name="T">Тип атрибута</typeparam>
        /// <returns>True, если атрибут присутствует, иначе false</returns>
        public bool HasAttribute<T>() where T : Attribute
        {
            return _type.GetCustomAttributes<T>().Any();
        }

        /// <summary>
        /// Получает информацию о типе анализируемого класса
        /// </summary>
        public Type AnalyzedType => _type;

        /// <summary>
        /// Получает полное имя анализируемого класса
        /// </summary>
        public string ClassName => _type.Name;

        /// <summary>
        /// Получает пространство имен анализируемого класса
        /// </summary>
        public string ClassNamespace => _type.Namespace;
    }
}