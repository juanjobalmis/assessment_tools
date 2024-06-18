using System;
using System.Collections.Generic;
using System.IO;

namespace AssessmentTools.Commands
{
    public abstract class CommandOptions : CommandManager.IHelpProvider
    {
        protected ushort ForcedOptionsSetMask = 0;
        public bool Verbose { get; protected set; }
        public bool Test { get; protected set; }
        public string StudentNamesFile { get; protected set; }

        protected CommandOptions(string[] mainArguments)
        {
            Verbose = false;
            Test = false;
            StudentNamesFile = null;
            ForcedOptionsSetMask = 0;

            if (mainArguments.Length <= 1)
                throw new CommandManager.CommandException($"{mainArguments[0]} command needs more options.", this);

            List<string> options = new List<string>(mainArguments);
            options.RemoveAt(0);

            if (!CommandManager.IsHelpOption(options[0]))
            {
                int optionIndex = 0;
                do
                {
                    CheckOption(options, ref optionIndex);
                    optionIndex++;
                } while (optionIndex < options.Count);

                CheckForcedOptionsNotIntroduced();
            }
            else
            {
                if (options.Count == 1)
                    throw new CommandManager.CommandException(this);
                else
                    throw new CommandManager.CommandException($"--help do not admit more options.", this);
            }
        }

        private void CheckForcedOptionsNotIntroduced()
        {
            List<string> fOpts = ForcedOptionsNotIntroduced;
            if (fOpts.Count > 0)
            {
                string msg = fOpts.Count == 1 ? "option" : "options";
                throw new CommandManager.CommandException($"The {String.Join(", ", fOpts)} {msg} must be set", this);
            }
        }

        protected abstract string Command { get; }
        protected abstract string CommandExtended { get; }
        protected abstract List<string> ForcedOptionsNotIntroduced { get; }
        protected List<string> ForcedOptionsNotInMask(Type type)
        {
            List<string> forcedOptionsNotIntroduced = new List<string>();
            foreach (var fo in Enum.GetValues(type))
            {
                if ((ForcedOptionsSetMask & (ushort)fo) == 0)
                    forcedOptionsNotIntroduced.Add($"--{fo}");
            }
            return forcedOptionsNotIntroduced;
        }

        protected void SetStudentNamesFile(List<string> options, ref int optionIndex)
        {
            string option = options[optionIndex++];
            StudentNamesFile = options[optionIndex];

            if (StudentNamesFile[0] == '-')
                throw new CommandManager.CommandException($"After {option} option, you have to specify a valid student names file.", this);

            if (!File.Exists(StudentNamesFile))
                throw new CommandManager.CommandException($"The student names file {StudentNamesFile} does not exist.", this);
        }

        public virtual string Help(string program)
        {
            return
                $"{program} {CommandExtended} Uses ...\n\n" +
                $"\t{program} {Command} --help\n" +
                $"\t{program} {Command} <options>\n\n";
        }
        protected enum CommonHelpOption { Verbose, Test, StudentNamesFile }
        protected static Dictionary<CommonHelpOption, string> CommonHelp()
        {
            Dictionary<CommonHelpOption, string> help = new Dictionary<CommonHelpOption, string>();
            string helpMsg;

            helpMsg = $"\t\t-v (--verbose):\n" +
            $"\t\t\t * To give feedback of the process.";
            help.Add(CommonHelpOption.Verbose, helpMsg);

            helpMsg = $"\t\t-t (--test):\n" +
            $"\t\t\t * To indicate we are testing the command.";
            help.Add(CommonHelpOption.Test, helpMsg);

            helpMsg = $"\t\t-s (--studentNamesFile) <studentNamesFile>:\n" +
            $"\t\t\t * Is a CSV file with ';' separator.\n" +
            $"\t\t\t * The following name colums are allowed: Name, ID, NickName, GitHubLogin, Mail, Group.\n" +
            $"\t\t\t\t - Name (required): Student name without accentuation symbols.\n" +
            $"\t\t\t\t - ID (required): Identifies the student and it has to be unique.\n" +
            $"\t\t\t\t   The ID number will help us as deliver's alias and for assessing Moodle assignments.\n" +
            $"\t\t\t\t - NickName (optinal): Possible student alias.\n" +
            $"\t\t\t\t - GitHubLogin (optinal): Possible student alias and to download and map GitHub roaster.\n" +
            $"\t\t\t\t - Mail (optinal): To give feedback if you don't use any VLE environment.\n" +
            $"\t\t\t\t - Group (optinal): To classify students in reports.";
            help.Add(CommonHelpOption.StudentNamesFile, helpMsg);
            return help;
        }


        protected abstract void CheckOption(List<string> options, ref int optionIndex);
    }
}
