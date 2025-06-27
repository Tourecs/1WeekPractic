using System.Collections.Generic;
using System.Linq;
using Xunit;
using task02;

namespace task02tests
{
    public class StudentServiceTests
    {
        private readonly List<Student> _testStudents;
        private readonly StudentService _studentService;

        public StudentServiceTests()
        {
            _testStudents = new List<Student>
            {
                new Student("Анна Иванова", "Информатика", 4.5),
                new Student("Петр Петров", "Математика", 3.8),
                new Student("Мария Сидорова", "Информатика", 4.9),
                new Student("Иван Козлов", "Физика", 4.2),
                new Student("Елена Смирнова", "Математика", 4.7),
                new Student("Дмитрий Волков", "Информатика", 3.9),
                new Student("Ольга Морозова", "Физика", 4.8),
                new Student("Александр Новиков", "Математика", 3.5)
            };

            _studentService = new StudentService(_testStudents);
        }

        [Fact]
        public void GetStudentsByFaculty_ValidFaculty_ReturnsCorrectStudents()
        {
            // Act
            var result = _studentService.GetStudentsByFaculty("Информатика").ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.All(result, student => Assert.Equal("Информатика", student.Faculty));
            Assert.Contains(result, s => s.Name == "Анна Иванова");
            Assert.Contains(result, s => s.Name == "Мария Сидорова");
            Assert.Contains(result, s => s.Name == "Дмитрий Волков");
        }

        [Fact]
        public void GetStudentsByFaculty_CaseInsensitive_ReturnsCorrectStudents()
        {
            // Act
            var result = _studentService.GetStudentsByFaculty("информатика").ToList();

            // Assert
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void GetStudentsByFaculty_NonExistentFaculty_ReturnsEmpty()
        {
            // Act
            var result = _studentService.GetStudentsByFaculty("Химия").ToList();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetStudentsWithMinAverageGrade_ValidGrade_ReturnsCorrectStudents()
        {
            // Act
            var result = _studentService.GetStudentsWithMinAverageGrade(4.5).ToList();

            // Assert
            Assert.Equal(4, result.Count);
            Assert.All(result, student => Assert.True(student.AverageGrade >= 4.5));
        }

        [Fact]
        public void GetStudentsWithMinAverageGrade_HighGrade_ReturnsFewerStudents()
        {
            // Act
            var result = _studentService.GetStudentsWithMinAverageGrade(4.8).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, s => s.Name == "Мария Сидорова");
            Assert.Contains(result, s => s.Name == "Ольга Морозова");
        }

        [Fact]
        public void GetStudentsOrderedByName_ReturnsStudentsInAlphabeticalOrder()
        {
            // Act
            var result = _studentService.GetStudentsOrderedByName().ToList();

            // Assert
            Assert.Equal(8, result.Count);
            
            // Проверяем, что список отсортирован по имени
            for (int i = 1; i < result.Count; i++)
            {
                Assert.True(string.Compare(result[i-1].Name, result[i].Name) <= 0);
            }
            
            // Проверяем первого и последнего студента
            Assert.Equal("Александр Новиков", result.First().Name);
            Assert.Equal("Петр Петров", result.Last().Name);
        }

        [Fact]
        public void GroupStudentsByFaculty_ReturnsCorrectGroups()
        {
            // Act
            var result = _studentService.GroupStudentsByFaculty();

            // Assert
            Assert.Equal(3, result.Count); // 3 факультета
            Assert.Equal(3, result["Информатика"].Count());
            Assert.Equal(3, result["Математика"].Count());
            Assert.Equal(2, result["Физика"].Count());
        }

        [Fact]
        public void GroupStudentsByFaculty_NonExistentFaculty_ReturnsEmpty()
        {
            // Act
            var result = _studentService.GroupStudentsByFaculty();

            // Assert
            Assert.Empty(result["Химия"]);
        }

        [Fact]
        public void GetFacultyWithHighestAverageGrade_ReturnsCorrectFaculty()
        {
            // Act
            var result = _studentService.GetFacultyWithHighestAverageGrade();

            // Assert
            // Физика: (4.2 + 4.8) / 2 = 4.5
            // Математика: (3.8 + 4.7 + 3.5) / 3 = 4.0
            // Информатика: (4.5 + 4.9 + 3.9) / 3 = 4.43
            Assert.Equal("Физика", result);
        }

        [Fact]
        public void GetFacultyWithHighestAverageGrade_EmptyList_ReturnsEmptyString()
        {
            // Arrange
            var emptyService = new StudentService(new List<Student>());

            // Act
            var result = emptyService.GetFacultyWithHighestAverageGrade();

            // Assert
            Assert.Equal(string.Empty, result);
        }
    }
}