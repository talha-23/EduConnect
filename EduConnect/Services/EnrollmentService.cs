using EduConnect.Data;
using EduConnect.Models;
using EduConnect.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly AppDbContext _context;
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly INotificationService _notificationService;

        public EnrollmentService(
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

        public async Task<bool> EnrollStudentAsync(Guid studentId, Guid courseId)
        {
            var student = await _studentService.GetStudentByIdAsync(studentId);
            var course = await _courseService.GetCourseByIdAsync(courseId);

            if (student == null) throw new Exception("Student not found");
            if (course == null) throw new Exception("Course not found");
            if (course.CurrentEnrollment >= course.MaxCapacity) throw new Exception("Course is full");

            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId && e.IsActive);

            if (existingEnrollment != null) throw new Exception("Already enrolled");

            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.Now,
                IsActive = true
            };

            _context.Enrollments.Add(enrollment);

            // Update course enrollment count
            course.CurrentEnrollment++;
            _context.Courses.Update(course);

            await _context.SaveChangesAsync();

            await _notificationService.NotifyEnrollmentAsync(studentId, courseId);
            return true;
        }

        public async Task<bool> DropCourseAsync(Guid studentId, Guid courseId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId && e.IsActive);

            if (enrollment == null) throw new Exception("Enrollment not found");

            enrollment.IsActive = false;
            enrollment.DropDate = DateTime.Now;

            // Update course enrollment count
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course != null && course.CurrentEnrollment > 0)
            {
                course.CurrentEnrollment--;
                _context.Courses.Update(course);
            }

            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();

            await _notificationService.NotifyCourseDropAsync(studentId, courseId);
            return true;
        }

        public async Task<List<Course>> GetStudentEnrolledCoursesAsync(Guid studentId)
        {
            var enrollments = await _context.Enrollments
                .Where(e => e.StudentId == studentId && e.IsActive)
                .Include(e => e.Course)
                .ToListAsync();

            return enrollments.Select(e => e.Course!).Where(c => c != null).OrderBy(c => c.Code).ToList();
        }

        public async Task<List<Course>> GetAvailableCoursesForStudentAsync(Guid studentId)
        {
            var allCourses = await _courseService.GetAllCoursesAsync();
            var enrolledCourses = await GetStudentEnrolledCoursesAsync(studentId);
            var droppedCourses = await _context.Enrollments
                .Where(e => e.StudentId == studentId && !e.IsActive)
                .Select(e => e.CourseId)
                .ToListAsync();

            return allCourses
                .Where(c => !enrolledCourses.Any(ec => ec.Id == c.Id) &&
                           !droppedCourses.Contains(c.Id) &&
                           c.CurrentEnrollment < c.MaxCapacity)
                .OrderBy(c => c.Code)
                .ToList();
        }

        public async Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid courseId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId && e.IsActive);
        }

        public async Task<int> GetStudentTotalCreditsAsync(Guid studentId)
        {
            var enrolledCourses = await GetStudentEnrolledCoursesAsync(studentId);
            return enrolledCourses.Sum(c => c.CreditHours);
        }

        public async Task<bool> HasStudentDroppedCourseBeforeAsync(Guid studentId, Guid courseId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId && !e.IsActive);
        }

        public async Task<List<Enrollment>> GetAllEnrollmentsAsync()
        {
            return await _context.Enrollments.ToListAsync();
        }

        public async Task<List<Student>> GetStudentsByCourseAsync(Guid courseId)
        {
            var enrollments = await _context.Enrollments
                .Where(e => e.CourseId == courseId && e.IsActive)
                .Include(e => e.Student)
                .ToListAsync();

            return enrollments.Select(e => e.Student!).Where(s => s != null).ToList();
        }

        public async Task<int> GetEnrollmentCountByCourseAsync(Guid courseId)
        {
            return await _context.Enrollments.CountAsync(e => e.CourseId == courseId && e.IsActive);
        }
    }
}