using System;
using System.Collections.Generic;
using System.IO;

namespace AssessmentTools.Commands
{
    public class QuizGeneratorOptions : CommandOptions
    {
        private enum ForcedOption : ushort { outputFolder = 0x0001 }
        protected override List<string> ForcedOptionsNotIntroduced => ForcedOptionsNotInMask(typeof(ForcedOption));
        public const string COMMAND = "QG";
        public const string COMMAND_EXTENDED = "QUIZ_GENERATOR";
        protected override string Command => COMMAND;
        protected override string CommandExtended => COMMAND_EXTENDED;

        public string OutputFolder { get; private set; }

        public bool IsThereOutputFolder => OutputFolder != null;

        public QuizGeneratorOptions(string[] mainArguments) : base(mainArguments)
        {
        }

        private void SetOutputFolder(List<string> options, ref int optionIndex)
        {
            string option = options[optionIndex++];
            OutputFolder = options[optionIndex];

            if (OutputFolder[0] == '-')
                throw new CommandManager.CommandException($"After {option} option, you have to specify a valid output folder.", this);
            if (Directory.Exists(OutputFolder))
                throw new CommandManager.CommandException($"The output folder {OutputFolder} already exists.", this);
            Directory.CreateDirectory(OutputFolder);
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
                case "--outputFolder":
                case "-o":
                    SetOutputFolder(options, ref optionIndex);
                    ForcedOptionsSetMask |= (ushort)ForcedOption.outputFolder;
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
                $"\t\t-o (--outputFolder) <outputFolder>:\n" +
                $"\t\t\t * To specify the output folder for the generated quiz banks for moodle.\n\n" +
                $"\t\t Note: The YAML file name has to follow the pattern 'block-theme.yaml' to categorize the questions.\n" +
                $"\t\t in the moodle quiz bank for the curse.\n" +
                $"\t\t Besides, the YAML file must have the following structure:\n\n" +
                $"\t\t\t preguntas:\n" +
                $"\t\t\t - enunciado: <html of the question 1>\n" +
                $"\t\t\t   respuestas:\n" +
                $"\t\t\t   - texto: <html of the answer 1>\n" +
                $"\t\t\t     correcta: <true or false depending if the answer is correct or not>\n" +
                $"\t\t\t   - texto: <html of the answer 2>\n" +
                $"\t\t\t     correcta: <true or false depending if the answer is correct or not>\n" +
                $"\t\t\t - enunciado: <html of the question 2>\n" +
                $"\t\t\t   respuestas:\n" +
                $"\t\t\t   - texto: <html of the answer 1>\n" +
                $"\t\t\t     correcta: <true or false depending if the answer is correct or not>\n" +
                $"\t\t\t   ...\n\n" +
                $"\n\n\nExample: {program} {Command} -v -o quizzes\n\n";
        }
    }
}

// preguntas:
// - enunciado: ¿Qué condición <b>mínima</b> es necesaria para <b>ejecutar aplicaciones .NET</b> en un sistema operativo?
//   respuestas:
//   - texto: Tener el CLR instalado.
//     correcta: true
//   - texto: Tener el JRE instalado.
// - enunciado: ¿Qué es un <b>ensamble</b> o <b>ensamblado</b> en .NET?
//   respuestas:
//   - texto: Es un fichero con el código ensamblador nativo de la máquina.
//     correcta: false
