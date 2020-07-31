using System;
using System.IO;

namespace AssessmentTools.Commands
{
    public class CommandManager : CommandManager.IHelpProvider
    {
        public interface IHelpProvider
        {
            string Help(string program);
        }

        public class CommandException : Exception
        {
            private readonly static string program = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
            public CommandException(string message, IHelpProvider hp) : base($"Invalid argument: {message}\n{hp.Help(program)}")
            {
            }
            public CommandException(IHelpProvider hp) : base(hp.Help(program))
            {
            }
        }
        public CommandOptions CommandOptions { get; private set; }
        public CommandManager(string[] mainArguments)
        {
            if (mainArguments == null || mainArguments.Length < 1)
                throw new CommandException("No arguments found.", this);

            string command = mainArguments[0].ToUpper();
            switch (command)
            {
                case RubricGeneratorOptions.COMMAND:
                case RubricGeneratorOptions.COMMAND_EXTENDED:
                    CommandOptions = new RubricGeneratorOptions(mainArguments);
                    break;
                case AssessmentGeneratorOptions.COMMAND:
                case AssessmentGeneratorOptions.COMMAND_EXTENDED:
                    CommandOptions = new AssessmentGeneratorOptions(mainArguments);
                    break;
                case "--help":
                    throw new CommandException(this);
                default:
                    throw new CommandException($"{command} is not a valid command.", this);
            }
        }
        public string Help(string program)
        {
            return
                $"{program} Uses ...\n\n" +
                $"\t{program} <command>\n\n" +
                $"\t<command>\n" +
                $"\t\t{RubricGeneratorOptions.COMMAND} ({RubricGeneratorOptions.COMMAND_EXTENDED}):\n" +
                $"\t\t\t * To generate rubrics from an assessment template.\n" +
                $"\t\t{AssessmentGeneratorOptions.COMMAND} ({AssessmentGeneratorOptions.COMMAND_EXTENDED}):\n" +
                $"\t\t\t * To generate an assessment report from a rubrics folder.\n" +
                $"\nType for more information: {program} <command> --help\n";
        }

        public static bool IsHelpOption(string option)
        {
            return option == "--help" || option == "-h";
        }
    }
}
