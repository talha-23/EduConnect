using EduConnect.Data;
using EduConnect.Models;
using EduConnect.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Services
{
    public class GradeService : IGradeService
    {
        private readonly AppDbContext _context;
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly INotificationService _notificationService;

        public event Action<GradeRecord>? OnGradeSubmitted;

        public GradeService(
            AppDbContext context,
            IStudentService studentService,
            ICourseService courseService,
            INotificationService notificationService)
        {
            _context = context;
            _studentService = studentService;
            _courseService = courseService;
            _notificationService = notificationService;
        }

        public async Task<GradeRecord> SubmitGradeAsync(GradeRecord grade)
        {
            if (!await ValidateMarksAsync(grade.Marks))
                throw new Exception("Marks must be between 0 and 100");

            var existing = await GetGradeByStudentAndCourseAsync(grade.StudentId, grade.CourseId);
            GradeRecord result;

            if (existing != null)
            {
                existing.Marks = grade.Marks;
                existing.Remarks = grade.Remarks;
                existing.SubmissionDate = DateTime.Now;
                existing.CalculateLetterGrade();
                _context.GradeRecords.Update(existing);
                result = existing;
            }
            else
            {
                grade.Id = Guid.NewGuid();
                grade.SubmissionDate = DateTime.Now;
                grade.CalculateLetterGrade();
                _context.GradeRecords.Add(grade);
                result = grade;
            }

            await _context.SaveChangesAsync();
            await _notificationService.NotifyGradeSubmittedAsync(grade.StudentId, grade.CourseId, grade.Marks, grade.LetterGrade);
            OnGradeSubmitted?.Invoke(result);

            return result;
        }

        public async Task<List<GradeRecord>> SubmitMultipleGradesAsync(List<GradeRecord> grades)
        {
            var submittedGrades = new List<GradeRecord>();
            foreach (var grade in grades)
            {
                var submitted = await SubmitGradeAsync(grade);
                submittedGrades.Add(submitted);
            }
            return submittedGrades;
        }

        public async Task<GradeRecord?> UpdateGradeAsync(GradeRecord grade)
        {
            var existing = await GetGradeByStudentAndCourseAsync(grade.StudentId, grade.CourseId);
            if (existing != null)
            {
                existing.Marks = grade.Marks;
                existing.Remarks = grade.Remarks;
                existing.SubmissionDate = DateTime.Now;
                existing.CalculateLetterGrade();
                _context.GradeRecords.Update(existing);
                await _context.SaveChangesAsync();
                return existing;
            }
            return null;
        }

        public async Task<bool> DeleteGradeAsync(Guid gradeId)
        {
            var grade = await _context.GradeRecords.FindAsync(gradeId);
            if (grade != null)
            {
                _context.GradeRecords.Remove(grade);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<GradeRecord?> GetGradeByStudentAndCourseAsync(Guid studentId, Guid courseId)
        {
            return await _context.GradeRecords
                .FirstOrDefaultAsync(g => g.StudentId == studentId && g.CourseId == courseId);
        }

        public async Task<List<GradeRecord>> GetGradesByStudentAsync(Guid studentId)
        {
            return await _context.GradeRecords
                .Where(g => g.StudentId == studentId)
                .OrderBy(g => g.SubmissionDate)
                .ToListAsync();
        }

        public async Task<List<GradeRecord>> GetGradesByCourseAsync(Guid courseId)
        {
            return await _context.GradeRecords
                .Where(g => g.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<List<GradeRecord>> GetGradesByFacultyAsync(Guid facultyId)
        {
            return await _context.GradeRecords
                .Where(g => g.FacultyId == facultyId)
                .ToListAsync();
        }

        public async Task<double> CalculateStudentCGPAAsync(Guid studentId)
        {
            var grades = await GetGradesByStudentAsync(studentId);
            double totalGradePoints = 0;
            int totalCredits = 0;

            foreach (var grade in grades)
            {
                var course = await _courseService.GetCourseByIdAsync(grade.CourseId);
                if (course != null)
                {
                    totalGradePoints += grade.GradePoints * course.CreditHours;
                    totalCredits += course.CreditHours;
                }
            }

            return totalCredits > 0 ? totalGradePoints / totalCredits : 0;
        }

        public async Task<Dictionary<string, int>> GetGradeDistributionAsync(Guid courseId)
        {
            var grades = await GetGradesByCourseAsync(courseId);
            var distribution = new Dictionary<string, int>
            {
                { "A", 0 }, { "B", 0 }, { "C", 0 }, { "D", 0 }, { "F", 0 }
            };

            foreach (var grade in grades)
            {
                if (distribution.ContainsKey(grade.LetterGrade))
                    distribution[grade.LetterGrade]++;
            }
            return distribution;
        }

        public async Task<CourseStatistics> GetCourseStatisticsAsync(Guid courseId)
        {
            var grades = await GetGradesByCourseAsync(courseId);
            var course = await _courseService.GetCourseByIdAsync(courseId);

            var statistics = new CourseStatistics
            {
                CourseId = courseId,
                CourseName = course?.Title ?? "Unknown Course",
                TotalStudents = grades.Count,
                GradeDistribution = await GetGradeDistributionAsync(courseId)
            };

            if (grades.Any())
            {
                statistics.AverageMarks = grades.Average(g => g.Marks);
                statistics.HighestMarks = grades.Max(g => g.Marks);
                statistics.LowestMarks = grades.Min(g => g.Marks);
                statistics.PassCount = grades.Count(g => g.LetterGrade != "F");
                statistics.FailCount = grades.Count(g => g.LetterGrade == "F");
                statistics.PassPercentage = grades.Count > 0 ? (double)statistics.PassCount / grades.Count * 100 : 0;
            }

            return statistics;
        }

        public async Task<bool> HasStudentReceivedGradeAsync(Guid studentId, Guid courseId)
        {
            return await _context.GradeRecords
                .AnyAsync(g => g.StudentId == studentId && g.CourseId == courseId);
        }

        public async Task<bool> ValidateMarksAsync(double marks)
        {
            return marks >= 0 && marks <= 100;
        }
    }
}