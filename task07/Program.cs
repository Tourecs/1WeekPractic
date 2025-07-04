using System;
using task07;

namespace task07
{
    /// <summary>
    /// Демонстрационная программа для показа работы с пользовательскими атрибутами
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🚀 Демонстрация работы с пользовательскими атрибутами");
            Console.WriteLine();

            // Демонстрация работы ReflectionHelper
            try
            {
                Console.WriteLine("📋 Анализ SampleClass:");
                ReflectionHelper.PrintTypeInfo(typeof(SampleClass));
                
                Console.WriteLine();
                Console.WriteLine("📋 Анализ других типов:");
                
                // Анализ типа string (без наших атрибутов)
                Console.WriteLine("\n--- Анализ встроенного типа String ---");
                ReflectionHelper.PrintTypeInfo(typeof(string));
                
                Console.WriteLine();
                Console.WriteLine("✅ Демонстрация завершена успешно!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}