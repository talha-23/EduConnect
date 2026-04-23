using EduConnect.Models;
using EduConnect.Services.Interfaces;

namespace EduConnect.Services
{
    public class StudentService : IStudentService
    {
        private static List<Student> _students = new List<Student>();

        public StudentService()
        {
            // Initialize with sample data if empty
            if (!_students.Any())
            {
                InitializeSampleData();
            }
        }

        private void InitializeSampleData()
        {
            _students.Add(new Student
            {
                Id = Guid.NewGuid(),
                FullName = "Alice Johnson",
                Email = "alice@educonnect.com",
                Password = "password123",
                StudentId = "STU001",
                Semester = 3,
                CGPA = 3.8,
                Department = "Computer Science"
            });

            _students.Add(new Student
            {
                Id = Guid.NewGuid(),
                FullName = "Bob Smith",
                Email = "bob@educonnect.com",
                Password = "password123",
                StudentId = "STU002",
                Semester = 2,
                CGPA = 3.2,
                Department = "Software Engineering"
            });

            _students.Add(new Student
            {
                Id = Guid.NewGuid(),
                FullName = "Carol Davis",
                Email = "carol@educonnect.com",
                Password = "password123",
                StudentId = "STU003",
                Semester = 5,
                CGPA = 3.9,
                Department = "Computer Science"
            });
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            await Task.Delay(100);
            return _students.OrderBy(s => s.FullName).ToList();
        }

        public async Task<Student?> GetStudentByIdAsync(Guid id)
        {
            await Task.Delay(50);
            return _students.FirstOrDefault(s => s.Id == id);
        }

        public async Task<Student?> GetStudentByEmailAsync(string email)
        {
            await Task.Delay(50);
            return _students.FirstOrDefault(s => s.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<List<Student>> SearchStudentsAsync(string searchTerm)
        {
            await Task.Delay(50);
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _students.OrderBy(s => s.FullName).ToList();

            return _students
                .Where(s => s.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           s.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           s.StudentId.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .OrderBy(s => s.FullName)
                .ToList();
        }

        public async Task<Student> AddStudentAsync(Student student)
        {
            await Task.Delay(100);
            student.Id = Guid.NewGuid();
            student.Password = "default123";
            _students.Add(student);
            return student;
        }

        public async Task<Student> UpdateStudentAsync(Student student)
        {
            await Task.Delay(100);
            var existing = await GetStudentByIdAsync(student.Id);
            if (existing != null)
            {
                existing.FullName = student.FullName;
                existing.Email = student.Email;
                existing.Semester = student.Semester;
                existing.CGPA = student.CGPA;
                existing.StudentId = student.StudentId;
                existing.Department = student.Department;
            }
            return existing ?? student;
        }

        public async Task<bool> DeleteStudentAsync(Guid id)
        {
            await Task.Delay(100);
            var student = await GetStudentByIdAsync(id);
            if (student != null)
            {
                return _students.Remove(student);
            }
            return false;
        }

        public async Task<bool> CanDeleteStudentAsync(Guid id)
        {
            // This method should be implemented using IStudentEnrollmentValidator
            // For now, return true (you'll use the validator service instead)
            return true;
        }

        public async Task<int> GetTotalStudentsCountAsync()
        {
            await Task.Delay(50);
            return _students.Count;
        }
    }
}