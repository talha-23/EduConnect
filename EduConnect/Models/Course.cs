using System;

namespace EduConnect.Models
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        public int MaxCapacity { get; set; }
        private int _currentEnrollment;
        
        // Make CurrentEnrollment settable (not just computed)
        public int CurrentEnrollment 
        { 
            get => _currentEnrollment;
            set => _currentEnrollment = value;
        }
        
        public string Description { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        
        // Navigation property
        public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        
        // Computed properties (read-only)
        public bool IsFull => CurrentEnrollment >= MaxCapacity;
        public int AvailableSeats => MaxCapacity - CurrentEnrollment;
        public string EnrollmentStatus => IsFull ? "Full" : (AvailableSeats <= 5 ? "Almost Full" : "Open");
        
        // Helper method to get enrollment percentage
        public double GetEnrollmentPercentage()
        {
            return MaxCapacity > 0 ? (double)CurrentEnrollment / MaxCapacity * 100 : 0;
        }


    }
}