using System;
using System.Collections.Generic;
using System.IO;
using QuizGen;

namespace AssessmentTools.Commands
{
    public static class QuizGenerator
    {
        private const string QUIZZIES_FOLDER = "BancoPreguntasParaMoodle";
        private const string QUIZ_BANK_FILENAME = "UnionBancosDePreguntas.xml";

        private static List<string> GetYamlFilesRecursively(string path)
        {
            List<string> yamlFiles = new List<string>();
            foreach (string file in Directory.GetFiles(path))
            {
                if (Path.GetExtension(file).ToLower() == ".yaml")
                    yamlFiles.Add(file);
            }
            foreach (string folder in Directory.GetDirectories(path))
            {
                yamlFiles.AddRange(GetYamlFilesRecursively(folder));
            }
            return yamlFiles;
        }

        public static void Generate(QuizGeneratorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options", "QuizGeneratorOptions needs to be instantiated.");

            if (!options.IsThereOutputFolder)
                throw new GeneratorException("QuizBankFilename is required.");

            if (options.Verbose)
                Console.WriteLine($"Generating quiz banks in {options.OutputFolder}...");

            List<string> yamlFiles = GetYamlFilesRecursively(Directory.GetCurrentDirectory());

            if (yamlFiles.Count == 0)
                Console.WriteLine($"No YAML files found in {Directory.GetCurrentDirectory()} tree.");

            Quiz globalQuestionBank = new() { Preguntas = new() };
            foreach (string yamlFile in yamlFiles)
            {
                Quiz quiz = Quiz.FromYAML(yamlFile);
                string outputFile = Path.GetFileName(yamlFile).Replace(".yaml", ".xml");
                quiz.SaveToXML(
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        options.OutputFolder,
                        outputFile
                    ));
                globalQuestionBank.AddQuestions(quiz.Preguntas);
                if (options.Verbose)
                    Console.WriteLine($"\tQuiz {outputFile} generated.");
            }
            globalQuestionBank.SaveToXML(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    options.OutputFolder,
                    QUIZ_BANK_FILENAME
                ));
            if (options.Verbose)
                Console.WriteLine($"\tGlobal quiz bank {QUIZ_BANK_FILENAME} generated.");
            Console.WriteLine("Quiz bank generated.");
        }
    }
}
