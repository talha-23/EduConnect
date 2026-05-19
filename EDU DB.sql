-- Drop and recreate database fresh
USE master;
GO

-- Kill any existing connections
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'EduConnectDB')
BEGIN
    ALTER DATABASE EduConnectDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE EduConnectDB;
END
GO

-- Create fresh database
CREATE DATABASE EduConnectDB;
GO

USE EduConnectDB;
GO

-- ============================================
-- 1. USERS TABLE (for authentication)
-- ============================================
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    LastLoginAt DATETIME NULL,
    IsActive BIT DEFAULT 1
);
GO

-- ============================================
-- 2. STUDENTS TABLE
-- ============================================
CREATE TABLE Students (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId NVARCHAR(20) NOT NULL UNIQUE,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    Semester INT NOT NULL,
    CGPA FLOAT NOT NULL,
    Department NVARCHAR(50) NOT NULL
);
GO

-- ============================================
-- 3. FACULTIES TABLE
-- ============================================
CREATE TABLE Faculties (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    Department NVARCHAR(50) NOT NULL,
    Designation NVARCHAR(50) NOT NULL
);
GO

-- ============================================
-- 4. COURSES TABLE
-- ============================================
CREATE TABLE Courses (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Code NVARCHAR(20) NOT NULL UNIQUE,
    Title NVARCHAR(100) NOT NULL,
    CreditHours INT NOT NULL,
    MaxCapacity INT NOT NULL,
    CurrentEnrollment INT NOT NULL DEFAULT 0,
    Description NVARCHAR(500) NULL,
    Instructor NVARCHAR(100) NULL
);
GO

-- ============================================
-- 5. ENROLLMENTS TABLE
-- ============================================
CREATE TABLE Enrollments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    EnrollmentDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    DropDate DATETIME NULL,
    FOREIGN KEY (StudentId) REFERENCES Students(Id) ON DELETE CASCADE,
    FOREIGN KEY (CourseId) REFERENCES Courses(Id) ON DELETE CASCADE
);
GO

-- ============================================
-- 6. GRADE RECORDS TABLE
-- ============================================
CREATE TABLE GradeRecords (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    FacultyId UNIQUEIDENTIFIER NOT NULL,
    Marks FLOAT NOT NULL,
    LetterGrade NVARCHAR(2) NOT NULL,
    GradePoints FLOAT NOT NULL,
    SubmissionDate DATETIME DEFAULT GETDATE(),
    Remarks NVARCHAR(500) NULL,
    FOREIGN KEY (StudentId) REFERENCES Students(Id) ON DELETE CASCADE,
    FOREIGN KEY (CourseId) REFERENCES Courses(Id) ON DELETE CASCADE
);
GO

-- ============================================
-- 7. NOTIFICATIONS TABLE (MISSING ONE!)
-- ============================================
CREATE TABLE Notifications (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Title NVARCHAR(100) NOT NULL,
    Message NVARCHAR(500) NOT NULL,
    UserId UNIQUEIDENTIFIER NULL,
    UserRole NVARCHAR(20) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    IsRead BIT DEFAULT 0,
    ReadAt DATETIME NULL,
    Priority NVARCHAR(20) DEFAULT 'info',
    NotificationType NVARCHAR(30) DEFAULT 'Announcement'
);
GO

-- ============================================
-- INSERT SAMPLE DATA
-- ============================================

-- Insert Users
INSERT INTO Users (Id, FullName, Email, Password, Role) VALUES
(NEWID(), 'Admin User', 'admin@educonnect.com', 'admin123', 'Admin'),
(NEWID(), 'Dr. Insha Khan', 'faculty@educonnect.com', 'faculty123', 'Faculty'),
(NEWID(), 'John Doe', 'student@educonnect.com', 'student123', 'Student');
GO

