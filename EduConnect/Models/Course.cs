using System;
using System.Collections.Generic;
using EduConnect.Models;

namespace EduConnect.Models
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        public int MaxCapacity { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;

        // Navigation property
        public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        // Computed properties
        public int CurrentEnrollment => Enrollments?.Count(e => e.IsActive) ?? 0;
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