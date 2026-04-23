namespace EduConnect.Models
{
    /// <summary>
    /// Student entity - inherits from Person (Liskov Substitution Principle)
    /// </summary>
    public class Student : Person
    {
        public int Semester { get; set; }
        public double CGPA { get; set; }

        public override string GetRole()
        {
            return "Student";
        }

        // Additional student-specific properties can be added later
        public List<Guid> EnrolledCourseIds { get; set; } = new List<Guid>();
    }
}