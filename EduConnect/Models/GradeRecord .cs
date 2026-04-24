using System;

namespace EduConnect.Models
{
    public class GradeRecord
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public Guid FacultyId { get; set; }
        public double Marks { get; set; }
        public string LetterGrade { get; set; } = string.Empty;
        public double GradePoints { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Remarks { get; set; } = string.Empty;

        // Navigation properties
        public virtual Student? Student { get; set; }
        public virtual Course? Course { get; set; }

        // Helper methods
        public void CalculateLetterGrade()
        {
            if (Marks >= 85)
            {
                LetterGrade = "A";
                GradePoints = 4.0;
            }
            else if (Marks >= 70)
            {
                LetterGrade = "B";
                GradePoints = 3.0;
            }
            else if (Marks >= 55)
            {
                LetterGrade = "C";
                GradePoints = 2.0;
            }
            else if (Marks >= 45)
            {
                LetterGrade = "D";
                GradePoints = 1.0;
            }
            else
            {
                LetterGrade = "F";
                GradePoints = 0.0;
            }
        }

        public string GetGradeStatus()
        {
            return LetterGrade switch
            {
                "A" => "Excellent",
                "B" => "Good",
                "C" => "Average",
                "D" => "Pass",
                "F" => "Fail",
                _ => "Pending"
            };
        }

        public string GetCssClass()
        {
            return LetterGrade switch
            {
                "A" or "B" => "table-success",
                "C" or "D" => "table-warning",
                "F" => "table-danger",
                _ => ""
            };
        }
    }
}