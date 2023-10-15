using AssessmentTools.Data;
using AssessmentTools.Utilities;
using AssessmentTools.Utilities.EnviarCorreoElectronico;
using SheetWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AssessmentTools.Commands.AG
{
    public static class AssessmentGeneratorToEMail
    {
        private static string CredentialsPath
        {
            get
            {
                const string CREDENTIALS_FILE = "smtp.credentials";
                return Path.Combine(PathE.ExecutableDirectory(), CREDENTIALS_FILE);
            }
        }
        private static SmtpServerCredentials Credentials
        {
            get
            {
                return SmtpServerCredentials.Create(CredentialsPath);
            }
        }

        private static List<Book> BooksWithSummaryPerStudent(GroupAssessmentData data)
        {
            List<Book> books = new List<Book>();
            try
            {
                int startDataRow = 3;
                int startDataCol = 3;

                foreach (EntityDataAssessed e in data)
                {
                    if (e.Grade > 0d)
                    {
                        EntityDataAssessed firstStudentAssessed = data.First(e => e.Criteria.Any());
                        Book b = AssessGeneratorCommon.CreateBookWithoutData(
                                $"{e.Entity.Name}", data.AssignmentName,
                                firstStudentAssessed.Criteria.Select(c => c.Name).ToList(),
                                firstStudentAssessed.Select(c => $"{c.Criterion.WeightPercentage}%").ToList(),
                                startDataRow, startDataCol);
                        AssessGeneratorCommon.AddEntityData(b.MainSheet, e, startDataRow, startDataCol);
                        books.Add(b);
                    }
                }
            }
            catch (Exception e)
            {
                throw new GeneratorException($"Error generating summary books for {data.AssignmentName}", e);
            }
            return books;
        }

        public static void Generate(
            string assignmentName,
            List<string> rubricFiles,
            List<Student> studentsData,
            bool verbose)
        {
            using (MailSender mailSender = new MailSender(Credentials))
            {
                foreach (var rubic in rubricFiles)
                {
                    string studentName = "UNKNOWN";
                    try
                    {
                        using (Book b = new Book(rubic, true))
                        {
                            studentName = Path.GetFileNameWithoutExtension(b.FileName);
                            var student = studentsData.Find(s => s.Name.CompareTo(studentName) == 0);
                            if (verbose)
                                Console.Write($"Sending {assignmentName} assessment for {student.Name} to {student.Mail}...");
                            mailSender.Send(student.Mail, assignmentName, SheetToHtml.Convert(b.MainSheet));
                            if (verbose)
                                Console.WriteLine("OK");
                        }
                    }
                    catch (Exception e)
                    {
                        var message = new StringBuilder($"\nImpossible to send assessent for {studentName} ...\n");
                        while (e != null)
                        {
                            message.Append(e.Message.IndexOf("Authentication Required") >= 0
                                ? $"\tYour credentials are not valid. Please check them out:\n\t{Credentials}\n\tDelete {CredentialsPath} to renew them.\n"
                                : $"\t{e.Message}\n");
                            Console.WriteLine($"{message}");
                            e = e.InnerException;
                        } 
                        Console.WriteLine(message);
                    }
                }
            }
        }

        public static void GenerateSummary(
            string assignmentName,
            List<string> rubricFiles,
            List<Student> studentsData,
            bool verbose)
        {
            using (MailSender mailSender = new MailSender(Credentials))
            {
                string studentName = "UNKNOWN";
                try
                {
                    GroupAssessmentData data = AssessGeneratorCommon.Collect(assignmentName, rubricFiles, studentsData, verbose);
                    List<Book> books = BooksWithSummaryPerStudent(data);
                    foreach (Book b in books)
                    {
                        studentName = Path.GetFileNameWithoutExtension(b.FileName);
                        var student = studentsData.Find(s => s.Name.CompareTo(studentName) == 0);
                        if (verbose)
                            Console.Write($"Sending {assignmentName} assessment summary for {student.Name} to {student.Mail}...");
                        mailSender.Send(student.Mail, assignmentName, SheetToHtml.Convert(b.MainSheet));
                        b.Dispose();
                        if (verbose)
                            Console.WriteLine("OK");
                    }
                    books.Clear();
                }
                catch (Exception e)
                {
                    var message = new StringBuilder($"\nImpossible to send assessent for {studentName} ...\n");
                    while (e != null)
                    {
                        message.Append(e.Message.IndexOf("Authentication Required") >= 0
                            ? $"\tYour credentials are not valid. Please check them out:\n\t{Credentials}\n\tDelete {CredentialsPath} to renew them.\n"
                            : $"\t{e.Message}\n");
                        Console.WriteLine($"{message}");
                        e = e.InnerException;
                    }
                    Console.WriteLine(message);
                }
            }
        }
    }
}