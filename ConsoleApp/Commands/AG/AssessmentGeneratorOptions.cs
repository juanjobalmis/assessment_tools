using AssessmentTools.Commands.AG;
using System;
using System.Collections.Generic;
using System.IO;

namespace AssessmentTools.Commands
{
    public class AssessmentGeneratorOptions : CommandOptions
    {
        [Flags]
        private enum ForcedOption : ushort 
        { 
            studentNamesFile = 0x0001, 
            targetPlatform = 0x0002 
        }
        protected override List<string> ForcedOptionsNotIntroduced => ForcedOptionsNotInMask(typeof(ForcedOption));

        public const string COMMAND = "AG";
        public const string COMMAND_EXTENDED = "ASSESSMENT_GENERATOR";
        protected override string Command => COMMAND;
        protected override string CommandExtended => COMMAND_EXTENDED;

        public AssessmentPlatform AssessmentPlatform { get; private set; }
        public string AssignmentName { get; private set; }
        public AssessmentGeneratorOptions(string[] mainArguments) : base(mainArguments)
        {
            AssignmentName = AssignmentName ?? Path.GetFileName(Directory.GetCurrentDirectory());
        }
        protected override void CheckOption(List<string> options, ref int optionIndex)
        {
            switch (options[optionIndex])
            {
                case "--verbose":
                case "-v":
                    Verbose = true;
                    break;
                case "--test":
                case "-t":
                    Test = true;
                    break;
                case "--assignmentName":
                case "-n":
                    AssignmentName = options[++optionIndex];
                    break;
                case "--studentNamesFile":
                case "-s":
                    SetStudentNamesFile(options, ref optionIndex);
                    ForcedOptionsSetMask |= (ushort)ForcedOption.studentNamesFile;
                    break;
                case "--platform":
                case "-p":
                    AssessmentPlatform = new AssessmentPlatform(options[++optionIndex]);
                    ForcedOptionsSetMask |= (ushort)ForcedOption.targetPlatform;
                    break;
                default:
                    throw new CommandManager.CommandException($"Option {options[optionIndex]} is not valid.", this);
            }
        }
        public override string Help(string program)
        {
            return
                base.Help(program) +
                $"\t<options>\n" +
                $"{CommonHelp()[CommonHelpOption.Verbose]}\n" +
                $"{CommonHelp()[CommonHelpOption.Test]}\n" +
                $"{CommonHelp()[CommonHelpOption.StudentNamesFile]}\n" +
                $"\t\t-n (--assignmentName) <name>:\n" +
                $"\t\t\t * Name of the assignment.\n" +
                $"\t\t\t * If it is not specified the current folder name is taken.\n" +
                $"\t\t-p (--platform) <platform>:\n" +
                $"\t\t\t Available platforms:\n" +
                $"\t\t\t 1. moodlexml -> It generates a moodle grade format into moodle.xml file.\n" +
                $"\t\t\t                   To set the assignment Name and the student ID see Moodle doc at\n" +
                $"\t\t\t                   https://docs.moodle.org/38/en/Grade_import#XML_import \n" +
                $"\t\t\t 2. moodlecsv -> It generates a CSV grade format into moodle.csv file.\n" +
                $"\t\t\t                   We need to map Email with ID to identify a Student see\n" +
                $"\t\t\t                   https://docs.moodle.org/38/en/Grade_import#CSV_import \n" +
                $"\t\t\t 3. email -> It generates crypt credentials for SMTP server whether there are not.\n" +
                $"\t\t\t               Then it will send a e-Mail to each student with their rubric information\n" +
                $"\t\t\t               The message subject is the assignmentName value.\n\n" +
                $"\t\t Note: The xlsx book file with the assessment has to have a Named cell or range\n" +
                $"\t\t       called 'mark' to find out the assessment mark, grade or score in some targets." +
                $"\n\n\nExample: {program} {COMMAND} -v -s studentsData.csv -n exercise1 -p moodlecsv\n";
        }
    }
}
