Here's a comprehensive README for your EduConnect project on GitHub:

# 📚 EduConnect - University Academic Portal

[![.NET Version](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-Server-blue)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-purple)](https://getbootstrap.com/)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)

## 🎓 Project Overview

EduConnect is a comprehensive, fully functional university academic portal built with **Blazor Server** and **C#**. The system simulates a real-world academic management platform that universities use to manage students, courses, faculty, grades, and notifications.

This project demonstrates practical application of:
- **Object-Oriented Programming** principles
- **Event-Driven Programming** with C# delegates
- **SOLID Architecture** patterns
- **Blazor** reactive UI through data binding
- **Clean Architecture** with separation of concerns

## ✨ Features Implemented

### Module 1: Authentication & Role-Based Dashboard ✅
- **Multi-role Authentication**: Admin, Faculty, Student roles
- **In-memory Session Management**: No database required
- **Event-driven UI Updates**: Real-time navbar and dashboard updates
- **Role-based Access Control**: Different menus and permissions per role
- **Protected Routes**: Automatic redirect to unauthorized page
- **Demo Accounts**: Pre-configured accounts for testing

## 🚀 Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| Blazor | .NET 8 | Frontend Framework |
| C# | .NET 8 | Backend Language |
| Bootstrap | 5.3 | UI Styling |
| HTML5/CSS3 | - | Markup & Styling |
| JavaScript | ES6 | Interop & Utilities |

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (Recommended) or any C# IDE
- [Git](https://git-scm.com/) (for cloning)

## 🔧 Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/EduConnect.git
cd EduConnect
```

### 2. Open in Visual Studio

```bash
# Double-click the .sln file or open via Visual Studio
start EduConnect.sln
```

### 3. Restore NuGet Packages

```bash
dotnet restore
```

### 4. Build the Solution

```bash
dotnet build
```

### 5. Run the Application

```bash
dotnet run
```

Or press `F5` in Visual Studio.

### 6. Access the Application

Open your browser and navigate to:
```
https://localhost:5001
http://localhost:5000
```

## 🔐 Demo Credentials

Use these pre-configured accounts to test different roles:

| Role | Email | Password |
|------|-------|----------|
| 👑 **Admin** | admin@educonnect.com | admin123 |
| 👨‍🏫 **Faculty** | faculty@educonnect.com | faculty123 |
| 👨‍🎓 **Student** | student@educonnect.com | student123 |

## 📁 Project Structure

```
EduConnect/
├── Components/
│   └── NavBar.razor              # Navigation component
├── Models/
│   ├── Person.cs                 # Base abstract class
│   ├── Student.cs                # Student entity
│   ├── Faculty.cs                # Faculty entity
│   └── Admin.cs                  # Admin entity
├── Services/
│   ├── Interfaces/
│   │   └── IAuthStateService.cs  # Auth service contract
│   └── AuthStateService.cs       # Authentication implementation
├── Pages/
│   ├── Login.razor               # Login page
│   ├── Dashboard.razor           # User dashboard
│   ├── Unauthorized.razor        # Access denied page
│   └── TestAuth.razor            # Debug/testing page
├── Shared/
│   └── MainLayout.razor          # Main layout component
├── App.razor                     # App entry point
├── Program.cs                    # Application configuration
├── _Imports.razor                # Global usings
└── EduConnect.csproj             # Project file
```

## 🏗️ Architecture

### SOLID Principles Implementation

| Principle | Implementation |
|-----------|----------------|
| **Single Responsibility** | Each service has one purpose (AuthService only handles auth) |
| **Open/Closed** | Person abstract class allows new roles without modification |
| **Liskov Substitution** | Student, Faculty, Admin can replace Person anywhere |
| **Interface Segregation** | IAuthStateService contains only auth-related methods |
| **Dependency Inversion** | Components depend on interfaces, not concrete classes |

### Event-Driven Communication

```csharp
// Publisher (AuthStateService)
public event Action? OnLogin;
OnLogin?.Invoke();  // Broadcast login event

// Subscriber (Dashboard)
AuthService.OnLogin += OnAuthChanged;  // Subscribe
private void OnAuthChanged() => StateHasChanged();  // React
```

## 🎯 Features Demonstration

### Role-Based Menu System

- **Admin**: Dashboard, Students, Courses, Reports
- **Faculty**: Dashboard, My Courses, Submit Grades  
- **Student**: Dashboard, Enroll Courses, My Grades

### Data Binding Examples

- **One-Way Binding**: Display user information
- **Two-Way Binding**: Form inputs with `@bind`
- **EventCallback**: Parent-child component communication

### Protected Routes

The application includes route protection:
- Unauthorized access redirects to `/unauthorized`
- Role-based page access control
- Automatic login state persistence

## 🧪 Testing the Application

### Manual Test Plan

1. **Authentication Test**
   - Navigate to `/login`
   - Test each demo account
   - Verify correct role assignment

2. **Navigation Test**
   - Check navbar displays correct menu items per role
   - Test route protection (e.g., try accessing `/admin/students` as student)

3. **Event System Test**
   - Login and observe dashboard updates
   - Use `/test` page to verify event subscription
   - Check browser console for debug output

4. **Logout Test**
   - Click logout button
   - Verify redirect to login page
   - Confirm navbar shows login option

### Debug Page

Access `/test` to:
- View current authentication state
- Test login/logout directly
- Verify event subscriptions

## 📝 Code Examples

### Authentication Service

```csharp
public class AuthStateService : IAuthStateService
{
    public event Action? OnLogin;
    public event Action? OnLogout;
    
    public async Task<bool> LoginAsync(string email, string password)
    {
        // Authentication logic
        OnLogin?.Invoke();  // Notify subscribers
        return true;
    }
}
```

### Component Subscribing to Events

```csharp
@code {
    protected override void OnInitialized()
    {
        AuthService.OnLogin += OnAuthChanged;
        AuthService.OnLogout += OnAuthChanged;
    }
    
    private void OnAuthChanged() => InvokeAsync(StateHasChanged);
    
    public void Dispose()
    {
        AuthService.OnLogin -= OnAuthChanged;
        AuthService.OnLogout -= OnAuthChanged;
    }
}
```

## 🐛 Troubleshooting

### Common Issues & Solutions

| Issue | Solution |
|-------|----------|
| **Authentication state not persisting** | Ensure service is registered as Scoped in Program.cs |
| **UI not updating after login** | Check event subscriptions and StateHasChanged() calls |
| **Bootstrap not loading** | Verify internet connection for CDN links |
| **Navigation not working** | Use forceLoad: true in NavigateTo() |

### Debugging Tips

1. **Check Console Output**: Login/Logout events log to browser console
2. **Use Test Page**: Navigate to `/test` to verify auth state
3. **Clear Browser Cache**: Press Ctrl+Shift+Delete
4. **Rebuild Solution**: Clean → Rebuild in Visual Studio

## 🔄 Future Modules (Planned)

The authentication module is the foundation. Future modules will include:

- 📊 **Student Management** - Full CRUD operations
- 📚 **Course Management** - Course catalog and enrollment
- 📝 **Grading System** - Grade submission and CGPA calculation
- 🔔 **Notification System** - Event-driven notifications

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is for educational purposes as part of the Visual Programming course (CS-284) at Air University Islamabad.

## 👥 Authors

- **Group Members** - *Initial work*
  - Roll No. 1
  - Roll No. 2
  - Roll No. 3

## 🙏 Acknowledgments

- Air University Islamabad - Course Faculty
- Visual Programming Course (CS-284) Spring 2026

## 📞 Support

For issues, questions, or contributions:
- Create an issue in the GitHub repository
- Contact your course instructor
- Refer to the assignment documentation

## 🔗 Quick Links

- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [Blazor Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor)
- [Bootstrap Documentation](https://getbootstrap.com/docs/5.3)

---

## 📸 Screenshots

### Login Page
```
┌─────────────────────────────────────┐
│           EduConnect                 │
│      University Academic Portal      │
├─────────────────────────────────────┤
│  Email: [__________________]         │
│  Password: [__________________]      │
│                                      │
│  [              Login              ] │
│                                      │
│  Demo Accounts:                      │
│  • Admin: admin@educonnect.com      │
│  • Faculty: faculty@educonnect.com  │
│  • Student: student@educonnect.com  │
└─────────────────────────────────────┘
```

### Admin Dashboard
```
┌──────────────────────────────────────────────────┐
│  EduConnect  Dashboard  Students  Courses  Reports │
├──────────────────────────────────────────────────┤
│  ✅ Welcome back, Admin User!                     │
│     You are logged in as Admin                    │
│                                                    │
│  ┌──────────────┐  ┌──────────────┐              │
│  │ Your Info    │  │ Quick Actions│              │
│  │ Name: Admin  │  │ • Students   │              │
│  │ Role: Admin  │  │ • Courses    │              │
│  └──────────────┘  └──────────────┘              │
└──────────────────────────────────────────────────┘
```

---

## ⚡ Performance Considerations

- **Scoped Services**: Auth state is maintained per user session
- **Event Unsubscription**: Prevents memory leaks in long-running applications
- **Lazy Loading**: Components load only when needed
- **Minimal JavaScript**: Most logic in C# for better performance

---

**Built with ❤️ for Air University Islamabad | Spring 2026**
