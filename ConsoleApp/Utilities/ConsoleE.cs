using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AssessmentTools.Utilities
{
    public static class ConsoleE
    {
        private static void WriteLabel(string label)
        {
            if (label != null)
                Console.Write($"{label}: ");
        }

        public static string ReadEMail(string label)
        {
            string email;
            bool valid;
            do
            {
                WriteLabel(label);
                email = Console.ReadLine();
                valid = Regex.IsMatch(email, @"^[\w._%-]+@([\w.-]+\.)+[a-zA-Z]{2,4}$") == true;
                if (!valid)
                    Console.WriteLine($"{email} is not a valid e-mail address. Try again...");
            } while (!valid);
            return email;
        }
        private static string ReadLineEnhanced(bool hidden = true)
        {
            string text = "";
            do
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    text += key.KeyChar;
                    Console.Write(hidden ? "*" : key.KeyChar.ToString());
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && text.Length > 0)
                    {
                        text = text.Substring(0, (text.Length - 1));
                        Console.Write("\b \b");
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        Console.Write("\n");
                        break;
                    }
                }
            } while (true);
            return text;
        }

        public static string ReadPassword(string label, bool hidden=true)
        {
            string passWord;
            bool valid;
            do
            {
                WriteLabel(label);
                passWord = ReadLineEnhanced(hidden);
                valid = passWord.Length > 0;
                if (!valid)
                    Console.WriteLine($"The Password must have at least one character. Try again...");
            } while (!valid);
            return passWord;
        }

        private static string TextsEnumOptions<T>() where T : Enum {
            List<String> opciones = new();
            foreach (int option in (int[])Enum.GetValues(typeof(T)))
                opciones.Add($"{option} = {(T)Enum.ToObject(typeof(T), option)}");
            return string.Join(", ", opciones);
        }


        public static T ReadEnumOption<T>() where T : Enum
        {
            int option;
            bool valid;
            var validOptions = (int[])Enum.GetValues(typeof(T));
            string label = $"Options ({TextsEnumOptions<T>()})";
            do
            {
                WriteLabel(label);
                valid = int.TryParse(Console.ReadLine(), out option);
                if (valid)
                    valid = validOptions.Contains(option);
                if (!valid)
                    Console.WriteLine($"{option} is not a valid mut be {string.Join(", ", validOptions)}. Try again...");
            } while (!valid);
            return (T)Enum.ToObject(typeof(T), option);
        }
    }
}
