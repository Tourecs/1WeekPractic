using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace task03
{
    /// <summary>
    /// Пользовательская коллекция с поддержкой итераторов и LINQ операций
    /// </summary>
    /// <typeparam name="T">Тип элементов коллекции</typeparam>
    public class CustomCollection<T> : IEnumerable<T>
    {
        private readonly List<T> _items = new();

        /// <summary>
        /// Добавляет элемент в коллекцию
        /// </summary>
        /// <param name="item">Элемент для добавления</param>
        public void Add(T item) => _items.Add(item);

        /// <summary>
        /// Возвращает количество элементов в коллекции
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Возвращает стандартный итератор коллекции
        /// </summary>
        /// <returns>Итератор для прохода по элементам</returns>
        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        /// <summary>
        /// Возвращает нетипизированный итератор коллекции
        /// </summary>
        /// <returns>Нетипизированный итератор</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Возвращает итератор для обратного обхода коллекции
        /// </summary>
        /// <returns>Элементы в обратном порядке</returns>
        public IEnumerable<T> GetReverseEnumerator()
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                yield return _items[i];
            }
        }

        /// <summary>
        /// Генерирует числовую последовательность начиная с указанного значения
        /// </summary>
        /// <param name="start">Начальное значение</param>
        /// <param name="count">Количество элементов</param>
        /// <returns>Последовательность чисел</returns>
        public static IEnumerable<int> GenerateSequence(int start, int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return start + i;
            }
        }

        /// <summary>
        /// Фильтрует и сортирует коллекцию
        /// </summary>
        /// <param name="predicate">Условие фильтрации</param>
        /// <param name="keySelector">Селектор ключа для сортировки</param>
        /// <returns>Отфильтрованная и отсортированная последовательность</returns>
        public IEnumerable<T> FilterAndSort<TKey>(Func<T, bool> predicate, Func<T, TKey> keySelector) 
            where TKey : IComparable<TKey>
        {
            return _items
                .Where(predicate)
                .OrderBy(keySelector);
        }
    }
}