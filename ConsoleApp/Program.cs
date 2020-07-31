using System;
using AssessmentTools.Commands;

namespace AssessmentTools
{
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CommandManager cm = new CommandManager(args);

                switch (cm.CommandOptions)
                {
                    case RubricGeneratorOptions rgo:
                        RubricGenerator.Generate(rgo);
                        break;
                    case AssessmentGeneratorOptions ago:
                        AssessmentGenerator.Generate(ago);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                do
                {
                    Console.WriteLine(e.Message);
                    e = e.InnerException;
                } while (e != null);
            }
        }
    }
}