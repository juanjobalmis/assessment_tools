using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace AssessmentTools.Data
{
    public static class StudentsData
    {
        static private Dictionary<string, int> MapPropertyNamesToColNamesIndexInCSV(List<string> columNames, bool strict = false)
        {
            Dictionary<string, int> map = new Dictionary<string, int>();

            List<string> studentProperties = Student.PropertyNames;

            for (int i = 0; i < columNames.Count; i++)
            {
                string propertyName = studentProperties.Find(p => p.ToUpper() == columNames[i].ToUpper());
                if (propertyName != default)
                {
                    if (map.ContainsKey(propertyName))
                        throw new StudentsDataException($"The column name {columNames[i]} is duplicated in the csv file.");
                    map.Add(propertyName, i);
                }
                else
                {
                    if (strict)
                    {
                        string message = $"The column name {columNames[i]} is not a valid student data.\n" +
                                         $"The valid column names are the following: {string.Join(", ", studentProperties)}";
                        throw new StudentsDataException(message);
                    }
                    else
                        Console.WriteLine($"The student data {columNames[i]} is not loaded.");
                }
            }

            return map;
        }

        static public List<Student> LoadFromCSV(string studentNamesFile)
        {
            List<Student> students = new List<Student>();
            CsvFileReader csv = new CsvFileReader(studentNamesFile, Encoding.UTF8, true);

            try
            {
                bool first = true;
                Dictionary<string, int> map = default;
                while (!csv.EndOfStream)
                {
                    List<string> studentCSVData = csv.ReadRow();
                    if (first)
                    {
                        first = false;
                        map = MapPropertyNamesToColNamesIndexInCSV(studentCSVData);
                    }
                    else
                        students.Add(Student.CSVDataFactory(studentCSVData, map));
                }
            }
            catch (Exception e)
            {
                throw new CsvFileReader.CsvFileReaderException($"Error reading {studentNamesFile}", e);
            }
            finally
            {
                if (csv != null)
                    csv.Close();
            }
            return students;
        }

        private static string ToMoodleFolderName(this string name)
        {
            string[] parts = name.Split(", ");
            return $"{parts[1]} {parts[0]}";
        }

        static public Dictionary<string, List<string>> NamesWithAlias(string studentNamesFile)
        {
            Func<Student, List<string>> aliasGruping = s => new List<string> { s.Name, s.Name.ToMoodleFolderName(), s.ID, s.NickName, s.GitHubLogin };
            return LoadFromCSV(studentNamesFile).ToDictionary(s => s.Name, aliasGruping);
        }
    }
}
