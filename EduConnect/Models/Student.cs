using System;

namespace EduConnect.Models
{
    public class Student : Person
    {
        public int Semester { get; set; }
        public double CGPA { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        
        // Navigation properties
        public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        
        public override string GetRole()
        {
            return "Student";
        }
        
        // Helper method to get enrolled courses count
        public int GetEnrolledCoursesCount()
        {
            return Enrollments?.Count(e => e.IsActive) ?? 0;
        }
        
        // Helper method to check if student is enrolled in specific course
        public bool IsEnrolledInCourse(Guid courseId)
        {
            return Enrollments?.Any(e => e.CourseId == courseId && e.IsActive) ?? false;
        }
    }
}