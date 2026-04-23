using EduConnect.Models;
using EduConnect.Services.Interfaces;

namespace EduConnect.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private static List<Enrollment> _enrollments = new List<Enrollment>();
        private readonly ICourseService _courseService;

        public EnrollmentService(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public async Task<bool> EnrollStudentAsync(Guid studentId, Guid courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);

            if (course == null)
                throw new Exception("Course not found");

            if (course.IsFull)
                throw new Exception("Course is full");

            if (await IsStudentEnrolledAsync(studentId, courseId))
                throw new Exception("Student is already enrolled in this course");

            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.Now,
                IsActive = true
            };

            _enrollments.Add(enrollment);

            return true;
        }

        public async Task<bool> DropCourseAsync(Guid studentId, Guid courseId)
        {
            var enrollment = _enrollments.FirstOrDefault(e =>
                e.StudentId == studentId && e.CourseId == courseId && e.IsActive);

            if (enrollment == null)
                throw new Exception("Enrollment not found");

            enrollment.IsActive = false;
            enrollment.DropDate = DateTime.Now;

            return true;
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

        public async Task<List<Course>> GetAvailableCoursesForStudentAsync(Guid studentId)
        {
            var allCourses = await _courseService.GetAllCoursesAsync();
            var enrolledCourses = await GetStudentEnrolledCoursesAsync(studentId);
            var droppedCourses = _enrollments
                .Where(e => e.StudentId == studentId && !e.IsActive)
                .Select(e => e.CourseId)
                .ToList();

            var availableCourses = allCourses
                .Where(c => !enrolledCourses.Any(ec => ec.Id == c.Id) &&
                           !droppedCourses.Contains(c.Id) &&
                           !c.IsFull)
                .OrderBy(c => c.Code)
                .ToList();

            return availableCourses;
        }

        public async Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid courseId)
        {
            await Task.Delay(50);
            return _enrollments.Any(e =>
                e.StudentId == studentId && e.CourseId == courseId && e.IsActive);
        }

        public async Task<int> GetStudentTotalCreditsAsync(Guid studentId)
        {
            var enrolledCourses = await GetStudentEnrolledCoursesAsync(studentId);
            return enrolledCourses.Sum(c => c.CreditHours);
        }

        public async Task<bool> HasStudentDroppedCourseBeforeAsync(Guid studentId, Guid courseId)
        {
            await Task.Delay(50);
            return _enrollments.Any(e =>
                e.StudentId == studentId && e.CourseId == courseId && !e.IsActive);
        }
    }
}