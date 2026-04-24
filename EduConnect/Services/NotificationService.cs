using EduConnect.Models;
using EduConnect.Services.Interfaces;

namespace EduConnect.Services
{
    public class NotificationService : INotificationService
    {
        private static List<Notification> _notifications = new List<Notification>();
        private readonly ICourseService _courseService;

        public event Action<Notification>? OnNewNotification;

        public NotificationService(ICourseService courseService)
        {
            _courseService = courseService;

            // Initialize sample notifications
            if (!_notifications.Any())
            {
                InitializeSampleNotifications();
            }
        }

        private void InitializeSampleNotifications()
        {
            _notifications.Add(new Notification
            {
                Id = Guid.NewGuid(),
                Title = "Welcome to EduConnect!",
                Message = "Welcome to the university academic portal. Explore your dashboard to get started.",
                UserRole = "All",
                CreatedAt = DateTime.Now.AddDays(-5),
                IsRead = false,
                Priority = "success",
                NotificationType = "Announcement"
            });

            _notifications.Add(new Notification
            {
                Id = Guid.NewGuid(),
                Title = "Mid-Term Exams Schedule",
                Message = "Mid-term examinations will start from December 15th. Please check your course schedule.",
                UserRole = "Student",
                CreatedAt = DateTime.Now.AddDays(-3),
                IsRead = false,
                Priority = "warning",
                NotificationType = "Announcement"
            });

            _notifications.Add(new Notification
            {
                Id = Guid.NewGuid(),
                Title = "Faculty Meeting",
                Message = "There will be a faculty meeting on Friday at 2:00 PM in the conference room.",
                UserRole = "Faculty",
                CreatedAt = DateTime.Now.AddDays(-2),
                IsRead = false,
                Priority = "info",
                NotificationType = "Announcement"
            });
        }

        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            await Task.Delay(50);
            notification.Id = Guid.NewGuid();
            notification.CreatedAt = DateTime.Now;
            notification.IsRead = false;

            _notifications.Add(notification);
            Console.WriteLine($"Notification created: {notification.Title} for role {notification.UserRole}");

            // Fire event for real-time updates
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
                NotificationType = "Announcement",
                CreatedAt = DateTime.Now
            };

            await CreateNotificationAsync(notification);
            Console.WriteLine($"Role-based notification sent to: {role}");
        }

        public async Task SendUserSpecificNotificationAsync(Guid userId, string title, string message, string priority = "info")
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                UserId = userId,
                Priority = priority,
                NotificationType = "Announcement",
                CreatedAt = DateTime.Now
            };

            await CreateNotificationAsync(notification);
            Console.WriteLine($"User-specific notification sent to: {userId}");
        }

        public async Task SendBulkNotificationAsync(List<Guid> userIds, string title, string message, string priority = "info")
        {
            foreach (var userId in userIds)
            {
                await SendUserSpecificNotificationAsync(userId, title, message, priority);
            }
            Console.WriteLine($"Bulk notification sent to {userIds.Count} users");
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
                NotificationType = "GradePosted",
                CreatedAt = DateTime.Now
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
                NotificationType = "Enrollment",
                CreatedAt = DateTime.Now
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
                NotificationType = "Enrollment",
                CreatedAt = DateTime.Now
            };

            await CreateNotificationAsync(notification);
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(Guid userId)
        {
            await Task.Delay(50);
            var result = _notifications
                .Where(n => n.UserId == userId || n.UserRole == "All")
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            Console.WriteLine($"Found {result.Count} notifications for user {userId}");
            return result;
        }

        public async Task<List<Notification>> GetRoleNotificationsAsync(string role)
        {
            await Task.Delay(50);
            return _notifications
                .Where(n => n.UserRole == role || n.UserRole == "All")
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync(Guid userId)
        {
            var notifications = await GetUserNotificationsAsync(userId);
            return notifications.Where(n => !n.IsRead).ToList();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            var unread = await GetUnreadNotificationsAsync(userId);
            return unread.Count;
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            await Task.Delay(50);
            var notification = _notifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.Now;
                Console.WriteLine($"Notification {notificationId} marked as read");
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
            Console.WriteLine($"All notifications marked as read for user {userId}");
        }

        public async Task<bool> DeleteNotificationAsync(Guid notificationId)
        {
            await Task.Delay(50);
            var notification = _notifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification != null)
            {
                return _notifications.Remove(notification);
            }
            return false;
        }

        public async Task ClearOldNotificationsAsync(int daysOld = 30)
        {
            await Task.Delay(100);
            var cutoffDate = DateTime.Now.AddDays(-daysOld);
            _notifications.RemoveAll(n => n.CreatedAt < cutoffDate && n.IsRead);
        }
    }
}