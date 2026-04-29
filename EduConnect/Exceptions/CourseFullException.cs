using System;

namespace EduConnect.Exceptions
{
    public class CourseFullException : Exception
    {
        public CourseFullException()
            : base("Cannot enroll because the course has reached maximum capacity.")
        {
        }

        public CourseFullException(string message)
            : base(message)
        {
        }

        public CourseFullException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}