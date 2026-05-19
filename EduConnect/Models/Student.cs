using System;

namespace EduConnect.Models
{
    public class Student : Person
    {
        public int Semester { get; set; }
        public double CGPA { get; set; }  // Changed from decimal to double
        public string StudentId { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

        public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public override string GetRole()
        {
            return "Student";
        }

        public int GetEnrolledCoursesCount()
        {
            return Enrollments?.Count(e => e.IsActive) ?? 0;
        }

        public bool IsEnrolledInCourse(Guid courseId)
        {
            return Enrollments?.Any(e => e.CourseId == courseId && e.IsActive) ?? false;
        }
    }
}