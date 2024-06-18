using System;
using System.Collections.Generic;
using System.IO;

namespace AssessmentTools.Commands
{
    public class QuizGeneratorOptions : CommandOptions
    {
        private enum ForcedOption : ushort { quizBankFilename = 0x0001 }
        protected override List<string> ForcedOptionsNotIntroduced => ForcedOptionsNotInMask(typeof(ForcedOption));
        public const string COMMAND = "QG";
        public const string COMMAND_EXTENDED = "QUIZ_GENERATOR";
        protected override string Command => COMMAND;
        protected override string CommandExtended => COMMAND_EXTENDED;

        public string QuizBankFilename { get; private set; }

        public bool IsThereStudentNamesFile => StudentNamesFile != null;

        public QuizGeneratorOptions(string[] mainArguments) : base(mainArguments)
        {
        }

        private void SetQuizBankFilename(List<string> options, ref int optionIndex)
        {
            string option = options[optionIndex++];
            QuizBankFilename = options[optionIndex];

            if (QuizBankFilename[0] == '-')
                throw new CommandManager.CommandException($"After {option} option, you have to specify a valid XML file.", this);
            if (Path.GetExtension(QuizBankFilename).ToUpper() != "XML")
                throw new CommandManager.CommandException($"The target file {QuizBankFilename} must hace xml extension.", this);
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
                case "--quizBankFilename":
                case "-q":
                    SetQuizBankFilename(options, ref optionIndex);
                    ForcedOptionsSetMask |= (ushort)ForcedOption.quizBankFilename;
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
                $"\t\t-q (--quizBankFilename) <quizBankFilename>:\n" +
                $"\t\t\t * To specify the XML file name with the quiz bank questions.\n\n" +
                $"\n\n\nExample: {program} {Command} -v -q quizBank.xml\n";
        }
    }
}
