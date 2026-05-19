using EduConnect.Data;
using EduConnect.Models;
using EduConnect.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Services
{
    public class CourseService : ICourseService
    {
        private readonly AppDbContext _context;

        public CourseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(Guid id)
        {
            return await _context.Courses.FindAsync(id);
        }

        public async Task<List<Course>> GetAvailableCoursesAsync()
        {
            return await _context.Courses
                .Where(c => c.CurrentEnrollment < c.MaxCapacity)
                .OrderBy(c => c.Code)
                .ToListAsync();
        }

        public async Task<Course> AddCourseAsync(Course course)
        {
            course.Id = Guid.NewGuid();
            course.CurrentEnrollment = 0;
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course> UpdateCourseAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<bool> DeleteCourseAsync(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null && course.CurrentEnrollment == 0)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> CanDeleteCourseAsync(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            return course != null && course.CurrentEnrollment == 0;
        }

        public async Task<int> GetTotalCoursesCountAsync()
        {
            return await _context.Courses.CountAsync();
        }

        // Method to increment enrollment count
        public async Task IncrementEnrollmentCount(Guid courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course != null)
            {
                course.CurrentEnrollment++;
                await _context.SaveChangesAsync();
            }
        }

        // Method to decrement enrollment count
        public async Task DecrementEnrollmentCount(Guid courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course != null && course.CurrentEnrollment > 0)
            {
                course.CurrentEnrollment--;
                await _context.SaveChangesAsync();
            }
        }
    }
}