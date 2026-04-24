using EduConnect.Models;
using EduConnect.Services.Interfaces;

namespace EduConnect.Services
{
    public class CourseService : ICourseService
    {
        private static List<Course> _courses = new List<Course>();

        public CourseService()
        {
            // Initialize with sample data if empty
            if (!_courses.Any())
            {
                InitializeSampleData();
            }
        }

        private void InitializeSampleData()
        {
            _courses.Add(new Course
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Code = "CS101",
                Title = "Introduction to Programming",
                CreditHours = 3,
                MaxCapacity = 30,
                CurrentEnrollment = 0,
                Description = "Basic programming concepts using C#",
                Instructor = "Dr. Chohan",
                Enrollments = new List<Enrollment>()
            });

            _courses.Add(new Course
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Code = "CS201",
                Title = "Data Structures",
                CreditHours = 3,
                MaxCapacity = 25,
                CurrentEnrollment = 0,
                Description = "Advanced data structures and algorithms",
                Instructor = "Dr. Farooq",
                Enrollments = new List<Enrollment>()
            });

            _courses.Add(new Course
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Code = "CS301",
                Title = "Database Systems",
                CreditHours = 3,
                MaxCapacity = 30,
                CurrentEnrollment = 0,
                Description = "Database design and SQL",
                Instructor = "Prof. Irfan",
                Enrollments = new List<Enrollment>()
            });

            _courses.Add(new Course
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Code = "CS401",
                Title = "Web Development",
                CreditHours = 3,
                MaxCapacity = 28,
                CurrentEnrollment = 0,
                Description = "Modern web development with Blazor",
                Instructor = "Dr. Rashid",
                Enrollments = new List<Enrollment>()
            });
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            await Task.Delay(50);
            return _courses.OrderBy(c => c.Code).ToList();
        }

        public async Task<Course?> GetCourseByIdAsync(Guid id)
        {
            await Task.Delay(50);
            return _courses.FirstOrDefault(c => c.Id == id);
        }

        public async Task<List<Course>> GetAvailableCoursesAsync()
        {
            await Task.Delay(50);
            return _courses.Where(c => !c.IsFull).OrderBy(c => c.Code).ToList();
        }

        public async Task<Course> AddCourseAsync(Course course)
        {
            await Task.Delay(100);
            course.Id = Guid.NewGuid();
            course.CurrentEnrollment = 0;
            course.Enrollments = new List<Enrollment>();
            _courses.Add(course);
            return course;
        }

        public async Task<Course> UpdateCourseAsync(Course course)
        {
            await Task.Delay(100);
            var existing = await GetCourseByIdAsync(course.Id);
            if (existing != null)
            {
                existing.Code = course.Code;
                existing.Title = course.Title;
                existing.CreditHours = course.CreditHours;
                existing.MaxCapacity = course.MaxCapacity;
                existing.Description = course.Description;
                existing.Instructor = course.Instructor;
            }
            return existing ?? course;
        }

        public async Task<bool> DeleteCourseAsync(Guid id)
        {
            await Task.Delay(100);
            var course = await GetCourseByIdAsync(id);
            if (course != null && course.CurrentEnrollment == 0)
            {
                return _courses.Remove(course);
            }
            return false;
        }

        public async Task<bool> CanDeleteCourseAsync(Guid id)
        {
            var course = await GetCourseByIdAsync(id);
            return course != null && course.CurrentEnrollment == 0;
        }

        public async Task<int> GetTotalCoursesCountAsync()
        {
            await Task.Delay(50);
            return _courses.Count;
        }

        // Method to increment enrollment count (called from EnrollmentService)
        public void IncrementEnrollmentCount(Guid courseId)
        {
            var course = _courses.FirstOrDefault(c => c.Id == courseId);
            if (course != null)
            {
                course.CurrentEnrollment++;
                Console.WriteLine($"Course {course.Code} enrollment increased to {course.CurrentEnrollment}");
            }
        }

        // Method to decrement enrollment count (called from EnrollmentService)
        public void DecrementEnrollmentCount(Guid courseId)
        {
            var course = _courses.FirstOrDefault(c => c.Id == courseId);
            if (course != null && course.CurrentEnrollment > 0)
            {
                course.CurrentEnrollment--;
                Console.WriteLine($"Course {course.Code} enrollment decreased to {course.CurrentEnrollment}");
            }
        }

        // Method to add enrollment to course
        public void AddEnrollmentToCourse(Guid courseId, Enrollment enrollment)
        {
            var course = _courses.FirstOrDefault(c => c.Id == courseId);
            if (course != null)
            {
                if (course.Enrollments == null)
                    course.Enrollments = new List<Enrollment>();

                course.Enrollments.Add(enrollment);
                course.CurrentEnrollment = course.Enrollments.Count(e => e.IsActive);
                Console.WriteLine($"Enrollment added to course {course.Code}. Total: {course.CurrentEnrollment}");
            }
        }

        // Method to remove enrollment from course
        public void RemoveEnrollmentFromCourse(Guid courseId, Guid enrollmentId)
        {
            var course = _courses.FirstOrDefault(c => c.Id == courseId);
            if (course != null && course.Enrollments != null)
            {
                var enrollment = course.Enrollments.FirstOrDefault(e => e.Id == enrollmentId);
                if (enrollment != null)
                {
                    enrollment.IsActive = false;
                    course.CurrentEnrollment = course.Enrollments.Count(e => e.IsActive);
                    Console.WriteLine($"Enrollment removed from course {course.Code}. Total: {course.CurrentEnrollment}");
                }
            }
        }
    }
}