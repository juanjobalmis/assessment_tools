using System;

namespace AssessmentTools.Commands
{
    public class GeneratorException : Exception
    {
        public GeneratorException(string message) : base(message)
        {
        }
        public GeneratorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
