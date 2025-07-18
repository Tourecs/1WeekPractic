using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Task13
{
    public class Subject
    {
        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Range(1, 5, ErrorMessage = "Grade must be between 1 and 5")]
        [JsonPropertyName("grade")]
        public int Grade { get; set; }

        public Subject() { }

        public Subject(string name, int grade)
        {
            Name = name;
            Grade = grade;
        }

        public override string ToString()
        {
            return $"{Name}: {Grade}";
        }
    }
}