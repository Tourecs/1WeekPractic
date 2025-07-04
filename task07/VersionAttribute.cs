using System;

namespace task07
{
    /// <summary>
    /// Атрибут для указания версии класса в формате Major.Minor
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, 
                    AllowMultiple = false, Inherited = true)]
    public class VersionAttribute : Attribute
    {
        /// <summary>
        /// Основная версия (Major)
        /// </summary>
        public int Major { get; }

        /// <summary>
        /// Минорная версия (Minor)
        /// </summary>
        public int Minor { get; }

        /// <summary>
        /// Полная версия в формате "Major.Minor"
        /// </summary>
        public string Version => $"{Major}.{Minor}";

        /// <summary>
        /// Конструктор атрибута Version
        /// </summary>
        /// <param name="major">Основная версия</param>
        /// <param name="minor">Минорная версия</param>
        public VersionAttribute(int major, int minor)
        {
            if (major < 0)
                throw new ArgumentException("Major version cannot be negative", nameof(major));
            if (minor < 0)
                throw new ArgumentException("Minor version cannot be negative", nameof(minor));

            Major = major;
            Minor = minor;
        }

        /// <summary>
        /// Конструктор атрибута Version из строки
        /// </summary>
        /// <param name="version">Версия в формате "Major.Minor"</param>
        public VersionAttribute(string version)
        {
            if (string.IsNullOrEmpty(version))
                throw new ArgumentException("Version string cannot be null or empty", nameof(version));

            var parts = version.Split('.');
            if (parts.Length != 2)
                throw new ArgumentException("Version must be in format 'Major.Minor'", nameof(version));

            if (!int.TryParse(parts[0], out int major) || !int.TryParse(parts[1], out int minor))
                throw new ArgumentException("Version parts must be valid integers", nameof(version));

            if (major < 0 || minor < 0)
                throw new ArgumentException("Version parts cannot be negative", nameof(version));

            Major = major;
            Minor = minor;
        }

        /// <summary>
        /// Строковое представление атрибута
        /// </summary>
        public override string ToString()
        {
            return $"Version: {Version}";
        }

        /// <summary>
        /// Проверка равенства атрибутов
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is VersionAttribute other)
            {
                return Major == other.Major && Minor == other.Minor;
            }
            return false;
        }

        /// <summary>
        /// Получение хэш-кода
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(Major, Minor);
        }

        /// <summary>
        /// Сравнение версий
        /// </summary>
        /// <param name="other">Другая версия для сравнения</param>
        /// <returns>-1 если меньше, 0 если равны, 1 если больше</returns>
        public int CompareTo(VersionAttribute other)
        {
            if (other == null) return 1;
            
            int majorComparison = Major.CompareTo(other.Major);
            if (majorComparison != 0) return majorComparison;
            
            return Minor.CompareTo(other.Minor);
        }
    }
}