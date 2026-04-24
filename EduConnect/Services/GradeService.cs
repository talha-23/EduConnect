using EduConnect.Models;
using EduConnect.Services.Interfaces;

namespace EduConnect.Services
{
    public class GradeService : IGradeService
    {
        private static List<GradeRecord> _grades = new List<GradeRecord>();
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly INotificationService _notificationService;

        public event Action<GradeRecord>? OnGradeSubmitted;

        public GradeService(
            IStudentService studentService,
            ICourseService courseService,
            IEnrollmentService enrollmentService,
            INotificationService notificationService)
        {
            _studentService = studentService;
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _notificationService = notificationService;

            // Initialize sample grades
            InitializeSampleGrades();
        }

        private async void InitializeSampleGrades()
        {
            try
            {
                var students = await _studentService.GetAllStudentsAsync();
                var courses = await _courseService.GetAllCoursesAsync();

                if (students.Any() && courses.Any() && !_grades.Any())
                {
                    // Add sample grades for students
                    foreach (var student in students)
                    {
                        foreach (var course in courses.Take(2))
                        {
                            var random = new Random();
                            var marks = random.Next(65, 92);
                            var grade = new GradeRecord
                            {
                                Id = Guid.NewGuid(),
                                StudentId = student.Id,
                                CourseId = course.Id,
                                FacultyId = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                                Marks = marks,
                                SubmissionDate = DateTime.Now.AddDays(-random.Next(1, 20)),
                                Remarks = "Sample grade"
                            };
                            grade.CalculateLetterGrade();
                            _grades.Add(grade);
                        }
                    }
                    Console.WriteLine($"Initialized {_grades.Count} sample grades");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing sample grades: {ex.Message}");
            }
        }

        public async Task<GradeRecord> SubmitGradeAsync(GradeRecord grade)
        {
            await Task.Delay(100);

            // Validate marks
            if (!await ValidateMarksAsync(grade.Marks))
                throw new Exception("Marks must be between 0 and 100");

            // Check if grade already exists
            var existing = await GetGradeByStudentAndCourseAsync(grade.StudentId, grade.CourseId);
            GradeRecord result;

            if (existing != null)
            {
                // Update existing grade
                existing.Marks = grade.Marks;
                existing.Remarks = grade.Remarks;
                existing.SubmissionDate = DateTime.Now;
                existing.CalculateLetterGrade();
                result = existing;
            }
            else
            {
                // Add new grade
                grade.Id = Guid.NewGuid();
                grade.SubmissionDate = DateTime.Now;
                grade.CalculateLetterGrade();
                _grades.Add(grade);
                result = grade;
            }

            // Send notification for grade submission
            await _notificationService.NotifyGradeSubmittedAsync(grade.StudentId, grade.CourseId, grade.Marks, grade.LetterGrade);

            // Fire event for real-time updates
            OnGradeSubmitted?.Invoke(result);

            return result;
        }

        public async Task<List<GradeRecord>> SubmitMultipleGradesAsync(List<GradeRecord> grades)
        {
            await Task.Delay(100);
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
            await Task.Delay(100);
            var existing = await GetGradeByStudentAndCourseAsync(grade.StudentId, grade.CourseId);

            if (existing != null)
            {
                existing.Marks = grade.Marks;
                existing.Remarks = grade.Remarks;
                existing.SubmissionDate = DateTime.Now;
                existing.CalculateLetterGrade();

                // Send notification for grade update
                await _notificationService.NotifyGradeSubmittedAsync(grade.StudentId, grade.CourseId, grade.Marks, grade.LetterGrade);

                // Fire event for real-time updates
                OnGradeSubmitted?.Invoke(existing);

                return existing;
            }

            return null;
        }

        public async Task<bool> DeleteGradeAsync(Guid gradeId)
        {
            await Task.Delay(100);
            var grade = _grades.FirstOrDefault(g => g.Id == gradeId);
            if (grade != null)
            {
                return _grades.Remove(grade);
            }
            return false;
        }

        public async Task<GradeRecord?> GetGradeByStudentAndCourseAsync(Guid studentId, Guid courseId)
        {
            await Task.Delay(50);
            return _grades.FirstOrDefault(g => g.StudentId == studentId && g.CourseId == courseId);
        }

        public async Task<List<GradeRecord>> GetGradesByStudentAsync(Guid studentId)
        {
            await Task.Delay(50);
            return _grades.Where(g => g.StudentId == studentId).OrderBy(g => g.SubmissionDate).ToList();
        }

        public async Task<List<GradeRecord>> GetGradesByCourseAsync(Guid courseId)
        {
            await Task.Delay(50);
            return _grades.Where(g => g.CourseId == courseId).ToList();
        }

        public async Task<List<GradeRecord>> GetGradesByFacultyAsync(Guid facultyId)
        {
            await Task.Delay(50);
            return _grades.Where(g => g.FacultyId == facultyId).ToList();
        }

        public async Task<double> CalculateStudentCGPAAsync(Guid studentId)
        {
            await Task.Delay(100);
            var grades = await GetGradesByStudentAsync(studentId);
            var totalGradePoints = 0.0;
            var totalCredits = 0;

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
            await Task.Delay(100);
            var grades = await GetGradesByCourseAsync(courseId);
            var distribution = new Dictionary<string, int>
            {
                { "A", 0 },
                { "B", 0 },
                { "C", 0 },
                { "D", 0 },
                { "F", 0 }
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
            await Task.Delay(100);
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
                statistics.PassPercentage = (double)statistics.PassCount / grades.Count * 100;
            }

            return statistics;
        }

        public async Task<bool> HasStudentReceivedGradeAsync(Guid studentId, Guid courseId)
        {
            await Task.Delay(50);
            return _grades.Any(g => g.StudentId == studentId && g.CourseId == courseId);
        }

        public async Task<bool> ValidateMarksAsync(double marks)
        {
            await Task.Delay(10);
            return marks >= 0 && marks <= 100;
        }

        // Additional helper methods for reports
        public async Task<List<GradeRecord>> GetAllGradesAsync()
        {
            await Task.Delay(50);
            return _grades.ToList();
        }

        public async Task<double> GetCourseAverageAsync(Guid courseId)
        {
            var grades = await GetGradesByCourseAsync(courseId);
            return grades.Any() ? grades.Average(g => g.Marks) : 0;
        }

        public async Task<Dictionary<Guid, double>> GetAllStudentCGPAsAsync()
        {
            var students = await _studentService.GetAllStudentsAsync();
            var cgpas = new Dictionary<Guid, double>();

            foreach (var student in students)
            {
                cgpas[student.Id] = await CalculateStudentCGPAAsync(student.Id);
            }

            return cgpas;
        }
    }
}