-- Insert Students (using fixed GUIDs to match AuthStateService)
INSERT INTO Students (Id, StudentId, FullName, Email, Password, Semester, CGPA, Department) VALUES
('11111111-1111-1111-1111-111111111111', 'STU001', 'BABER AZAM', 'baber@educonnect.com', 'baber123', 3, 3.8, 'Computer Science'),
('22222222-2222-2222-2222-222222222222', 'STU002', 'ALLAH DITA', 'dita@educonnect.com', 'dita123', 2, 3.2, 'Software Engineering'),
('33333333-3333-3333-3333-333333333333', 'STU003', 'MIA ASLAM', 'aslam@educonnect.com', 'aslam123', 5, 3.9, 'Computer Science'),
('537bdf6b-06e0-45cc-aec8-5512168ae5bb', 'STU004', 'ABDUL MOEEZ RAZA KAZMI', 'student@educonnect.com', 'student123', 3, 3.5, 'Computer Science');
GO

-- Insert Faculties
INSERT INTO Faculties (Id, FullName, Email, Password, Department, Designation) VALUES
('88888888-8888-8888-8888-888888888888', 'Dr. Insha Khan', 'faculty@educonnect.com', 'faculty123', 'Computer Science', 'Professor');
GO

-- Insert Courses
INSERT INTO Courses (Id, Code, Title, CreditHours, MaxCapacity, CurrentEnrollment, Description, Instructor) VALUES
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'CS101', 'Introduction to Programming', 3, 30, 0, 'Basic programming concepts using C#', 'Dr. Chohan'),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'CS201', 'Data Structures', 3, 25, 0, 'Advanced data structures and algorithms', 'Dr. Farooq'),
('cccccccc-cccc-cccc-cccc-cccccccccccc', 'CS301', 'Database Systems', 3, 30, 0, 'Database design and SQL', 'Prof. Irfan'),
('dddddddd-dddd-dddd-dddd-dddddddddddd', 'CS401', 'Web Development', 3, 28, 0, 'Modern web development with Blazor', 'Dr. Rashid');
GO

-- Insert Sample Notifications
INSERT INTO Notifications (Id, Title, Message, UserRole, Priority, NotificationType) VALUES
(NEWID(), 'Welcome to EduConnect!', 'Welcome to the university academic portal. Explore your dashboard to get started.', 'All', 'success', 'Announcement'),
(NEWID(), 'Mid-Term Exams Schedule', 'Mid-term examinations will start from December 15th. Please check your course schedule.', 'Student', 'warning', 'Announcement'),
(NEWID(), 'Faculty Meeting', 'There will be a faculty meeting on Friday at 2:00 PM in the conference room.', 'Faculty', 'info', 'Announcement');
GO

-- Insert Sample Enrollments (enroll student4 in CS101)
INSERT INTO Enrollments (Id, StudentId, CourseId, EnrollmentDate, IsActive) VALUES
(NEWID(), '537bdf6b-06e0-45cc-aec8-5512168ae5bb', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', GETDATE(), 1);
GO

-- Insert Sample Grades
INSERT INTO GradeRecords (Id, StudentId, CourseId, FacultyId, Marks, LetterGrade, GradePoints, Remarks) VALUES
(NEWID(), '537bdf6b-06e0-45cc-aec8-5512168ae5bb', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '88888888-8888-8888-8888-888888888888', 85, 'A', 4.0, 'Excellent work!');
GO

-- Verify all tables were created
SELECT 'Users' as TableName, COUNT(*) as RowCount FROM Users
UNION ALL
SELECT 'Students', COUNT(*) FROM Students
UNION ALL
SELECT 'Faculties', COUNT(*) FROM Faculties
UNION ALL
SELECT 'Courses', COUNT(*) FROM Courses
UNION ALL
SELECT 'Enrollments', COUNT(*) FROM Enrollments
UNION ALL
SELECT 'GradeRecords', COUNT(*) FROM GradeRecords
UNION ALL
SELECT 'Notifications', COUNT(*) FROM Notifications;
GO

SELECT * FROM Notifications;
SELECT * FROM Courses;