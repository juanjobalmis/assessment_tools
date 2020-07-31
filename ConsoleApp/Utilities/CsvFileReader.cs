using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AssessmentTools
{
    class CsvFileReader : StreamReader
    {
        private string FileName { get; set;}

        public class CsvFileReaderException : Exception
        {
            public CsvFileReaderException(string message) : base(message)
            {
                ;
            }
            public CsvFileReaderException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }
        public CsvFileReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks) : base(path, encoding, detectEncodingFromByteOrderMarks)
        {
            FileName = path;
        }

        public List<string> ReadRow()
        {            
            if (EndOfStream)
                throw new CsvFileReaderException($"You have reached the end of {Path.GetFileName(FileName)}");

            string text = this.ReadLine();
            List<string> row = new List<string>();
            foreach (var value in text.Split(new char[] { ';' }, StringSplitOptions.None))
                row.Add(value.Replace("\"", ""));

            return row;
        }
    }
}
