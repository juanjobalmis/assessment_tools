using System;
using System.Collections.Generic;
using System.IO;

namespace AssessmentTools.Commands
{
    class RubricGeneratorOptions : CommandOptions
    {
        private enum ForcedOption : ushort { rubricTemplateFile = 0x0001 }
        protected override List<string> ForcedOptionsNotIntroduced => ForcedOptionsNotInMask(typeof(ForcedOption));

        public const string COMMAND = "RG";
        public const string COMMAND_EXTENDED = "RUBRIC_GENERATOR";
        protected override string Command => RubricGeneratorOptions.COMMAND;
        protected override string CommandExtended => RubricGeneratorOptions.COMMAND_EXTENDED;

        public string RubricTemplate { get; private set; }
        public string TemplateExtension { get; private set; }
        public bool NormalizedName { get; private set; }
        public bool IsThereStudentNamesFile => StudentNamesFile != null;

        public RubricGeneratorOptions(string[] mainArguments) : base(mainArguments)
        {
        }

        private void SetRubricFile(List<string> options, ref int optionIndex)
        {
            string option = options[optionIndex++];
            RubricTemplate = options[optionIndex];

            if (RubricTemplate[0] == '-')
                throw new CommandManager.CommandException($"After {option} option, you have to specify a valid rubric file.", this);
            if (!File.Exists(RubricTemplate))
                throw new CommandManager.CommandException($"The rubric file {RubricTemplate} does not exist.", this);

            TemplateExtension = Path.HasExtension(RubricTemplate) ? Path.GetExtension(RubricTemplate) : "";
            TemplateExtension = NormalizedName ? TemplateExtension.ToUpper() : TemplateExtension;
        }

        protected override void CheckOption(List<string> options, ref int optionIndex)
        {
            switch (options[optionIndex])
            {
                case "--verbose":
                case "-v":
                    Verbose = true;
                    break;
                case "--normalized":
                case "-n":
                    NormalizedName = true;
                    break;
                case "--studentNamesFile":
                case "-s":
                    SetStudentNamesFile(options, ref optionIndex);
                    break;
                case "--rubricTemplateFile":
                case "-r":
                    SetRubricFile(options, ref optionIndex);
                    ForcedOptionsSetMask |= (ushort)ForcedOption.rubricTemplateFile;
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
                $"{CommonHelp()[CommonHelpOption.StudentNamesFile]}\n" +
                $"\t\t-n (--normalized):\n" +
                $"\t\t\t * Normalize the character to Unicode C.\n" +
                $"\t\t\t * Uppercase conversion.\n" +
                $"\t\t\t * Reduces the folder name to the student name. (whether it is possible).\n" +
                $"\t\t\t * Not necessary if you define the --studentNamesFile.\n" +
                $"\t\t-r (--rubricTemplateFile) <rubricTemplateFile>:\n" +
                $"\t\t\t * To specify the file name with the assessment rubric template.\n\n" +
                $"\t\t Note: The columns ID, NickName and GitHubLogin are used as possible\n" +
                $"\t\t       students alias to match student name in the delivered folders.\n" +
                $"\t\t       In case of ambiguity, the program tries to match an existing\n" +
                $"\t\t       folder with a student name, asking us for confirmation.\n" +
                $"\t\t       Besides, use students name file implies normalization." +
                $"\n\n\nExample: {program} {Command} -v -s studentsData.csv -r assessmentTemplate.xlsx\n";
        }
    }
}
