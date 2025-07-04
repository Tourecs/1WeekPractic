using System;

namespace task07
{
    /// <summary>
    /// Атрибут для указания отображаемого имени класса, метода или свойства
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Constructor, 
                    AllowMultiple = false, Inherited = true)]
    public class DisplayNameAttribute : Attribute
    {
        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Конструктор атрибута DisplayName
        /// </summary>
        /// <param name="displayName">Отображаемое имя</param>
        public DisplayNameAttribute(string displayName)
        {
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }

        /// <summary>
        /// Строковое представление атрибута
        /// </summary>
        public override string ToString()
        {
            return $"DisplayName: {DisplayName}";
        }

        /// <summary>
        /// Проверка равенства атрибутов
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is DisplayNameAttribute other)
            {
                return DisplayName == other.DisplayName;
            }
            return false;
        }

        /// <summary>
        /// Получение хэш-кода
        /// </summary>
        public override int GetHashCode()
        {
            return DisplayName?.GetHashCode() ?? 0;
        }
    }
}