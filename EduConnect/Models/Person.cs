using System;

namespace EduConnect.Models
{
    /// <summary>
    /// Base class for all user types in the system
    /// Following SOLID: Open/Closed Principle - open for extension, closed for modification
    /// </summary>
    public abstract class Person
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Abstract method - forces each derived class to implement its own role
        public abstract string GetRole();

        // Virtual method - can be overridden if needed
        public virtual string GetDisplayInfo()
        {
            return $"{FullName} ({GetRole()})";
        }
    }
}