using EduConnect.Models;

namespace EduConnect.Services.Interfaces
{
    /// <summary>
    /// Interface for authentication state management
    /// Following SOLID: Interface Segregation Principle - only auth-related methods
    /// </summary>
    public interface IAuthStateService
    {
        // Events for reactive UI updates
        event Action? OnLogin;
        event Action? OnLogout;

        // Properties
        Person? CurrentUser { get; }
        bool IsAuthenticated { get; }

        // Methods
        Task<bool> LoginAsync(string email, string password);
        void Logout();
    }
}