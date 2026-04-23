using System;

namespace EduConnect.Models
{
    public class Enrollment
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? DropDate { get; set; }

        // Navigation properties
        public virtual Student? Student { get; set; }
        public virtual Course? Course { get; set; }

        // Helper property
        public string Status => IsActive ? "Active" : "Dropped";
    }
}