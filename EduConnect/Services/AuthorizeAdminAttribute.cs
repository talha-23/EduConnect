// Create a new file: Authorization/AuthorizeAdminAttribute.cs
using Microsoft.AspNetCore.Authorization;

namespace EduConnect.Authorization
{
    public class AuthorizeAdminAttribute : AuthorizeAttribute
    {
        public AuthorizeAdminAttribute()
        {
            Policy = "AdminOnly";
        }
    }

    public class AuthorizeStudentAttribute : AuthorizeAttribute
    {
        public AuthorizeStudentAttribute()
        {
            Policy = "StudentOnly";
        }
    }

    public class AuthorizeFacultyAttribute : AuthorizeAttribute
    {
        public AuthorizeFacultyAttribute()
        {
            Policy = "FacultyOnly";
        }
    }
}