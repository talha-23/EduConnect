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
                Id = Guid.NewGuid(),
                Code = "CS101",
                Title = "Introduction to Programming",
                CreditHours = 3,
                MaxCapacity = 30,
                Description = "Basic programming concepts using C#",
                Instructor = "Dr. Smith"
            });

            _courses.Add(new Course
            {
                Id = Guid.NewGuid(),
                Code = "CS201",
                Title = "Data Structures",
                CreditHours = 3,
                MaxCapacity = 25,
                Description = "Advanced data structures and algorithms",
                Instructor = "Dr. Johnson"
            });

            _courses.Add(new Course
            {
                Id = Guid.NewGuid(),
                Code = "CS301",
                Title = "Database Systems",
                CreditHours = 3,
                MaxCapacity = 30,
                Description = "Database design and SQL",
                Instructor = "Prof. Williams"
            });

            _courses.Add(new Course
            {
                Id = Guid.NewGuid(),
                Code = "CS401",
                Title = "Web Development",
                CreditHours = 3,
                MaxCapacity = 28,
                Description = "Modern web development with Blazor",
                Instructor = "Dr. Brown"
            });
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            await Task.Delay(100);
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
    }
}