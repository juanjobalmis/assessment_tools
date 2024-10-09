using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using AssessmentTools.Data;

namespace AssessmentTools.Commands
{
    public static class RubricGenerator
    {
        private static string RemoveAccentuationSymbols(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            string nomalized = stringBuilder.ToString().Normalize(NormalizationForm.FormC);
            return nomalized;
        }

        private static string NormalizeFolderName(this string name)
        {
            string nameNormalized;

            nameNormalized = name.RemoveAccentuationSymbols().ToUpper();
            int inicio = nameNormalized.LastIndexOf('_');
            nameNormalized = (inicio >= 1) ? nameNormalized.Remove(inicio, nameNormalized.Length - inicio) : nameNormalized;
            return nameNormalized.Replace("_", " ");
        }
        private static string NormalizeAliasName(this string name) => name.RemoveAccentuationSymbols().ToUpper();

        private static void RenameFolder(string f1, string f2)
        {
            try
            {
                string swapF = $"{f1}_TEMP_";
                Directory.Move(f1, swapF);
                Directory.Move(swapF, f2);
            }
            catch (Exception e)
            {
                throw new GeneratorException($"Impossible yo rename {f1} to {f2}", e);
            }
        }

        private static void GenerateRubrics(List<string> folders, RubricGeneratorOptions options)
        {
            List<string> rubricFileNames = new List<string>();
            folders.ForEach(f =>
            {
                f = Path.GetFileName(f);
                rubricFileNames.Add($"{f}{options.TemplateExtension}");
            });
            rubricFileNames.ForEach(r =>
            {
                if (options.Verbose)
                    Console.WriteLine($"Creating {r} ...");
                File.Copy(options.RubricTemplate, r);
            });
        }

        private static List<string> NormalizeFolderNames(List<string> folders, bool verbose = false)
        {
            List<string> renamedFolders = new();
            folders.ForEach(f =>
            {
                string target = Path.Combine(Path.GetDirectoryName(f), Path.GetFileName(f).NormalizeFolderName());
                renamedFolders.Add(target);
                string log;
                if (f != target)
                {
                    log = $"Renaming {Path.GetFileName(f)} to normalized name {Path.GetFileName(target)}.";
                    RenameFolder(f, target);
                }
                else
                    log = $"{Path.GetFileName(f)} is already normalized.";

                if (verbose)
                    Console.WriteLine(log);
            });
            return renamedFolders;
        }

        private static void RenameFoldersWithStudentNamesAsociation(Dictionary<string, string> studetNameToFolderAsociation, bool verbose = false)
        {
            foreach (var asociation in studetNameToFolderAsociation)
            {
                string namedFolder = asociation.Key;
                string deliveredFolder = asociation.Value;

                if (Path.GetDirectoryName(namedFolder) != Path.GetDirectoryName(deliveredFolder))
                    throw new GeneratorException("Delivered and new student named folder must be in the same path.");

                string log;
                if (deliveredFolder != namedFolder)
                {
                    log = $"Renaming {Path.GetFileName(deliveredFolder)} to student name {Path.GetFileName(namedFolder)}.";
                    RenameFolder(deliveredFolder, namedFolder);
                }
                else
                    log = $"Delivered folder {Path.GetFileName(deliveredFolder)} is OK !! (Matched with a student name)";

                if (verbose)
                    Console.WriteLine(log);
            }
        }

        private static void DeflatingFoldersContent(List<string> folders, bool verbose = false)
        {
            folders.ForEach(folder =>
            {
                string[] compressedFiles = Directory.GetFiles(folder, "*.zip", SearchOption.TopDirectoryOnly);
                foreach (var file in compressedFiles)
                {
                    try
                    {
                        if (verbose)
                            Console.WriteLine($"Extracting {file} into {folder}");
                        ZipFile.ExtractToDirectory(file, folder);
                        if (verbose)
                            Console.WriteLine($"Successfully extracted, deleting {file} ...");
                        File.Delete(file);
                    }
                    catch (Exception e)
                    {
                        if (verbose)
                            Console.WriteLine($"\tError extracting {file}: {e.Message}");
                    }
                }
            });
        }

        private static bool AskForValidation(string question)
        {
            char option;
            bool correctOption;
            do
            {
                Console.Write($"{question} (y/n): ");
                option = char.ToUpper(Console.ReadKey().KeyChar);
                Console.Write("\n\n");
                correctOption = option == 'Y' || option == 'N';
                if (!correctOption)
                    Console.WriteLine($"Invalid option. Please type y or n.");
            } while (!correctOption);
            return option == 'Y';
        }

        private static List<string> RemoveFoldersNotInValidFolders(
            List<string> validForlders, 
            bool testing = false)
        {
            List<string> folders = new List<string>(Directory.GetDirectories(Directory.GetCurrentDirectory()));
            int i = 0;
            while (i < folders.Count)
            {
                if (validForlders.FindIndex(ftk => folders[i].ToUpper().IndexOf(ftk.ToUpper()) >= 0) < 0)
                {
                    string message = $"{folders[i]} not found in the names list.";
                    
                    if (testing)
                        throw new GeneratorException(message);

                    if (AskForValidation($"{testing}\nDou you want to remove it?"))
                    {
                        Console.WriteLine($"Removing {folders[i]} ...");
                        Directory.Delete(folders[i], true);
                        folders.RemoveAt(i);
                    }
                    else i++;
                }
                else i++;
            }
            return folders;
        }

