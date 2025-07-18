using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Task13;
using Xunit;

namespace Task13Tests
{
    public class StudentTests
    {
        private readonly StudentJsonService _service;

        public StudentTests()
        {
            _service = new StudentJsonService();
        }

        [Fact]
        public void Student_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            var firstName = "John";
            var lastName = "Doe";
            var birthDate = new DateTime(1995, 5, 15);

            // Act
            var student = new Student(firstName, lastName, birthDate);

            // Assert
            Assert.Equal(firstName, student.FirstName);
            Assert.Equal(lastName, student.LastName);
            Assert.Equal(birthDate, student.BirthDate);
            Assert.NotNull(student.Grades);
            Assert.Empty(student.Grades);
        }

        [Fact]
        public void Student_AddGrade_AddsGradeToList()
        {
            // Arrange
            var student = new Student("John", "Doe", new DateTime(1995, 5, 15));
            var subject = new Subject("Math", 5);

            // Act
            student.AddGrade(subject);

            // Assert
            Assert.Single(student.Grades);
            Assert.Equal(subject, student.Grades[0]);
        }

        [Fact]
        public void Student_GetAverageGrade_ReturnsCorrectAverage()
        {
            // Arrange
            var student = new Student("John", "Doe", new DateTime(1995, 5, 15));
            student.AddGrade(new Subject("Math", 5));
            student.AddGrade(new Subject("Physics", 4));
            student.AddGrade(new Subject("Chemistry", 3));

            // Act
            var average = student.GetAverageGrade();

            // Assert
            Assert.Equal(4.0, average);
        }

        [Fact]
        public void Student_IsValid_ReturnsTrueForValidStudent()
        {
            // Arrange
            var student = new Student("John", "Doe", new DateTime(1995, 5, 15));
            student.AddGrade(new Subject("Math", 5));

            // Act
            var isValid = student.IsValid();

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData("", "Doe", "2023-01-01")]
        [InlineData("John", "", "2023-01-01")]
        [InlineData("John", "Doe", "0001-01-01")]
        public void Student_IsValid_ReturnsFalseForInvalidStudent(string firstName, string lastName, string birthDateString)
        {
            // Arrange
            var birthDate = DateTime.Parse(birthDateString);
            var student = new Student(firstName, lastName, birthDate);

            // Act
            var isValid = student.IsValid();

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void SerializeToJson_ValidStudent_ReturnsJsonString()
        {
            // Arrange
            var student = new Student("John", "Doe", new DateTime(1995, 5, 15));
            student.AddGrade(new Subject("Math", 5));

            // Act
            var json = _service.SerializeToJson(student);

            // Assert
            Assert.NotNull(json);
            Assert.Contains("John", json);
            Assert.Contains("Doe", json);
            Assert.Contains("1995-05-15", json);
            Assert.Contains("Math", json);
        }

        [Fact]
        public void SerializeToJson_NullStudent_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _service.SerializeToJson(null));
        }

        [Fact]
        public void DeserializeFromJson_ValidJson_ReturnsStudent()
        {
            // Arrange
            var json = @"{
                ""firstName"": ""John"",
                ""lastName"": ""Doe"",
                ""birthDate"": ""1995-05-15"",
                ""grades"": [
                    {
                        ""name"": ""Math"",
                        ""grade"": 5
                    }
                ]
            }";

            // Act
            var student = _service.DeserializeFromJson(json);

            // Assert
            Assert.NotNull(student);
            Assert.Equal("John", student.FirstName);
            Assert.Equal("Doe", student.LastName);
            Assert.Equal(new DateTime(1995, 5, 15), student.BirthDate);
            Assert.Single(student.Grades);
            Assert.Equal("Math", student.Grades[0].Name);
            Assert.Equal(5, student.Grades[0].Grade);
        }

        [Fact]
        public void DeserializeFromJson_EmptyJson_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.DeserializeFromJson(""));
        }

        [Fact]
        public void SaveToFile_ValidStudent_CreatesFile()
        {
            // Arrange
            var student = new Student("John", "Doe", new DateTime(1995, 5, 15));
            student.AddGrade(new Subject("Math", 5));
            var filePath = Path.GetTempFileName();

            try
            {
                // Act
                _service.SaveToFile(student, filePath);

                // Assert
                Assert.True(File.Exists(filePath));
                var content = File.ReadAllText(filePath);
                Assert.Contains("John", content);
            }
            finally
            {
                // Cleanup
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }

        [Fact]
        public void LoadFromFile_ValidFile_ReturnsStudent()
        {
            // Arrange
            var student = new Student("John", "Doe", new DateTime(1995, 5, 15));
            student.AddGrade(new Subject("Math", 5));
            var filePath = Path.GetTempFileName();

            try
            {
                _service.SaveToFile(student, filePath);

                // Act
                var loadedStudent = _service.LoadFromFile(filePath);

                // Assert
                Assert.NotNull(loadedStudent);
                Assert.Equal(student.FirstName, loadedStudent.FirstName);
                Assert.Equal(student.LastName, loadedStudent.LastName);
                Assert.Equal(student.BirthDate, loadedStudent.BirthDate);
                Assert.Single(loadedStudent.Grades);
            }
            finally
            {
                // Cleanup
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }

        [Fact]
        public async Task SaveToFileAsync_ValidStudent_CreatesFile()
        {
            // Arrange
            var student = new Student("John", "Doe", new DateTime(1995, 5, 15));
            student.AddGrade(new Subject("Math", 5));
            var filePath = Path.GetTempFileName();

            try
            {
                // Act
                await _service.SaveToFileAsync(student, filePath);

                // Assert
                Assert.True(File.Exists(filePath));
                var content = await File.ReadAllTextAsync(filePath);
                Assert.Contains("John", content);
            }
            finally
            {
                // Cleanup
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }
    }
}