using AssessmentTools.Data;
using SheetWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AssessmentTools.Commands.AG
{
    public static class AssessmentGeneratorToMoodleCsv
    {
        private const string MOODLE_FILE = "moodle.csv";

        public static void Generate(
            string assignmentName,
            List<string> rubricFiles,
            List<Student> studentsData,
            bool verbose)
        {
            using StreamWriter csv = new(MOODLE_FILE, false, Encoding.UTF8);
            csv.WriteLine("Nombre,Correo,Actividad,Calificacion");
            foreach (var rubic in rubricFiles)
            {
                string studentName = "UNKNOWN";
                try
                {
                    using Book b = new Book(rubic, true);
                    studentName = Path.GetFileNameWithoutExtension(b.FileName);
                    var student = studentsData.Find(s => s.Name.CompareTo(studentName) == 0);
                    if (student.ValidEmail())
                    {
                        if (verbose)
                            Console.WriteLine($"Adding assessment to {MOODLE_FILE} for {student.Name}...");
                        using CellRange cell = b.MainSheet["mark"];        
                        var grade = double.Parse(cell.Text);
                        csv.WriteLine($"\"{student.Name}\",{student.Mail},\"{assignmentName}\",\"{grade}\"");
                    }
                }
                catch (Exception e)
                {
                    var message = new StringBuilder($"\nImpossible to generate assessent for {studentName} in {MOODLE_FILE} ...\n");
                    while (e != null)
                    {
                        message.Append($"\t{e.Message}\n");
                        e = e.InnerException;
                    }
                    Console.WriteLine(message);
                }
            }
            Console.WriteLine($"Assessment correctly generated in {MOODLE_FILE}");
        }
    }
}
