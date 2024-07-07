using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace QuizGen
{
    public class Quiz
    {
        public List<QuizQuestion> Preguntas { get; init; }

        public string ToYAML()
        {
            var yaml = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            return yaml.Serialize(this);
        }

        public override string ToString() => ToYAML();
    
        private static string GetCategory(string yamlPath) 
        {
            string[] quizCategories = Path.GetFileNameWithoutExtension(yamlPath).Split('-');
            return $"$course$/top/{string.Join<string>("/", quizCategories)}";
        }

        public static Quiz FromYAML(string yamlPath)
        {
            using StreamReader sR = new StreamReader(yamlPath, Encoding.UTF8);
            string yaml = sR.ReadToEnd();
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            Quiz q = deserializer.Deserialize<Quiz>(yaml) 
                     ?? throw new System.InvalidOperationException("Error deserializing YAML.");

            q.Preguntas.Insert(0, new QuizQuestion
            {
                Type = QuestionType.Description,
            });

            q.Preguntas.Insert(0, new QuizQuestion
            {
                Type = QuestionType.Category,
                Category = GetCategory(yamlPath)
            });

            return q;
        }

        public void SaveToXML(string pathXml)
        {
            using StreamWriter sW = new StreamWriter(pathXml, false, Encoding.UTF8);
            sW.WriteLine(ToXML());
        }

        public void AddQuestions(List<QuizQuestion> questions) => Preguntas.AddRange(questions);

        private XElement ToXML()
        {
            double defaultGrade = 10D / Preguntas.Count;

            var xml = new XElement("quiz");
            Preguntas.ForEach(
                question => xml.Add(question.ToXML(defaultGrade))
            );
            return xml;
        }
    }
}
