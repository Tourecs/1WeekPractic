namespace task02
{
    /// <summary>
    /// Модель студента
    /// </summary>
    public class Student
    {
        public string Name { get; set; }
        public string Faculty { get; set; }
        public double AverageGrade { get; set; }

        public Student(string name, string faculty, double averageGrade)
        {
            Name = name;
            Faculty = faculty;
            AverageGrade = averageGrade;
        }

        public override string ToString()
        {
            return $"{Name} ({Faculty}) - {AverageGrade:F2}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Student other)
            {
                return Name == other.Name && 
                       Faculty == other.Faculty && 
                       Math.Abs(AverageGrade - other.AverageGrade) < 0.001;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Faculty, AverageGrade);
        }
    }
}