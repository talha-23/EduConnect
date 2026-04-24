using EduConnect.Models;
using EduConnect.Services.Interfaces;

namespace EduConnect.Services
{
    public class AuthStateService : IAuthStateService
    {
        private Person? _currentUser;

        public event Action? OnLogin;
        public event Action? OnLogout;

        public Person? CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;

        // In-memory user store with IDs that match StudentService
        private readonly List<Person> _users = new List<Person>();

        public AuthStateService()
        {
            // Create Admin user
            var admin = new Admin
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                FullName = "Muhammad Talha",
                Email = "admin@educonnect.com",
                Password = "admin123",
                AdminLevel = "Super Admin"
            };

            // Create Faculty user
            var faculty = new Faculty
            {
                Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                FullName = "Dr. Insha Khan",
                Email = "faculty@educonnect.com",
                Password = "faculty123",
                Department = "Computer Science",
                Designation = "Professor"
            };

            // Create Student users with same IDs as in StudentService
            var student1 = new Student
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), // Matches StudentService
                FullName = "BABER AZAM",
                Email = "baber@educonnect.com",
                Password = "baber123",
                StudentId = "STU001",
                Semester = 3,
                CGPA = 3.8,
                Department = "Computer Science"
            };

            var student2 = new Student
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), // Matches StudentService
                FullName = "ALLAH DITA",
                Email = "dita@educonnect.com",
                Password = "dita123",
                StudentId = "STU002",
                Semester = 2,
                CGPA = 3.2,
                Department = "Software Engineering"
            };

            var student3 = new Student
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), // Matches StudentService
                FullName = "MIA ASLAM",
                Email = "aslam@educonnect.com",
                Password = "aslam123",
                StudentId = "STU003",
                Semester = 5,
                CGPA = 3.9,
                Department = "Computer Science"
            };
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

            _users.Add(admin);
            _users.Add(faculty);
            _users.Add(student1);
            _users.Add(student2);
            _users.Add(student3);
            _users.Add(student4);

            Console.WriteLine($"AuthStateService initialized with {_users.Count} users");
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                Console.WriteLine($"Attempting login with: {email}");

                // Find user by email and password (case-insensitive)
                var user = _users.FirstOrDefault(u =>
                    u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                    u.Password == password);

                if (user != null)
                {
                    _currentUser = user;
                    Console.WriteLine($"Login SUCCESS for: {user.FullName}, Role: {user.GetRole()}, ID: {user.Id}");
                    OnLogin?.Invoke();
                    return true;
                }

                Console.WriteLine($"Login FAILED for: {email}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return false;
            }
        }

        public void Logout()
        {
            _currentUser = null;
            Console.WriteLine("User logged out");
            OnLogout?.Invoke();
        }
    }
}