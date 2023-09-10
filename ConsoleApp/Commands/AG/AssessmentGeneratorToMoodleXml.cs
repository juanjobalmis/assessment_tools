using AssessmentTools.Data;
using SheetWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace AssessmentTools.Commands.AG
{
    public static class AssessmentGeneratorToMoodleXml
    {
        private const string MOODLE_FILE = "moodle.xml";

        //< results >
        //    < result >
        //        < assignment > activityidnumber </ assignment >
        //        < student > studentidnumber </ student >
        //        < score > 53.00 </ score >
        //       < feedback ><![CDATA[<p> .... </p>]]></ feedback>
        //    </ result >
        //</ results>
        private static void AddStudentAssessmentToMoodleXML(
                                        string assignmentName,
                                        XmlTextWriter xml,
                                        Student student,
                                        double grade,
                                        string feedback)
        {
            xml.WriteStartElement("result");

            xml.WriteElementString("assignment", assignmentName);
            xml.WriteElementString("student", student.ID);
            xml.WriteElementString("score", $"{(grade != 0 ? grade : 1):#}");
            xml.WriteStartElement("feedback");
            xml.WriteCData(feedback);
            xml.WriteEndElement();

            xml.WriteEndElement();
        }

        public static void Generate(
            string assignmentName,
            List<string> rubricFiles,
            List<Student> studentsData,
            bool verbose)
        {
            using XmlTextWriter xml = new(MOODLE_FILE, Encoding.UTF8);
            xml.Formatting = Formatting.Indented;
            xml.WriteStartElement("results");
            foreach (var rubic in rubricFiles)
            {
                string studentName = "UNKNOWN";
                try
                {
                    using Book b = new Book(rubic, true);
                    studentName = Path.GetFileNameWithoutExtension(b.FileName);
                    var student = studentsData.Find(s => s.Name.CompareTo(studentName) == 0);
                    if (student.ValidID())
                    {
                        if (verbose)
                            Console.WriteLine($"Adding assessment to {MOODLE_FILE} for {student.Name}...");
                        using CellRange cell = b.MainSheet["mark"];
                        var grade = double.Parse(cell.Text) * 10;
                        AddStudentAssessmentToMoodleXML(
                            assignmentName, 
                            xml, 
                            student, 
                            grade,
                            SheetToHtml.Convert(b.MainSheet));
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
            xml.WriteEndElement();
            Console.WriteLine($"Assessment correctly generated in {MOODLE_FILE}");
        }
    }
}
