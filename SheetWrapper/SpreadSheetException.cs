using System;

namespace SheetWrapper
{
    public class SpreadSheetException : Exception
    {
        public SpreadSheetException(string message) : base(message)
        {
        }

        public SpreadSheetException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
