using EduConnect.Models;

namespace EduConnect.Services.Interfaces
{
    public interface IStudentEnrollmentValidator
    {
        Task<bool> CanDeleteStudentAsync(Guid studentId);
        Task<int> GetStudentEnrollmentCountAsync(Guid studentId);
        Task<List<Course>> GetStudentEnrolledCoursesAsync(Guid studentId);
    }
}