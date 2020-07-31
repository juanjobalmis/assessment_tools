using System;

namespace AssessmentTools.Data
{
    public class StudentsDataException : Exception
    {
        public StudentsDataException(string message) : base(message)
        {
        }

        public StudentsDataException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
