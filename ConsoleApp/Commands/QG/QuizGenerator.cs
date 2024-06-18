using System;

namespace AssessmentTools.Commands
{
    public static class QuizGenerator
    {
        public static void Generate(QuizGeneratorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options", "QuizGeneratorOptions needs to be instantiated.");
            // List<string> deliveredFolders = new List<string>(Directory.GetDirectories(Directory.GetCurrentDirectory()));
            // if (deliveredFolders.Count == 0)
            //     throw new GeneratorException($"No sub-folders found in {Directory.GetCurrentDirectory()}");

        }
    }
}
