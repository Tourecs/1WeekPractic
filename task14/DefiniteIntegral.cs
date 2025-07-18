using System;
using System.Threading;

namespace Task14
{
    /// <summary>
    /// Класс для вычисления определенного интеграла методом трапеции в несколько потоков
    /// </summary>
    public class DefiniteIntegral
    {
        /// <summary>
        /// Вычисляет определенный интеграл методом трапеции с использованием нескольких потоков
        /// </summary>
        /// <param name="a">Левая граница интегрирования</param>
        /// <param name="b">Правая граница интегрирования</param>
        /// <param name="function">Функция для интегрирования</param>
        /// <param name="step">Размер шага разбиения</param>
        /// <param name="threadsNumber">Количество потоков для вычислений</param>
        /// <returns>Значение определенного интеграла</returns>
        public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
        {
            if (function == null)
                throw new ArgumentNullException(nameof(function));
            
            if (step <= 0)
                throw new ArgumentException("Step must be positive", nameof(step));
            
            if (threadsNumber <= 0)
                throw new ArgumentException("Threads number must be positive", nameof(threadsNumber));
            
            if (a >= b)
                return 0.0;

            // Разделяемая переменная для накопления результата
            double totalResult = 0.0;
            
            // Барьер для синхронизации потоков
            using var barrier = new Barrier(threadsNumber + 1); // +1 для основного потока
            
            // Длина всего интервала
            double totalLength = b - a;
            
            // Длина интервала для каждого потока
            double segmentLength = totalLength / threadsNumber;
            
            // Массив потоков
            var threads = new Thread[threadsNumber];
            
            // Создание и запуск потоков
            for (int i = 0; i < threadsNumber; i++)
            {
                int threadIndex = i;
                threads[i] = new Thread(() =>
                {
                    // Границы сегмента для текущего потока
                    double segmentStart = a + threadIndex * segmentLength;
                    double segmentEnd = threadIndex == threadsNumber - 1 ? b : a + (threadIndex + 1) * segmentLength;
                    
                    // Вычисление интеграла на сегменте методом трапеции
                    double segmentResult = CalculateSegmentIntegral(segmentStart, segmentEnd, function, step);
                    
                    // Атомарное добавление результата к общей сумме
                    AddToTotal(ref totalResult, segmentResult);
                    
                    // Ожидание на барьере
                    barrier.SignalAndWait();
                });
                
                threads[i].Start();
            }
            
            // Основной поток ожидает завершения всех вычислительных потоков
            barrier.SignalAndWait();
            
            // Ожидание завершения всех потоков
            foreach (var thread in threads)
            {
                thread.Join();
            }
            
            return totalResult;
        }
        
        /// <summary>
        /// Вычисляет интеграл на заданном сегменте методом трапеции
        /// </summary>
        /// <param name="start">Начало сегмента</param>
        /// <param name="end">Конец сегмента</param>
        /// <param name="function">Функция для интегрирования</param>
        /// <param name="step">Размер шага</param>
        /// <returns>Значение интеграла на сегменте</returns>
        private static double CalculateSegmentIntegral(double start, double end, Func<double, double> function, double step)
        {
            double result = 0.0;
            double x = start;
            
            while (x < end)
            {
                double nextX = Math.Min(x + step, end);
                
                // Формула трапеции: (f(x) + f(x+h)) * h / 2
                double trapezoidArea = (function(x) + function(nextX)) * (nextX - x) / 2.0;
                result += trapezoidArea;
                
                x = nextX;
            }
            
            return result;
        }
        
        /// <summary>
        /// Атомарно добавляет значение к общей сумме
        /// </summary>
        /// <param name="total">Ссылка на общую сумму</param>
        /// <param name="value">Значение для добавления</param>
        private static void AddToTotal(ref double total, double value)
        {
            double currentTotal, newTotal;
            do
            {
                currentTotal = total;
                newTotal = currentTotal + value;
            } while (Interlocked.CompareExchange(ref total, newTotal, currentTotal) != currentTotal);
        }
    }
}