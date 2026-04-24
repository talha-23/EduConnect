using EduConnect.Models;

namespace EduConnect.Services.Interfaces
{
    public interface IGradeService
    {
        // Grade management
        Task<GradeRecord> SubmitGradeAsync(GradeRecord grade);
        Task<List<GradeRecord>> SubmitMultipleGradesAsync(List<GradeRecord> grades);
        Task<GradeRecord?> UpdateGradeAsync(GradeRecord grade);
        Task<bool> DeleteGradeAsync(Guid gradeId);

        // Grade retrieval
        Task<GradeRecord?> GetGradeByStudentAndCourseAsync(Guid studentId, Guid courseId);
        Task<List<GradeRecord>> GetGradesByStudentAsync(Guid studentId);
        Task<List<GradeRecord>> GetGradesByCourseAsync(Guid courseId);
        Task<List<GradeRecord>> GetGradesByFacultyAsync(Guid facultyId);

        // Grade calculation
        Task<double> CalculateStudentCGPAAsync(Guid studentId);
        Task<Dictionary<string, int>> GetGradeDistributionAsync(Guid courseId);
        Task<CourseStatistics> GetCourseStatisticsAsync(Guid courseId);

        // Validation
        Task<bool> HasStudentReceivedGradeAsync(Guid studentId, Guid courseId);
        Task<bool> ValidateMarksAsync(double marks);

        // Events
        event Action<GradeRecord> OnGradeSubmitted;
    }

    public class CourseStatistics
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
        public double AverageMarks { get; set; }
        public double HighestMarks { get; set; }
        public double LowestMarks { get; set; }
        public int PassCount { get; set; }
        public int FailCount { get; set; }
        public double PassPercentage { get; set; }
        public Dictionary<string, int> GradeDistribution { get; set; } = new();
    }
}