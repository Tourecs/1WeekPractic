using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using task03;

namespace task03tests
{
    public class IteratorTests
    {
        [Fact]
        public void CustomCollection_GetEnumerator_ReturnsAllItems()
        {
            // Arrange
            var collection = new CustomCollection<int>();
            collection.Add(1);
            collection.Add(2);

            // Act
            var result = new List<int>();
            foreach (var item in collection)
            {
                result.Add(item);
            }

            // Assert
            Assert.Equal(new[] { 1, 2 }, result);
        }

        [Fact]
        public void GetReverseEnumerator_ReturnsItemsInReverseOrder()
        {
            // Arrange
            var collection = new CustomCollection<int>();
            collection.Add(1);
            collection.Add(2);
            collection.Add(3);

            // Act
            var result = collection.GetReverseEnumerator().ToList();

            // Assert
            Assert.Equal(new[] { 3, 2, 1 }, result);
        }

        [Fact]
        public void GetReverseEnumerator_EmptyCollection_ReturnsEmpty()
        {
            // Arrange
            var collection = new CustomCollection<int>();

            // Act
            var result = collection.GetReverseEnumerator().ToList();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GenerateSequence_ReturnsCorrectSequence()
        {
            // Act
            var sequence = CustomCollection<int>.GenerateSequence(5, 3).ToList();

            // Assert
            Assert.Equal(new[] { 5, 6, 7 }, sequence);
        }

        [Fact]
        public void GenerateSequence_ZeroCount_ReturnsEmpty()
        {
            // Act
            var sequence = CustomCollection<int>.GenerateSequence(10, 0).ToList();

            // Assert
            Assert.Empty(sequence);
        }

        [Fact]
        public void GenerateSequence_NegativeStart_ReturnsCorrectSequence()
        {
            // Act
            var sequence = CustomCollection<int>.GenerateSequence(-2, 4).ToList();

            // Assert
            Assert.Equal(new[] { -2, -1, 0, 1 }, sequence);
        }

        [Fact]
        public void FilterAndSort_ReturnsFilteredAndSortedItems()
        {
            // Arrange
            var collection = new CustomCollection<int>();
            collection.Add(3);
            collection.Add(1);
            collection.Add(2);
            collection.Add(4);

            // Act
            var result = collection.FilterAndSort(x => x > 1, x => x).ToList();

            // Assert
            Assert.Equal(new[] { 2, 3, 4 }, result);
        }

        [Fact]
        public void FilterAndSort_NoMatchingItems_ReturnsEmpty()
        {
            // Arrange
            var collection = new CustomCollection<int>();
            collection.Add(1);
            collection.Add(2);
            collection.Add(3);

            // Act
            var result = collection.FilterAndSort(x => x > 10, x => x).ToList();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void FilterAndSort_StringCollection_WorksCorrectly()
        {
            // Arrange
            var collection = new CustomCollection<string>();
            collection.Add("apple");
            collection.Add("banana");
            collection.Add("cherry");
            collection.Add("date");

            // Act
            var result = collection.FilterAndSort(
                s => s.Length > 4, 
                s => s.Length
            ).ToList();

            // Assert
            Assert.Equal(new[] { "apple", "banana", "cherry" }, result);
        }

        [Fact]
        public void CustomCollection_SupportsLinqOperations()
        {
            // Arrange
            var collection = new CustomCollection<int>();
            collection.Add(1);
            collection.Add(2);
            collection.Add(3);
            collection.Add(4);
            collection.Add(5);

            // Act
            var evenNumbers = collection.Where(x => x % 2 == 0).ToList();
            var sum = collection.Sum();
            var max = collection.Max();

            // Assert
            Assert.Equal(new[] { 2, 4 }, evenNumbers);
            Assert.Equal(15, sum);
            Assert.Equal(5, max);
        }

        [Fact]
        public void CustomCollection_Count_ReturnsCorrectValue()
        {
            // Arrange
            var collection = new CustomCollection<string>();

            // Act & Assert
            Assert.Equal(0, collection.Count);

            collection.Add("test");
            Assert.Equal(1, collection.Count);

            collection.Add("test2");
            Assert.Equal(2, collection.Count);
        }

        [Fact]
        public void GetReverseEnumerator_WithMockData_WorksCorrectly()
        {
            // Arrange
            var collection = new CustomCollection<string>();
            collection.Add("first");
            collection.Add("second");
            collection.Add("third");

            // Act
            var reversed = collection.GetReverseEnumerator().ToArray();

            // Assert
            Assert.Equal("third", reversed[0]);
            Assert.Equal("second", reversed[1]);
            Assert.Equal("first", reversed[2]);
        }
    }
}