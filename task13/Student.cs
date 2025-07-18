using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;

namespace Task13
{
    public class Student
    {
        [Required]
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [Required]
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("birthDate")]
        public DateTime BirthDate { get; set; }

        [JsonPropertyName("grades")]
        public List<Subject> Grades { get; set; }

        public Student()
        {
            Grades = new List<Subject>();
        }

        public Student(string firstName, string lastName, DateTime birthDate)
        {
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
            Grades = new List<Subject>();
        }

        public void AddGrade(Subject subject)
        {
            Grades.Add(subject);
        }

        public double GetAverageGrade()
        {
            return Grades.Any() ? Grades.Average(g => g.Grade) : 0;
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(FirstName) && 
                   !string.IsNullOrWhiteSpace(LastName) && 
                   BirthDate != default(DateTime) &&
                   Grades != null &&
                   Grades.All(g => !string.IsNullOrWhiteSpace(g.Name) && g.Grade >= 1 && g.Grade <= 5);
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName}, Born: {BirthDate:yyyy-MM-dd}, Average Grade: {GetAverageGrade():F2}";
        }
    }
}