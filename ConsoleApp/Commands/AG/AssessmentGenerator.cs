using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using AssessmentTools.Data;
using AssessmentTools.Commands.AG;

namespace AssessmentTools.Commands
{
    public static class AssessmentGenerator
    {
        private static List<string> RubricFilesForReporting(List<Student> studentsData, bool verbose = false)
        {
            List<string> rubricFiles = Directory.GetFiles(".", "*.xlsx").ToList();
            List<string> validRubricFiles = studentsData.Select(s => $"{s.Name}.xlsx").ToList();

            int i = 0;
            while (i < rubricFiles.Count)
            {
                if (validRubricFiles.FindIndex(vf => vf.CompareTo(Path.GetFileName(rubricFiles[i])) == 0) < 0)
                {
                    if (verbose)
                        Console.WriteLine($"The file {rubricFiles[i]} is not a valid rubric file for a Student.");
                    rubricFiles.RemoveAt(i);
                }
                else
                    i++;
            }

            return rubricFiles;
        }

        public static void Generate(AssessmentGeneratorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options", "TemplateGeneratorOptions needs to be instantiated.");

            List<Student> studentsData = StudentsData.LoadFromCSV(options.StudentNamesFile);
            if (studentsData.Count == 0)
                throw new GeneratorException($"There are no students in {options.StudentNamesFile} to find their rubrics.");

            List<string> rubricFiles = RubricFilesForReporting(studentsData, options.Verbose);
            if (rubricFiles.Count == 0)
                throw new GeneratorException($"There are no valid rubric files in {Directory.GetCurrentDirectory()} to generate the assessment report.");

            switch (options.AssessmentPlatform.TargetPlatForm)
            {
                case AssessmentPlatform.Target.Moodlexml:
                    AssessmentGeneratorToMoodleXml.Generate(options.AssignmentName, rubricFiles, studentsData, options.Verbose);
                    break;
                case AssessmentPlatform.Target.Moodlecsv:
                    AssessmentGeneratorToMoodleCsv.Generate(options.AssignmentName, rubricFiles, studentsData, options.Verbose);
                    break;
                case AssessmentPlatform.Target.Email:
                    AssessmentGeneratorToEMail.Generate(options.AssignmentName, rubricFiles, studentsData, options.Verbose);
                    break;
                case AssessmentPlatform.Target.Xlsx:
                    AssessmentGeneratorToBook.GenerateSummary(options.AssignmentName, rubricFiles, studentsData, options.Verbose);
                    break;
                default:
                    throw new ArgumentException($"The target platform '{options.AssessmentPlatform.TargetPlatForm}' is not still available.");
            }
        }
    }
}
