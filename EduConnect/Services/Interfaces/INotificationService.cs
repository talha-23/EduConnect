using EduConnect.Models;

namespace EduConnect.Services.Interfaces
{
    public interface INotificationService
    {
        // Events for real-time updates
        event Action<Notification> OnNewNotification;

        // Create notifications
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task SendRoleBasedNotificationAsync(string title, string message, string role, string priority = "info");
        Task SendUserSpecificNotificationAsync(Guid userId, string title, string message, string priority = "info");
        Task SendBulkNotificationAsync(List<Guid> userIds, string title, string message, string priority = "info");

        // Auto notifications from system events
        Task NotifyGradeSubmittedAsync(Guid studentId, Guid courseId, double marks, string letterGrade);
        Task NotifyEnrollmentAsync(Guid studentId, Guid courseId);
        Task NotifyCourseDropAsync(Guid studentId, Guid courseId);

        // Retrieve notifications
        Task<List<Notification>> GetUserNotificationsAsync(Guid userId);
        Task<List<Notification>> GetRoleNotificationsAsync(string role);
        Task<List<Notification>> GetUnreadNotificationsAsync(Guid userId);
        Task<int> GetUnreadCountAsync(Guid userId);

        // Manage notifications
        Task MarkAsReadAsync(Guid notificationId);
        Task MarkAllAsReadAsync(Guid userId);
        Task<bool> DeleteNotificationAsync(Guid notificationId);
        Task ClearOldNotificationsAsync(int daysOld = 30);
    }
}