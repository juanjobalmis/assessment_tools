using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AssessmentTools.Data
{
    
    public class Student : IEntityInfoToAssess
    {
        private const string INVALID_ID = "unknown";
        private string name;

        public string Name
        {
            get { return name; }
            protected set
            {
                if (value == null || value == "")
                    throw new StudentsDataException("Invalid estudent name");

                name = value;
            }
        }

        private string id;

        public string ID
        {
            get { return id; }
            protected set
            {
                if (value == null || value == "")
                {
                    id = INVALID_ID;
                    Console.WriteLine($"Invalid student ID for {Name}");
                }
                else
                    id = value;
            }
        }

        public bool ValidEmail()
            => Regex.IsMatch(Mail, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

        public bool ValidID()
        {
            return id != INVALID_ID;
        }

        public string NickName { get; protected set; }
        public string GitHubLogin { get; protected set; }
        public string Mail { get; protected set; }
        public string Group { get; protected set; }

        private Student()
        {
            NickName = "";
            GitHubLogin = "";
            Mail = "";
            Group = "";
        }

        public static Student CSVDataFactory(
                                        List<string> studentCSVData,
                                        Dictionary<string, int> mapPropetyToColIndex)
        {
            Student s = new Student();

            string[] propertiesCSVMustContain = new string[] { "Name", "ID", "Mail" };
            foreach (var property in propertiesCSVMustContain)
                if (!mapPropetyToColIndex.ContainsKey(property))
                    throw new ArgumentException($"The CSV file do not define {property} column.");

            foreach (var propertyName in mapPropetyToColIndex.Keys)
                s.GetType().GetProperty(propertyName).SetValue(s, studentCSVData[mapPropetyToColIndex[propertyName]]);
            return s;
        }

        public static List<string> PropertyNames
        {
            get
            {
                return new Student().GetType().GetProperties().Select(p => p.Name).ToList();
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
