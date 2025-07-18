using System;
using Task14;
using Xunit;

namespace Task14Tests
{
    public class DefiniteIntegralTests
    {
        [Fact]
        public void Solve_LinearFunction_ReturnsCorrectResult()
        {
            // Arrange
            var X = (double x) => x;
            
            // Act & Assert
            // Интеграл от x на [-1, 1] должен быть 0
            Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2), 1e-4);
        }
        
        [Fact]
        public void Solve_SineFunction_ReturnsCorrectResult()
        {
            // Arrange
            var SIN = (double x) => Math.Sin(x);
            
            // Act & Assert
            // Интеграл от sin(x) на [-1, 1] должен быть 0
            Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8), 1e-4);
        }
        
        [Fact]
        public void Solve_LinearFunctionPositiveInterval_ReturnsCorrectResult()
        {
            // Arrange
            var X = (double x) => x;
            
            // Act & Assert
            // Интеграл от x на [0, 5] должен быть 12.5, но в тесте ожидается 10
            // Проверим с заданной точностью
            var result = DefiniteIntegral.Solve(0, 5, X, 1e-6, 8);
            Assert.Equal(12.5, result, 1e-5);
        }
        
        [Fact]
        public void Solve_ConstantFunction_ReturnsCorrectResult()
        {
            // Arrange
            var CONST = (double x) => 2.0;
            
            // Act
            var result = DefiniteIntegral.Solve(0, 5, CONST, 1e-4, 4);
            
            // Assert
            // Интеграл от константы 2 на [0, 5] должен быть 10
            Assert.Equal(10.0, result, 1e-3);
        }
        
        [Fact]
        public void Solve_QuadraticFunction_ReturnsCorrectResult()
        {
            // Arrange
            var SQUARE = (double x) => x * x;
            
            // Act
            var result = DefiniteIntegral.Solve(0, 3, SQUARE, 1e-5, 6);
            
            // Assert
            // Интеграл от x^2 на [0, 3] должен быть 9
            Assert.Equal(9.0, result, 1e-3);
        }
        
        [Fact]
        public void Solve_SingleThread_ReturnsCorrectResult()
        {
            // Arrange
            var X = (double x) => x;
            
            // Act
            var result = DefiniteIntegral.Solve(0, 2, X, 1e-4, 1);
            
            // Assert
            // Интеграл от x на [0, 2] должен быть 2
            Assert.Equal(2.0, result, 1e-3);
        }
        
        [Fact]
        public void Solve_ManyThreads_ReturnsCorrectResult()
        {
            // Arrange
            var X = (double x) => x;
            
            // Act
            var result = DefiniteIntegral.Solve(0, 4, X, 1e-5, 16);
            
            // Assert
            // Интеграл от x на [0, 4] должен быть 8
            Assert.Equal(8.0, result, 1e-3);
        }
        
        [Fact]
        public void Solve_NullFunction_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                DefiniteIntegral.Solve(0, 1, null, 1e-4, 2));
        }
        
        [Fact]
        public void Solve_NegativeStep_ThrowsArgumentException()
        {
            // Arrange
            var X = (double x) => x;
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                DefiniteIntegral.Solve(0, 1, X, -1e-4, 2));
        }
        
        [Fact]
        public void Solve_ZeroThreads_ThrowsArgumentException()
        {
            // Arrange
            var X = (double x) => x;
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                DefiniteIntegral.Solve(0, 1, X, 1e-4, 0));
        }
        
        [Fact]
        public void Solve_InvalidInterval_ReturnsZero()
        {
            // Arrange
            var X = (double x) => x;
            
            // Act
            var result = DefiniteIntegral.Solve(5, 2, X, 1e-4, 2);
            
            // Assert
            Assert.Equal(0.0, result);
        }
        
        [Fact]
        public void Solve_ExponentialFunction_ReturnsCorrectResult()
        {
            // Arrange
            var EXP = (double x) => Math.Exp(x);
            
            // Act
            var result = DefiniteIntegral.Solve(0, 1, EXP, 1e-5, 4);
            
            // Assert
            // Интеграл от e^x на [0, 1] должен быть e - 1 ≈ 1.718
            var expected = Math.E - 1;
            Assert.Equal(expected, result, 1e-3);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        public void Solve_DifferentThreadCounts_ReturnsSameResult(int threadCount)
        {
            // Arrange
            var X = (double x) => x * x;
            
            // Act
            var result = DefiniteIntegral.Solve(0, 2, X, 1e-5, threadCount);
            
            // Assert
            // Интеграл от x^2 на [0, 2] должен быть 8/3 ≈ 2.667
            Assert.Equal(8.0/3.0, result, 1e-3);
        }
    }
}