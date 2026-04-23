using EduConnect.Models;
using EduConnect.Services.Interfaces;

namespace EduConnect.Services
{
    public class AuthStateService : IAuthStateService
    {
        private Person? _currentUser;

        public event Action? OnLogin;
        public event Action? OnLogout;

        public Person? CurrentUser
        {
            get { return _currentUser; }
        }

        public bool IsAuthenticated
        {
            get { return _currentUser != null; }
        }

        // Hardcoded users for testing
        private readonly List<Person> _users = new List<Person>();

        public AuthStateService()
        {
            // Initialize users in constructor
            _users.Add(new Admin
            {
                Id = Guid.NewGuid(),
                FullName = "MUHAMMAD TALHA",
                Email = "admin@educonnect.com",
                Password = "admin123",
                AdminLevel = "Super Admin"
            });

            _users.Add(new Faculty
            {
                Id = Guid.NewGuid(),
                FullName = "INSHA KHAN",
                Email = "faculty@educonnect.com",
                Password = "faculty123",
                Department = "Computer Science",
                Designation = "Professor"
            });

            _users.Add(new Student
            {
                Id = Guid.NewGuid(),
                FullName = "ABDUL MOEEZ RAZA KAZMI",
                Email = "student@educonnect.com",
                Password = "student123",
                Semester = 3,
                CGPA = 3.5
            });
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            Console.WriteLine($"Attempting login with: {email}");

            var user = _users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password);

            if (user != null)
            {
                _currentUser = user;
                Console.WriteLine($"Login SUCCESS for: {user.FullName}, Role: {user.GetRole()}");
                OnLogin?.Invoke();
                NotifyStateChanged();
                await Task.Yield();
                return true;
                Console.WriteLine($"SERVICE INSTANCE: {this.GetHashCode()}");
                Console.WriteLine($"USER SET: {_currentUser?.Email}");
            }

            Console.WriteLine($"Login FAILED for: {email}");
            return false;
        }
        public event Action? OnChange;

        private void NotifyStateChanged()
        {
            OnChange?.Invoke();
        }

        public void Logout()
        {
            _currentUser = null;
            Console.WriteLine("Logout successful");
            OnLogout?.Invoke();
            NotifyStateChanged();

        }


    }
}