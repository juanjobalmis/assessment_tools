using AssessmentTools.Data;
using SheetWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AssessmentTools.Commands.AG
{
    static class AssessGeneratorCommon
    {
        private static EntityDataAssessed CreateFromSheet(
            Sheet assessment,
            Student student,
            bool verbose = false)
        {
            EntityDataAssessed ad;
            try
            {
                ad = new EntityDataAssessed(student);
                using (CellRange criteria = assessment["items"])
                using (CellRange criteriaWeight = assessment["itemsweight"])
                using (CellRange criteriaGrade = assessment["itemsmark"])
                {
                    var ce = criteria.GetEnumerator();
                    var cwe = criteriaWeight.GetEnumerator();
                    var cge = criteriaGrade.GetEnumerator();
                    while (ce.MoveNext())
                    {
                        string criteriaDescription = ce.Current.Text.Replace('\n', ' ');
                        if (!cwe.MoveNext())
                            throw new GeneratorException($"There is not found weight for {criteriaDescription}");
                        if (!cge.MoveNext())
                            throw new GeneratorException($"There is not found grade for {criteriaDescription}");
                        Match weightMath = Regex.Match(cwe.Current.Text, @"\d+");
                        ushort percentage = weightMath.Success ? ushort.Parse(weightMath.Value) : (ushort)0;
                        double grade = weightMath.Success ? double.Parse(cge.Current.Text) : 0D;
                        ad.Add(new AssessableCriterionRegistry(criteriaDescription, percentage, grade));
                    }
                }
                if (verbose)
                    Console.WriteLine(ad);
            }
            catch (Exception e)
            {
                throw new GeneratorException($"Reading assess data for {student.Name}", e);
            }

            return ad;
        }

        private static List<AssessableCriterion> GetAssessmentCriteriaFrom(
            string rubricFile,
            bool verbose = false)
        {
            List<AssessableCriterion> criteria = new List<AssessableCriterion>();
            try
            {
                if (verbose)
                    Console.WriteLine($"Collecting assessment criteria from {rubricFile}:");
                using (Book b = new Book(rubricFile, true))
                using (CellRange sheetCriteria = b.MainSheet["items"])
                using (CellRange sheetCriteriaWeight = b.MainSheet["itemsweight"])
                {
                    var ce = sheetCriteria.GetEnumerator();
                    var cwe = sheetCriteriaWeight.GetEnumerator();
                    while (ce.MoveNext())
                    {
                        string criteriaDescription = ce.Current.Text.Replace('\n', ' ');
                        if (!cwe.MoveNext())
                            throw new GeneratorException($"There is not found weight for {criteriaDescription}");
                        Match weightMath = Regex.Match(cwe.Current.Text, @"\d+");
                        ushort percentage = weightMath.Success ? ushort.Parse(weightMath.Value) : (ushort)0;
                        criteria.Add(new AssessableCriterion(criteriaDescription, percentage));
                        if (verbose)
                            Console.WriteLine($"\t- {criteria.Last()} added.");
                    }
                }
            }
            catch (Exception e)
            {
                throw new GeneratorException($"Error collecting assessment criteria from {rubricFile}", e);
            }

            return criteria;
        }

        private static double GetCellHeightAccordingToCriteriaNames(List<string> criteriaNames)
        {
            double height;
            int maxCriterionLeght = criteriaNames.Select(c => c.Length).Max();
            if (maxCriterionLeght < 30)
                height = 100;
            else if (maxCriterionLeght < 60)
                height = 125;
            else if (maxCriterionLeght < 90)
                height = 150;
            else
                height = 200;

            return height;
        }

        public static GroupAssessmentData Collect(
            string assignmentName,
            List<string> rubricFiles,
            List<Student> studentsData,
            bool verbose = false)
        {
            GroupAssessmentData summary = new GroupAssessmentData(assignmentName);
            GetAssessmentCriteriaFrom(rubricFiles.First(), verbose).ForEach(ac => summary.Add(ac));

            foreach (var student in studentsData)
            {
                try
                {
                    string rubric;
                    EntityDataAssessed ad;
                    if ((rubric = rubricFiles.Find(rf => rf.Contains(student.Name))) != default)
                    {
                        using (Book b = new Book(rubric, true))
                        {
                            if (verbose)
                                Console.WriteLine($"Creating assess data for {student.Name} ...");
                            ad = CreateFromSheet(b.MainSheet, student, verbose);
                        }
                    }
                    else
                    {
                        ad = new EntityDataAssessed(student);
                    }
                    summary.Add(ad);
                }
                catch (Exception e)
                {
                    throw new GeneratorException("Error collecting students assess data", e);
                }
            }

            return summary;
        }

        public static Book CreateBookWithoutData(
            string bookName,
            string assignmentName,
            List<string> criteriaNames,
            List<string> weightPercentages,
            int startDataRow,
            int startDataCol)
        {
            if (startDataRow < 3)
                throw new ArgumentException("Data must start from row 3", "startDataRow");
            if (startDataCol < 3)
                throw new ArgumentException("Data must start from column 3", "startDataCol");

            Book b;
            string path = $"{bookName}.xlsx";
            try
            {
                b = new Book(path, false);
                StylesPackageBN.ApplyTo(b);

                Sheet s = b.AddSheet(assignmentName);
                s.ShowGridLines = false;

                // Criteria Title Columns
                s[new CellAddress(startDataRow - 2, startDataCol)].LoadDataHorizontally(criteriaNames, StylesPackageBN.Style.LongPrimaryColumnName.ToString());
                s.GetRow(1).Height = GetCellHeightAccordingToCriteriaNames(criteriaNames);
                s[new CellAddress(startDataRow - 1, startDataCol - 2)].LoadSingleData("Name", StylesPackageBN.Style.ShortSecondaryColumnName.ToString());
                s[new CellAddress(startDataRow - 1, startDataCol - 1)].LoadSingleData("Group", StylesPackageBN.Style.ShortSecondaryColumnName.ToString());
                s[new CellAddress(startDataRow - 1, startDataCol)].LoadDataHorizontally(weightPercentages, StylesPackageBN.Style.ShortSecondaryColumnName.ToString());
                s[new CellAddress(startDataRow - 1, startDataCol + criteriaNames.Count)].LoadSingleData("Grade", StylesPackageBN.Style.ShortSecondaryColumnName.ToString());
            }
            catch (Exception e)
            {
                throw new GeneratorException($"Error creating asess data template for {assignmentName} assignment to {path}", e);
            }
            return b;
        }

        public static void AddEntityData(
             Sheet s,
             EntityDataAssessed e,
             int dataRow, int dataCol)
        {
            s[new CellAddress(dataRow, dataCol - 2)].LoadSingleData(e.Entity.Name, StylesPackageBN.Style.RemarkableCellLeft.ToString());
            s[new CellAddress(dataRow, dataCol - 1)].LoadSingleData(e.Entity.Group, StylesPackageBN.Style.RemarkableCellCenter.ToString());
            double[] criteriaGrades = e.Select(c => c.GradeRelativeToWeight).ToArray();
            s[new CellAddress(dataRow, dataCol)].LoadDataHorizontally(criteriaGrades, StylesPackageBN.Style.RegularCell.ToString());
            s[new CellAddress(dataRow, dataCol + criteriaGrades.Length)].LoadSingleData(e.Grade, StylesPackageBN.Style.RemarkableCellCenter.ToString());
        }

    }
}
