using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Task13
{
    public class StudentJsonService
    {
        private readonly JsonSerializerOptions _jsonOptions;

        public StudentJsonService()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                Converters = { new CustomDateTimeConverter() }
            };
        }

        public string SerializeToJson(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (!student.IsValid())
                throw new ArgumentException("Student data is not valid");

            return JsonSerializer.Serialize(student, _jsonOptions);
        }

        public Student DeserializeFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON string cannot be null or empty");

            try
            {
                var student = JsonSerializer.Deserialize<Student>(json, _jsonOptions);
                
                if (student == null)
                    throw new InvalidOperationException("Failed to deserialize student");

                if (!student.IsValid())
                    throw new ArgumentException("Deserialized student data is not valid");

                return student;
            }
            catch (JsonException ex)
            {
                throw new ArgumentException($"Invalid JSON format: {ex.Message}", ex);
            }
        }

        public async Task SaveToFileAsync(Student student, string filePath)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty");

            var json = SerializeToJson(student);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<Student> LoadFromFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var json = await File.ReadAllTextAsync(filePath);
            return DeserializeFromJson(json);
        }

        public void SaveToFile(Student student, string filePath)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty");

            var json = SerializeToJson(student);
            File.WriteAllText(filePath, json);
        }

        public Student LoadFromFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var json = File.ReadAllText(filePath);
            return DeserializeFromJson(json);
        }
    }
}