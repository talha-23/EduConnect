using EduConnect.Data;
using EduConnect.Models;
using EduConnect.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Services
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;

        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(Guid id)
        {
            return await _context.Students.FindAsync(id);
        }

        public async Task<Student?> GetStudentByEmailAsync(string email)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.Email == email);
        }

        public async Task<List<Student>> SearchStudentsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllStudentsAsync();

            return await _context.Students
                .Where(s =>
                    s.FullName.Contains(searchTerm) ||
                    s.Email.Contains(searchTerm) ||
                    s.StudentId.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<Student> AddStudentAsync(Student student)
        {
            student.Id = Guid.NewGuid();
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<Student> UpdateStudentAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<bool> DeleteStudentAsync(Guid id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> CanDeleteStudentAsync(Guid id)
        {
            var enrollments = await _context.Enrollments
                .Where(e => e.StudentId == id && e.IsActive)
                .AnyAsync();
            return !enrollments;
        }

        public async Task<int> GetTotalStudentsCountAsync()
        {
            return await _context.Students.CountAsync();
        }
    }
}