        private static bool IsFolderInStudentNameOrAliasSubstring(string folder, List<string> aliasesForAStudent)
        {
            string name = Path.GetFileName(folder).ToUpper();
            return aliasesForAStudent.Select(alias => alias.NormalizeAliasName())
                                     .Where(alias => alias != "" && name.IndexOf(alias) >= 0)
                                     .Count() > 0;
        }

        private static bool IsFolderInStudentNameOrAliasHeuristic(string folder, List<string> aliasesForAStudent)
        {
            bool match;
            List<string> wordsInFolder = new List<string>(Path.GetFileName(folder).Split(" ,;_-.".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            match = wordsInFolder.Count < 1;
            if (!match)
                match = wordsInFolder.FindIndex(w => IsFolderInStudentNameOrAliasSubstring(w, aliasesForAStudent)) >= 0;
            return match;
        }

        private static string AskUserForMultipleMatch(
                                List<string> proposedNames,
                                string deliveredFolder,
                                bool testing = false)
        {
            int option;
            bool correctOption;
            do
            {
                string message = $"There is multiple choices for delivered folder {Path.GetFileName(deliveredFolder)}";

                if (testing)
                    throw new GeneratorException(message);

                Console.WriteLine(message);                
                Console.Write("Select the number with the correct name: ");
                Console.WriteLine();
                option = 1;
                proposedNames.ForEach(n => Console.WriteLine($"{option++}.- {n}"));
                correctOption = int.TryParse(Console.ReadKey().KeyChar.ToString(), out option);
                if (correctOption)
                    correctOption = option > 0 && option <= proposedNames.Count;
                if (!correctOption)
                    Console.WriteLine($"Invalid option. Type a number between {1} and {proposedNames.Count}");


            } while (!correctOption);
            return proposedNames[option - 1];
        }

        private static void AddProposedNamesToAsociation(
                                Dictionary<string, string> namedToDeliveredFolderAsociation,
                                List<string> proposedNames,
                                string deliveredFolder,
                                bool testing = false)
        {
            string name = proposedNames.Count == 1 
                        ? proposedNames[0] 
                        : AskUserForMultipleMatch(proposedNames, deliveredFolder, testing);
            name = name.ToUpper();
            string namedFolder = Path.Combine(Path.GetDirectoryName(deliveredFolder), name);

            if (namedToDeliveredFolderAsociation.ContainsKey(namedFolder))
            {
                string message  = $"The student name entry {name} aosciated to\n";
                message += $"{Path.GetFileName(namedToDeliveredFolderAsociation[namedFolder])} already exists.";

                if (testing)
                    throw new GeneratorException(message);

                Console.WriteLine(message);
                string question = $"Do you want to replace the association with {Path.GetFileName(deliveredFolder)}?";
                if (AskForValidation(question))
                    namedToDeliveredFolderAsociation[namedFolder] = deliveredFolder;
            }
            else
                namedToDeliveredFolderAsociation.Add(namedFolder, deliveredFolder);
        }

        private static Dictionary<string, string> NamedToDeliveredFolderAsociation(
                                                        Dictionary<string, List<string>> studentNamesWithAlias,
                                                        List<string> deliveredFolders,
                                                        bool testing = false)
        {
            Dictionary<string, string> namedToDeliveredFolderAsociation = new Dictionary<string, string>();
            foreach (var deliveredFolder in deliveredFolders)
            {
                string deliveredFolderWithOutAccents = Path.Combine(
                                                                Path.GetDirectoryName(deliveredFolder),
                                                                Path.GetFileName(deliveredFolder).NormalizeFolderName());
                var proposedNames = studentNamesWithAlias.Where(nameWithAlias => IsFolderInStudentNameOrAliasSubstring(deliveredFolderWithOutAccents, nameWithAlias.Value))
                                                         .Select(nameWithAlias => nameWithAlias.Key).ToList();
                if (proposedNames.Count == 0)
                    proposedNames = studentNamesWithAlias.Where(nameWithAlias => IsFolderInStudentNameOrAliasHeuristic(deliveredFolderWithOutAccents, nameWithAlias.Value))
                                                         .Select(nameWithAlias => nameWithAlias.Key).ToList();
                if (proposedNames.Count > 0)
                    AddProposedNamesToAsociation(namedToDeliveredFolderAsociation, proposedNames, deliveredFolder, testing);
            }
            return namedToDeliveredFolderAsociation;
        }

        public static void Generate(RubricGeneratorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options", "RubricGeneratorOptions needs to be instantiated.");
            List<string> deliveredFolders = new List<string>(Directory.GetDirectories(Directory.GetCurrentDirectory()));
            if (deliveredFolders.Count == 0)
                throw new GeneratorException($"No sub-folders found in {Directory.GetCurrentDirectory()}");

            List<string> namedFolders;
            if (options.IsThereStudentNamesFile)
            {
                Dictionary<string, string> namedToDeliveredFolderAsociation = NamedToDeliveredFolderAsociation(
                                                StudentsData.NamesWithAlias(options.StudentNamesFile),
                                                deliveredFolders,
                                                options.Test);
                RenameFoldersWithStudentNamesAsociation(namedToDeliveredFolderAsociation, options.Verbose);
                namedFolders = RemoveFoldersNotInValidFolders(namedToDeliveredFolderAsociation.Keys.ToList(), options.Test);
            }
            else
            {
                // Si no hay fichero de estudiantes normalizo los nombres.
                namedFolders = NormalizeFolderNames(deliveredFolders, options.Verbose);
            }

            DeflatingFoldersContent(namedFolders, options.Verbose);
            GenerateRubrics(namedFolders, options);

            Console.WriteLine("Rubric files successfully generated.");
        }
    }
}
