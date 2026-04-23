using EduConnect.Models;

namespace EduConnect.Services.Interfaces
{
    public interface IStudentService
    {
        Task<List<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByIdAsync(Guid id);
        Task<Student?> GetStudentByEmailAsync(string email);
        Task<List<Student>> SearchStudentsAsync(string searchTerm);
        Task<Student> AddStudentAsync(Student student);
        Task<Student> UpdateStudentAsync(Student student);
        Task<bool> DeleteStudentAsync(Guid id);
        Task<bool> CanDeleteStudentAsync(Guid id);
        Task<int> GetTotalStudentsCountAsync();
    }
}