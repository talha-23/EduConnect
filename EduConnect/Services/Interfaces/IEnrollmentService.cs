using EduConnect.Models;

namespace EduConnect.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<bool> EnrollStudentAsync(Guid studentId, Guid courseId);
        Task<bool> DropCourseAsync(Guid studentId, Guid courseId);
        Task<List<Course>> GetStudentEnrolledCoursesAsync(Guid studentId);
        Task<List<Course>> GetAvailableCoursesForStudentAsync(Guid studentId);
        Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid courseId);
        Task<int> GetStudentTotalCreditsAsync(Guid studentId);
        Task<bool> HasStudentDroppedCourseBeforeAsync(Guid studentId, Guid courseId);
    }
}