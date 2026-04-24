using System;

namespace EduConnect.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty; // Admin, Faculty, Student, All
        public Guid? UserId { get; set; } // Specific user or null for role-based
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public string Priority { get; set; } = "info"; // info, success, warning, danger
        public string NotificationType { get; set; } = "Announcement"; // Announcement, GradePosted, Enrollment

        // Helper properties
        public bool IsUnread => !IsRead;
        public string TimeAgo => GetTimeAgo();

        private string GetTimeAgo()
        {
            var timeSpan = DateTime.Now - CreatedAt;

            if (timeSpan.TotalMinutes < 1)
                return "Just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{timeSpan.Minutes} minute(s) ago";
            if (timeSpan.TotalHours < 24)
                return $"{timeSpan.Hours} hour(s) ago";
            if (timeSpan.TotalDays < 7)
                return $"{timeSpan.Days} day(s) ago";
            if (timeSpan.TotalDays < 30)
                return $"{timeSpan.TotalDays / 7:F0} week(s) ago";

            return CreatedAt.ToString("MMM dd, yyyy");
        }

        public string GetPriorityClass()
        {
            return Priority switch
            {
                "danger" => "bg-danger",
                "warning" => "bg-warning",
                "success" => "bg-success",
                _ => "bg-info"
            };
        }

        public string GetIcon()
        {
            return NotificationType switch
            {
                "GradePosted" => "bi-trophy",
                "Enrollment" => "bi-journal-check",
                "Announcement" => "bi-megaphone",
                _ => "bi-bell"
            };
        }
    }
}