using EduConnect.Models;

namespace EduConnect.Services.Interfaces
{
    public interface ICourseService
    {
        Task<List<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(Guid id);
        Task<List<Course>> GetAvailableCoursesAsync();
        Task<Course> AddCourseAsync(Course course);
        Task<Course> UpdateCourseAsync(Course course);
        Task<bool> DeleteCourseAsync(Guid id);
        Task<bool> CanDeleteCourseAsync(Guid id);
        Task<int> GetTotalCoursesCountAsync();
    }
}