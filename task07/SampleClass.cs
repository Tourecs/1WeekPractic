using System;
using System.Reflection;

namespace task07
{
    /// <summary>
    /// Пример класса с пользовательскими атрибутами
    /// </summary>
    [DisplayName("Пример класса")]
    [Version(1, 0)]
    public class SampleClass
    {
        private string _name = string.Empty;

        /// <summary>
        /// Числовое свойство с атрибутом DisplayName
        /// </summary>
        [DisplayName("Числовое свойство")]
        public int Number { get; set; }

        /// <summary>
        /// Строковое свойство с атрибутом DisplayName
        /// </summary>
        [DisplayName("Имя объекта")]
        public string Name 
        { 
            get => _name; 
            set => _name = value ?? string.Empty; 
        }

        /// <summary>
        /// Свойство без атрибута для тестирования
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Публичное поле с атрибутом
        /// </summary>
        [DisplayName("Публичное поле")]
        public bool IsActive = true;

        /// <summary>
        /// Приватное поле с атрибутом
        /// </summary>
        [DisplayName("Приватное поле")]
        private int _internalId = 42;

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public SampleClass()
        {
            Number = 0;
            Name = "Default";
        }

        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="number">Числовое значение</param>
        /// <param name="name">Имя</param>
        public SampleClass(int number, string name)
        {
            Number = number;
            Name = name;
        }

        /// <summary>
        /// Тестовый метод с атрибутом DisplayName
        /// </summary>
        [DisplayName("Тестовый метод")]
        public void TestMethod()
        {
            Console.WriteLine($"Выполняется тестовый метод. Number: {Number}, Name: {Name}");
        }

        /// <summary>
        /// Метод с параметрами и возвращаемым значением
        /// </summary>
        /// <param name="multiplier">Множитель</param>
        /// <returns>Результат умножения</returns>
        [DisplayName("Метод вычисления")]
        public int Calculate(int multiplier)
        {
            return Number * multiplier;
        }

        /// <summary>
        /// Метод без атрибута для тестирования
        /// </summary>
        public string GetInfo()
        {
            return $"SampleClass: {Name} (Number: {Number}, Created: {CreatedDate:yyyy-MM-dd})";
        }

        /// <summary>
        /// Приватный метод с атрибутом
        /// </summary>
        [DisplayName("Приватный метод")]
        private void PrivateMethod()
        {
            _internalId++;
        }

        /// <summary>
        /// Статический метод с атрибутом
        /// </summary>
        [DisplayName("Статический метод")]
        public static string GetVersion()
        {
            var versionAttr = typeof(SampleClass).GetCustomAttribute<VersionAttribute>();
            return versionAttr?.Version ?? "Unknown";
        }

        /// <summary>
        /// Переопределение ToString
        /// </summary>
        public override string ToString()
        {
            return GetInfo();
        }
    }
}