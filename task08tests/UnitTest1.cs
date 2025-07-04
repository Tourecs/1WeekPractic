using System;
using System.IO;
using System.Linq;
using Xunit;
using FileSystemCommands;
using CommandLib;

namespace task08tests
{
    public class FileSystemCommandsTests : IDisposable
    {
        private readonly string _testDir;

        public FileSystemCommandsTests()
        {
            // Создаем уникальную папку для каждого теста
            _testDir = Path.Combine(Path.GetTempPath(), $"TestDir_{Guid.NewGuid():N}");
            Directory.CreateDirectory(_testDir);
        }

        public void Dispose()
        {
            // Очищаем после каждого теста
            if (Directory.Exists(_testDir))
            {
                Directory.Delete(_testDir, true);
            }
        }

        [Fact]
        public void DirectorySizeCommand_ShouldCalculateSize()
        {
            // Arrange
            File.WriteAllText(Path.Combine(_testDir, "test1.txt"), "Hello");
            File.WriteAllText(Path.Combine(_testDir, "test2.txt"), "World");

            var command = new DirectorySizeCommand(_testDir);

            // Act
            command.Execute(); // Проверяем, что не возникает исключений

            // Assert
            Assert.True(command.LastCalculatedSize > 0);
            Assert.Equal("DirectorySize", command.Name);
            Assert.Contains(_testDir, command.Description);
        }

        [Fact]
        public void DirectorySizeCommand_NonExistentDirectory_ShouldHandleGracefully()
        {
            // Arrange
            var nonExistentPath = Path.Combine(_testDir, "NonExistent");
            var command = new DirectorySizeCommand(nonExistentPath);

            // Act
            command.Execute();

            // Assert
            Assert.Equal(0, command.LastCalculatedSize);
        }

        [Fact]
        public void DirectorySizeCommand_EmptyDirectory_ShouldReturnZero()
        {
            // Arrange
            var emptyDir = Path.Combine(_testDir, "Empty");
            Directory.CreateDirectory(emptyDir);
            var command = new DirectorySizeCommand(emptyDir);

            // Act
            command.Execute();

            // Assert
            Assert.Equal(0, command.LastCalculatedSize);
        }

        [Fact]
        public void DirectorySizeCommand_WithSubdirectories_ShouldCalculateRecursively()
        {
            // Arrange
            var subDir = Path.Combine(_testDir, "SubDir");
            Directory.CreateDirectory(subDir);
            
            File.WriteAllText(Path.Combine(_testDir, "root.txt"), "Root file content");
            File.WriteAllText(Path.Combine(subDir, "sub.txt"), "Sub file content");

            var command = new DirectorySizeCommand(_testDir);

            // Act
            command.Execute();

            // Assert
            Assert.True(command.LastCalculatedSize > 0);
            
            // Размер должен включать оба файла
            var expectedSize = "Root file content".Length + "Sub file content".Length;
            Assert.Equal(expectedSize, command.LastCalculatedSize);
        }

        [Fact]
        public void FindFilesCommand_ShouldFindMatchingFiles()
        {
            // Arrange
            File.WriteAllText(Path.Combine(_testDir, "file1.txt"), "Text");
            File.WriteAllText(Path.Combine(_testDir, "file2.log"), "Log");
            File.WriteAllText(Path.Combine(_testDir, "file3.txt"), "More text");

            var command = new FindFilesCommand(_testDir, "*.txt");

            // Act
            command.Execute(); // Должен найти 2 файла

            // Assert
            Assert.Equal(2, command.LastFoundFiles.Count);
            Assert.Equal("FindFiles", command.Name);
            Assert.Contains("file1.txt", command.LastFoundFiles.First());
            Assert.Contains("file3.txt", command.LastFoundFiles.Last());
        }

        [Fact]
        public void FindFilesCommand_NoMatchingFiles_ShouldReturnEmpty()
        {
            // Arrange
            File.WriteAllText(Path.Combine(_testDir, "file1.txt"), "Text");
            File.WriteAllText(Path.Combine(_testDir, "file2.log"), "Log");

            var command = new FindFilesCommand(_testDir, "*.pdf");

            // Act
            command.Execute();

            // Assert
            Assert.Empty(command.LastFoundFiles);
        }

