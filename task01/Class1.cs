using System;
using System.Linq;

namespace task01
{
    public static class StringExtensions
    {
        /// <summary>
        /// Проверяет, является ли строка палиндромом.
        /// Игнорирует регистр символов, пробелы и знаки препинания.
        /// </summary>
        /// <param name="input">Входная строка для проверки</param>
        /// <returns>true, если строка является палиндромом; иначе false</returns>
        public static bool IsPalindrome(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return true;
            
            // Приводим строку к нижнему регистру
            string lowerInput = input.ToLower();
            
            // Удаляем все пробелы и знаки препинания
            var cleanedString = new string(lowerInput
                .Where(c => !char.IsPunctuation(c) && !char.IsWhiteSpace(c))
                .ToArray());
            
            // Сравниваем строку с её перевёрнутой версией
            var reversed = new string(cleanedString.Reverse().ToArray());
            
            return cleanedString.Equals(reversed);
        }
    }
}