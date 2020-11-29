using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AssessmentTools
{
    public class CsvFileReader : StreamReader
    {
        private string FileName { get; set;}

        public class CsvFileReaderException : Exception
        {
            public CsvFileReaderException(string message) : base(message)
            {
            }
            public CsvFileReaderException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }
        public CsvFileReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks) : base(path, encoding, detectEncodingFromByteOrderMarks)
        {
            FileName = path;
        }

        private static string[] JoinOrphanStrings(string[] input, string separator)
        {
            List<string> output = new List<string>();
            for (int i = 0; i < input.Length; i++)
            {
                string data = input[i];
                if (output.Count > 0 && data.Length > 0 && data[0] != '"' && data[^1] == '"')
                    output[^1] = $"{output[^1]}{separator}{data}";
                else
                    output.Add(data);
            }
            return output.ToArray();
        }
        public List<string> ReadRow()
        {            
            if (EndOfStream)
                throw new CsvFileReaderException($"You have reached the end of {Path.GetFileName(FileName)}");

            string text = this.ReadLine();
            List<string> row = new List<string>();
            string[] fields = text.Split(new char[] { ';', ',' }, StringSplitOptions.None);
            foreach (var value in JoinOrphanStrings(fields, ","))
                row.Add(value.Replace("\"", ""));

            return row;
        }
    }
}
