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
            // Student 1
            var student1 = new Student
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                FullName = "BABER AZAM",
                Email = "baber@educonnect.com",
                Password = "baber123",
                StudentId = "STU001",
                Semester = 3,
                CGPA = 3.8,
                Department = "Computer Science",
                Enrollments = new List<Enrollment>()
            };
            _students.Add(student1);

            // Student 2
            var student2 = new Student
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                FullName = "ALLAH DITA",
                Email = "dita@educonnect.com",
                Password = "dita123",
                StudentId = "STU002",
                Semester = 2,
                CGPA = 3.2,
                Department = "Software Engineering",
                Enrollments = new List<Enrollment>()
            };
            _students.Add(student2);

            // Student 3
            var student3 = new Student
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                FullName = "MIA ASLAM",
                Email = "aslam@educonnect.com",
                Password = "aslam123",
                StudentId = "STU003",
                Semester = 5,
                CGPA = 3.9,
                Department = "Computer Science",
                Enrollments = new List<Enrollment>()
            };
            _students.Add(student3);

            // Student 4 - The one that was logging in (ABDUL MOEEZ RAZA KAZMI)
            var student4 = new Student
            {
                Id = Guid.Parse("537bdf6b-06e0-45cc-aec8-5512168ae5bb"),
                FullName = "ABDUL MOEEZ RAZA KAZMI",
                Email = "student@educonnect.com",
                Password = "student123",
                StudentId = "STU004",
                Semester = 3,
                CGPA = 3.5,
                Department = "Computer Science",
                Enrollments = new List<Enrollment>()
            };
            _students.Add(student4);

            Console.WriteLine($"StudentService initialized with {_students.Count} students");
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            await Task.Delay(100);
            return _students.OrderBy(s => s.FullName).ToList();
        }

        public async Task<Student?> GetStudentByIdAsync(Guid id)
        {
            await Task.Delay(50);
            var student = _students.FirstOrDefault(s => s.Id == id);
            Console.WriteLine($"Looking for student with ID: {id} - Found: {(student != null ? student.FullName : "Not found")}");
            return student;
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
            student.Enrollments = new List<Enrollment>();
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
            var student = await GetStudentByIdAsync(id);
            if (student == null) return false;

            // Check if student has any active enrollments using the Enrollments collection
            bool hasEnrollments = student.Enrollments != null && student.Enrollments.Any(e => e.IsActive);

            return !hasEnrollments;
        }

        public async Task<int> GetTotalStudentsCountAsync()
        {
            await Task.Delay(50);
            return _students.Count;
        }

        // Method to add enrollment to student (called from EnrollmentService)
        public void AddEnrollmentToStudent(Guid studentId, Enrollment enrollment)
        {
            var student = _students.FirstOrDefault(s => s.Id == studentId);
            if (student != null)
            {
                if (student.Enrollments == null)
                    student.Enrollments = new List<Enrollment>();

                student.Enrollments.Add(enrollment);
                Console.WriteLine($"Enrollment added to student {student.FullName}");
            }
        }

        // Method to remove enrollment from student (called from EnrollmentService)
        public void RemoveEnrollmentFromStudent(Guid studentId, Guid enrollmentId)
        {
            var student = _students.FirstOrDefault(s => s.Id == studentId);
            if (student != null && student.Enrollments != null)
            {
                var enrollment = student.Enrollments.FirstOrDefault(e => e.Id == enrollmentId);
                if (enrollment != null)
                {
                    enrollment.IsActive = false;
                    Console.WriteLine($"Enrollment removed from student {student.FullName}");
                }
            }
        }
    }
}