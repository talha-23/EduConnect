using EduConnect.Data;
using EduConnect.Models;
using EduConnect.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;
        private readonly ICourseService _courseService;

        public event Action<Notification>? OnNewNotification;

        public NotificationService(AppDbContext context, ICourseService courseService)
        {
            _context = context;
            _courseService = courseService;
        }

        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            notification.Id = Guid.NewGuid();
            notification.CreatedAt = DateTime.Now;
            notification.IsRead = false;

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            OnNewNotification?.Invoke(notification);
            return notification;
        }

        public async Task SendRoleBasedNotificationAsync(string title, string message, string role, string priority = "info")
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                UserRole = role,
                Priority = priority,
                NotificationType = "Announcement"
            };
            await CreateNotificationAsync(notification);
        }

        public async Task SendUserSpecificNotificationAsync(Guid userId, string title, string message, string priority = "info")
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                UserId = userId,
                Priority = priority,
                NotificationType = "Announcement"
            };
            await CreateNotificationAsync(notification);
        }

        public async Task SendBulkNotificationAsync(List<Guid> userIds, string title, string message, string priority = "info")
        {
            foreach (var userId in userIds)
            {
                await SendUserSpecificNotificationAsync(userId, title, message, priority);
            }
        }

        public async Task NotifyGradeSubmittedAsync(Guid studentId, Guid courseId, double marks, string letterGrade)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            var courseName = course?.Title ?? "a course";

            var notification = new Notification
            {
                Title = "Grade Posted",
                Message = $"Your grade for {courseName} has been posted. You received {marks} marks ({letterGrade}).",
                UserId = studentId,
                Priority = marks >= 70 ? "success" : (marks >= 45 ? "info" : "warning"),
                NotificationType = "GradePosted"
            };
            await CreateNotificationAsync(notification);
        }

        public async Task NotifyEnrollmentAsync(Guid studentId, Guid courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            var courseName = course?.Title ?? "a course";

            var notification = new Notification
            {
                Title = "Enrollment Confirmed",
                Message = $"You have successfully enrolled in {courseName}. Welcome to the course!",
                UserId = studentId,
                Priority = "success",
                NotificationType = "Enrollment"
            };
            await CreateNotificationAsync(notification);
        }

        public async Task NotifyCourseDropAsync(Guid studentId, Guid courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            var courseName = course?.Title ?? "a course";

            var notification = new Notification
            {
                Title = "Course Dropped",
                Message = $"You have dropped {courseName}. The course has been removed from your schedule.",
                UserId = studentId,
                Priority = "warning",
                NotificationType = "Enrollment"
            };
            await CreateNotificationAsync(notification);
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(Guid userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId || n.UserRole == "All")
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetRoleNotificationsAsync(string role)
        {
            return await _context.Notifications
                .Where(n => n.UserRole == role || n.UserRole == "All")
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync(Guid userId)
        {
            var notifications = await GetUserNotificationsAsync(userId);
            return notifications.Where(n => !n.IsRead).ToList();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _context.Notifications
                .CountAsync(n => (n.UserId == userId || n.UserRole == "All") && !n.IsRead);
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var notifications = await GetUserNotificationsAsync(userId);
            foreach (var notification in notifications.Where(n => !n.IsRead))
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.Now;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteNotificationAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task ClearOldNotificationsAsync(int daysOld = 30)
        {
            var cutoffDate = DateTime.Now.AddDays(-daysOld);
            var oldNotifications = _context.Notifications.Where(n => n.CreatedAt < cutoffDate && n.IsRead);
            _context.Notifications.RemoveRange(oldNotifications);
            await _context.SaveChangesAsync();
        }
    }
}