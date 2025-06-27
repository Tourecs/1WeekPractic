using Xunit;
using task01;

namespace task01tests
{
    public class StringExtensionsTests
    {
        [Fact]
        public void IsPalindrome_SimpleWord_ReturnsFalse()
        {
            // Arrange
            string input = "hello";
            
            // Act
            bool result = input.IsPalindrome();
            
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsPalindrome_RussianPalindromeWithSpacesAndPunctuation_ReturnsTrue()
        {
            // Arrange
            string input = "А роза упала на лапу Азора";
            
            // Act
            bool result = input.IsPalindrome();
            
            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPalindrome_SimplePalindrome_ReturnsTrue()
        {
            // Arrange
            string input = "level";
            
            // Act
            bool result = input.IsPalindrome();
            
            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPalindrome_PalindromeWithMixedCase_ReturnsTrue()
        {
            // Arrange
            string input = "Madam";
            
            // Act
            bool result = input.IsPalindrome();
            
            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPalindrome_PalindromeWithSpacesAndPunctuation_ReturnsTrue()
        {
            // Arrange
            string input = "A man, a plan, a canal: Panama";
            
            // Act
            bool result = input.IsPalindrome();
            
            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPalindrome_EmptyString_ReturnsTrue()
        {
            // Arrange
            string input = "";
            
            // Act
            bool result = input.IsPalindrome();
            
            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPalindrome_NullString_ReturnsTrue()
        {
            // Arrange
            string input = null;
            
            // Act
            bool result = input.IsPalindrome();
            
            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPalindrome_SingleCharacter_ReturnsTrue()
        {
            // Arrange
            string input = "a";
            
            // Act
            bool result = input.IsPalindrome();
            
            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPalindrome_NumberPalindrome_ReturnsTrue()
        {
            // Arrange
            string input = "12321";
            
            // Act
            bool result = input.IsPalindrome();
            
            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPalindrome_PalindromeWithNumbers_ReturnsTrue()
        {
            // Arrange
            string input = "A1B2b1a";
            
            // Act
            bool result = input.IsPalindrome();
            
            // Assert
            Assert.True(result);
        }
    }
}