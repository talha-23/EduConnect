using System;

namespace EduConnect.Exceptions
{
    public class StudentHasActiveEnrollmentsException : Exception
    {
        public StudentHasActiveEnrollmentsException()
            : base("Cannot delete student because they have active course enrollments.")
        {
        }

        public StudentHasActiveEnrollmentsException(string message)
            : base(message)
        {
        }

        public StudentHasActiveEnrollmentsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}