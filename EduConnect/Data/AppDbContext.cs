using EduConnect.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<GradeRecord> GradeRecords { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Use double (no precision needed for float)
            modelBuilder.Entity<Student>()
                .Property(s => s.CGPA)
                .HasColumnType("float");

            modelBuilder.Entity<GradeRecord>()
                .Property(g => g.Marks)
                .HasColumnType("float");

            modelBuilder.Entity<GradeRecord>()
                .Property(g => g.GradePoints)
                .HasColumnType("float");

            // Relationships
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);

            base.OnModelCreating(modelBuilder);
        }
    }
}