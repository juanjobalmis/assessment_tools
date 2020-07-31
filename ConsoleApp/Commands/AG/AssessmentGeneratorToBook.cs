using System;
using System.Linq;
using System.Collections.Generic;
using SheetWrapper;
using AssessmentTools.Data;
using System.Text;

namespace AssessmentTools.Commands.AG
{
    internal static class AssessmentGeneratorToBook
    {
        private static Book ToBook(GroupAssessmentData data)
        {
            Book b;
            string path = $"{data.AssignmentName}.xlsx";
            try
            {
                int startDataRow = 3;
                int startDataCol = 3;

                b = AssessGeneratorCommon.CreateBookWithoutData(
                            data.AssignmentName,
                            data.AssignmentName,
                            data.First().Criteria.Select(c => c.Name).ToList(),
                            data.First().Select(c => $"{c.Criterion.WeightPercentage}%").ToList(),
                            startDataRow,
                            startDataCol);

                foreach (var e in data)
                    AssessGeneratorCommon.AddEntityData(b.MainSheet, e, startDataRow++, startDataCol);

            }
            catch (Exception e)
            {
                throw new GeneratorException($"Error generating group assess data for {data.AssignmentName} assignment to {path}", e);
            }
            return b;
        }

        public static void GenerateSummary(
            string assignmentName,
            List<string> rubricFiles,
            List<Student> studentsData,
            bool verbose = false)
        {
            try
            {
                GroupAssessmentData data = AssessGeneratorCommon.Collect(assignmentName, rubricFiles, studentsData, verbose);
                using (Book b = ToBook(data))
                {
                    b.Save();
                    if (verbose)
                        Console.WriteLine($"The assessment summary has been successfully generate to {data.AssignmentName}.xlsx file.");
                }
            }
            catch (Exception e)
            {
                var message = new StringBuilder($"\nImpossible to generate XLSX summary...\n");
                while (e != null)
                {
                    message.Append($"\t{e.Message}\n");
                    e = e.InnerException;
                }
                Console.WriteLine(message);
            }
        }
    }
}