        [Fact]
        public void FindFilesCommand_NonExistentDirectory_ShouldHandleGracefully()
        {
            // Arrange
            var nonExistentPath = Path.Combine(_testDir, "NonExistent");
            var command = new FindFilesCommand(nonExistentPath, "*.txt");

            // Act
            command.Execute();

            // Assert
            Assert.Empty(command.LastFoundFiles);
        }

        [Fact]
        public void FindFilesCommand_RecursiveSearch_ShouldFindInSubdirectories()
        {
            // Arrange
            var subDir = Path.Combine(_testDir, "SubDir");
            Directory.CreateDirectory(subDir);
            
            File.WriteAllText(Path.Combine(_testDir, "root.txt"), "Root");
            File.WriteAllText(Path.Combine(subDir, "sub.txt"), "Sub");

            var command = new FindFilesCommand(_testDir, "*.txt", recursive: true);

            // Act
            command.Execute();

            // Assert
            Assert.Equal(2, command.LastFoundFiles.Count);
            Assert.Contains(command.LastFoundFiles, f => f.Contains("root.txt"));
            Assert.Contains(command.LastFoundFiles, f => f.Contains("sub.txt"));
        }

        [Fact]
        public void FindFilesCommand_NonRecursiveSearch_ShouldNotFindInSubdirectories()
        {
            // Arrange
            var subDir = Path.Combine(_testDir, "SubDir");
            Directory.CreateDirectory(subDir);
            
            File.WriteAllText(Path.Combine(_testDir, "root.txt"), "Root");
            File.WriteAllText(Path.Combine(subDir, "sub.txt"), "Sub");

            var command = new FindFilesCommand(_testDir, "*.txt", recursive: false);

            // Act
            command.Execute();

            // Assert
            Assert.Single(command.LastFoundFiles);
            Assert.Contains(command.LastFoundFiles, f => f.Contains("root.txt"));
            Assert.DoesNotContain(command.LastFoundFiles, f => f.Contains("sub.txt"));
        }

        [Fact]
        public void FindFilesCommand_GetFoundFilesInfo_ShouldReturnFileInfoList()
        {
            // Arrange
            File.WriteAllText(Path.Combine(_testDir, "test.txt"), "Test content");
            var command = new FindFilesCommand(_testDir, "*.txt");

            // Act
            command.Execute();
            var filesInfo = command.GetFoundFilesInfo();

            // Assert
            Assert.Single(filesInfo);
            Assert.Equal("test.txt", filesInfo.First().Name);
            Assert.True(filesInfo.First().Length > 0);
        }

        [Fact]
        public void FindFilesCommand_FilterBySize_ShouldFilterCorrectly()
        {
            // Arrange
            File.WriteAllText(Path.Combine(_testDir, "small.txt"), "Hi"); // 2 bytes
            File.WriteAllText(Path.Combine(_testDir, "large.txt"), "This is a longer file content"); // > 2 bytes

            var command = new FindFilesCommand(_testDir, "*.txt");

            // Act
            command.Execute();
            var smallFiles = command.FilterBySize(0, 5);
            var largeFiles = command.FilterBySize(10, long.MaxValue);

            // Assert
            Assert.Single(smallFiles);
            Assert.Contains(smallFiles, f => f.Contains("small.txt"));
            
            Assert.Single(largeFiles);
            Assert.Contains(largeFiles, f => f.Contains("large.txt"));
        }

        [Fact]
        public void FindFilesCommand_GroupByExtension_ShouldGroupCorrectly()
        {
            // Arrange
            File.WriteAllText(Path.Combine(_testDir, "file1.txt"), "Text1");
            File.WriteAllText(Path.Combine(_testDir, "file2.txt"), "Text2");
            File.WriteAllText(Path.Combine(_testDir, "file1.log"), "Log1");

            var command = new FindFilesCommand(_testDir, "*.*");

            // Act
            command.Execute();
            var grouped = command.GroupByExtension();

            // Assert
            Assert.Equal(2, grouped.Count);
            Assert.True(grouped.ContainsKey(".txt"));
            Assert.True(grouped.ContainsKey(".log"));
            Assert.Equal(2, grouped[".txt"].Count);
            Assert.Single(grouped[".log"]);
        }

