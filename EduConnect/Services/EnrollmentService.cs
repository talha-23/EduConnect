using EduConnect.Models;
using EduConnect.Services.Interfaces;

namespace EduConnect.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private static List<Enrollment> _enrollments = new List<Enrollment>();
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly INotificationService _notificationService;
        
        public EnrollmentService(
            IStudentService studentService, 
            ICourseService courseService,
            INotificationService notificationService)
        {
            _studentService = studentService;
            _courseService = courseService;
            _notificationService = notificationService;
        }
        
        public async Task<bool> EnrollStudentAsync(Guid studentId, Guid courseId)
        {
            try
            {
                Console.WriteLine($"=== ENROLLMENT START ===");
                
                var student = await _studentService.GetStudentByIdAsync(studentId);
                var course = await _courseService.GetCourseByIdAsync(courseId);
                
                if (student == null) throw new Exception("Student not found");
                if (course == null) throw new Exception("Course not found");
                
                if (course.IsFull) throw new Exception("Course is full");
                
                if (await IsStudentEnrolledAsync(studentId, courseId))
                    throw new Exception("Already enrolled");
                
                // Create enrollment
                var enrollment = new Enrollment
                {
                    Id = Guid.NewGuid(),
                    StudentId = studentId,
                    CourseId = courseId,
                    EnrollmentDate = DateTime.Now,
                    IsActive = true
                };
                
                _enrollments.Add(enrollment);
                
                // CRITICAL: Update course enrollment count via CourseService
                var courseService = _courseService as CourseService;
                if (courseService != null)
                {
                    courseService.IncrementEnrollmentCount(courseId);
                    courseService.AddEnrollmentToCourse(courseId, enrollment);
                }
                
                // Also update the course object's CurrentEnrollment directly
                course.CurrentEnrollment = _enrollments.Count(e => e.CourseId == courseId && e.IsActive);
                
                Console.WriteLine($"Enrollment successful! Total enrollments: {_enrollments.Count}");
                Console.WriteLine($"Course {course.Code} now has {course.CurrentEnrollment} students");
                
                await _notificationService.NotifyEnrollmentAsync(studentId, courseId);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Enrollment failed: {ex.Message}");
                throw;
            }
        }
        
        public async Task<bool> DropCourseAsync(Guid studentId, Guid courseId)
        {
            var enrollment = _enrollments.FirstOrDefault(e => 
                e.StudentId == studentId && e.CourseId == courseId && e.IsActive);
            
            if (enrollment == null) throw new Exception("Enrollment not found");
            
            enrollment.IsActive = false;
            enrollment.DropDate = DateTime.Now;
            
            var courseService = _courseService as CourseService;
            if (courseService != null)
            {
                courseService.DecrementEnrollmentCount(courseId);
                courseService.RemoveEnrollmentFromCourse(courseId, enrollment.Id);
            }
            
            await _notificationService.NotifyCourseDropAsync(studentId, courseId);
            
            return true;
        }
        
        public async Task<List<Course>> GetStudentEnrolledCoursesAsync(Guid studentId)
        {
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
            
            return allCourses
                .Where(c => !enrolledCourses.Any(ec => ec.Id == c.Id) && 
                           !droppedCourses.Contains(c.Id) && 
                           !c.IsFull)
                .OrderBy(c => c.Code)
                .ToList();
        }
        
        public async Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid courseId)
        {
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
            return _enrollments.Any(e => 
                e.StudentId == studentId && e.CourseId == courseId && !e.IsActive);
        }
        
        public async Task<List<Enrollment>> GetAllEnrollmentsAsync()
        {
            return _enrollments.ToList();
        }
        
        public async Task<List<Student>> GetStudentsByCourseAsync(Guid courseId)
        {
            Console.WriteLine($"GetStudentsByCourseAsync called for course {courseId}");
            Console.WriteLine($"Total enrollments in system: {_enrollments.Count}");
            
            var activeEnrollments = _enrollments.Where(e => e.CourseId == courseId && e.IsActive).ToList();
            Console.WriteLine($"Found {activeEnrollments.Count} active enrollments");
            
            var students = new List<Student>();
            
            foreach (var enrollment in activeEnrollments)
            {
                var student = await _studentService.GetStudentByIdAsync(enrollment.StudentId);
                if (student != null)
                {
                    students.Add(student);
                    Console.WriteLine($"Added student: {student.FullName}");
                }
            }
            
            Console.WriteLine($"Returning {students.Count} students");
            return students;
        }
        
        public async Task<int> GetEnrollmentCountByCourseAsync(Guid courseId)
        {
            return _enrollments.Count(e => e.CourseId == courseId && e.IsActive);
        }
    }
}