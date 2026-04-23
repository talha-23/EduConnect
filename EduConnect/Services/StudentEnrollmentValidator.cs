using EduConnect.Models;
using EduConnect.Services.Interfaces;

namespace EduConnect.Services
{
    public class StudentEnrollmentValidator : IStudentEnrollmentValidator
    {
        private static List<Enrollment> _enrollments = new List<Enrollment>();
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;

        public StudentEnrollmentValidator(IStudentService studentService, ICourseService courseService)
        {
            _studentService = studentService;
            _courseService = courseService;

            // Initialize sample enrollments if empty
            if (!_enrollments.Any())
            {
                InitializeSampleEnrollments();
            }
        }

        private async void InitializeSampleEnrollments()
        {
            var students = await _studentService.GetAllStudentsAsync();
            var courses = await _courseService.GetAllCoursesAsync();

            if (students.Any() && courses.Any())
            {
                // Enroll first student in first two courses
                await EnrollStudentAsync(students[0].Id, courses[0].Id);
                await EnrollStudentAsync(students[0].Id, courses[1].Id);

                // Enroll second student in first course
                await EnrollStudentAsync(students[1].Id, courses[0].Id);
            }
        }

        private async Task<bool> EnrollStudentAsync(Guid studentId, Guid courseId)
        {
            var student = await _studentService.GetStudentByIdAsync(studentId);
            var course = await _courseService.GetCourseByIdAsync(courseId);

            if (student == null || course == null)
                return false;

            if (await IsStudentEnrolledAsync(studentId, courseId))
                return false;

            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.Now,
                IsActive = true
            };

            _enrollments.Add(enrollment);

            // Update navigation properties
            if (student.Enrollments == null) student.Enrollments = new List<Enrollment>();
            student.Enrollments.Add(enrollment);

            if (course.Enrollments == null) course.Enrollments = new List<Enrollment>();
            course.Enrollments.Add(enrollment);

            return true;
        }

        private async Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid courseId)
        {
            await Task.Delay(50);
            return _enrollments.Any(e =>
                e.StudentId == studentId && e.CourseId == courseId && e.IsActive);
        }

        public async Task<bool> CanDeleteStudentAsync(Guid studentId)
        {
            var student = await _studentService.GetStudentByIdAsync(studentId);
            if (student == null) return false;

            var enrolledCourses = await GetStudentEnrolledCoursesAsync(studentId);
            return !enrolledCourses.Any();
        }

        public async Task<int> GetStudentEnrollmentCountAsync(Guid studentId)
        {
            var enrolledCourses = await GetStudentEnrolledCoursesAsync(studentId);
            return enrolledCourses.Count;
        }

        public async Task<List<Course>> GetStudentEnrolledCoursesAsync(Guid studentId)
        {
            await Task.Delay(50);
            var enrollments = _enrollments.Where(e => e.StudentId == studentId && e.IsActive);
            var courses = new List<Course>();

            foreach (var enrollment in enrollments)
            {
                var course = await _courseService.GetCourseByIdAsync(enrollment.CourseId);
                if (course != null)
                    courses.Add(course);
            }

            return courses.OrderBy(c => c.Code).ToList();
        }
    }
}