        [Fact]
        public void DirectorySizeCommand_GetDirectoryInfo_ShouldReturnValidInfo()
        {
            // Arrange
            var command = new DirectorySizeCommand(_testDir);

            // Act
            var dirInfo = command.GetDirectoryInfo();

            // Assert
            Assert.NotNull(dirInfo);
            Assert.Equal(_testDir, dirInfo.FullName);
            Assert.True(dirInfo.Exists);
        }

        [Fact]
        public void DirectorySizeCommand_GetDirectoryInfo_NonExistentDirectory_ShouldThrow()
        {
            // Arrange
            var nonExistentPath = Path.Combine(_testDir, "NonExistent");
            var command = new DirectorySizeCommand(nonExistentPath);

            // Act & Assert
            Assert.Throws<DirectoryNotFoundException>(() => command.GetDirectoryInfo());
        }

        [Fact]
        public void Commands_ShouldImplementICommandInterface()
        {
            // Arrange & Act
            ICommand dirCommand = new DirectorySizeCommand(_testDir);
            ICommand findCommand = new FindFilesCommand(_testDir, "*.*");

            // Assert
            Assert.NotNull(dirCommand.Name);
            Assert.NotNull(dirCommand.Description);
            Assert.NotNull(findCommand.Name);
            Assert.NotNull(findCommand.Description);
        }

        [Theory]
        [InlineData("*.txt")]
        [InlineData("*.log")]
        [InlineData("file*")]
        [InlineData("*.")]
        public void FindFilesCommand_DifferentPatterns_ShouldWork(string pattern)
        {
            // Arrange
            File.WriteAllText(Path.Combine(_testDir, "test.txt"), "Text");
            File.WriteAllText(Path.Combine(_testDir, "test.log"), "Log");
            File.WriteAllText(Path.Combine(_testDir, "file.dat"), "Data");

            var command = new FindFilesCommand(_testDir, pattern);

            // Act
            var exception = Record.Exception(() => command.Execute());

            // Assert
            Assert.Null(exception); // Не должно быть исключений
        }

        [Fact]
        public void Commands_Constructor_ShouldValidateParameters()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DirectorySizeCommand(null));
            Assert.Throws<ArgumentNullException>(() => new FindFilesCommand(null, "*.txt"));
            Assert.Throws<ArgumentNullException>(() => new FindFilesCommand(_testDir, null));
        }

        [Fact]
        public void Commands_Execute_ShouldNotThrowUnhandledExceptions()
        {
            // Arrange
            var commands = new ICommand[]
            {
                new DirectorySizeCommand(_testDir),
                new FindFilesCommand(_testDir, "*.txt"),
                new DirectorySizeCommand("C:\\NonExistentDirectory\\SubDir"),
                new FindFilesCommand("Z:\\InvalidDrive", "*.*")
            };

            // Act & Assert
            foreach (var command in commands)
            {
                var exception = Record.Exception(() => command.Execute());
                Assert.Null(exception); // Команды должны gracefully обрабатывать ошибки
            }
        }

        [Fact]
        public void DirectorySizeCommand_LargeFiles_ShouldCalculateCorrectly()
        {
            // Arrange
            var content1 = new string('A', 1000); // 1000 bytes
            var content2 = new string('B', 2000); // 2000 bytes
            
            File.WriteAllText(Path.Combine(_testDir, "large1.txt"), content1);
            File.WriteAllText(Path.Combine(_testDir, "large2.txt"), content2);

            var command = new DirectorySizeCommand(_testDir);

            // Act
            command.Execute();

            // Assert
            Assert.Equal(3000, command.LastCalculatedSize);
        }
    }
}