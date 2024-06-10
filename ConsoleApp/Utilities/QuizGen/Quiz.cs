using System.Collections.Generic;
using System.Xml.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace QuizGen
{
    public class Quiz
    {
        public List<QuizQuestion> Preguntas { get; init; }

        private string ToYAML()
        {
            var yaml = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            return yaml.Serialize(this);
        }

        public override string ToString() => ToYAML();

        public static Quiz FromYAML(string yaml)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            return deserializer.Deserialize<Quiz>(yaml);
        }

        /// <summary>
        /// El nombre del parámetro indicará la categoria en la que se incluirá la prueba
        /// puede describir subcategorias si en el nombre se incluye el caracter '-' (guión)
        /// si no se indica ninguna subcategoría, se incluirán en la subcategoría 'general'.
        /// </summary>
        public XElement ToXML(string nombreFichero = "general")
        {
            var quizInstructions = @"
            <p>Cada pregunta puede tener <b>una o más</b> de una respuestas correctas</p>.
            <p>La puntuación de cada pregunta se divide entre el número de respuestas correctas</p>.
            <p>Cada respuesta incorrecta resta también en proporción al número de posibilidades incorrectas</p>.
            <p>Tiene dos intentos para realizar la prueba y la calificación final será la media de ambos</p>.
            <p>Una vez finalice el plazo para realizar la prueba, podrá ver aquellos casos donde ha fallado y no podrá volver a realizarla</p>.
            ";

            var xml = new XElement("quiz");

            string[] quizCategories = nombreFichero.Split('-');

            xml.Add(new XElement("question",
                new XAttribute("type", "category"),
                new XElement("category",
                    new XElement("text", $"$course$/top/{string.Join<string>("/", quizCategories)}")
                )
            ));

            xml.Add(new XElement("question",
                new XAttribute("type", "description"),
                new XElement("name",
                    new XElement("text", "Instrucciones de la prueba")
                ),
                new XElement("questiontext",
                    new XAttribute("format", "html"),
                    new XElement("text",
                        new XCData(quizInstructions)
                    )
                ),
                new XElement("defaultgrade", "0.0000000"),
                new XElement("penalty", "0.0000000"),
                new XElement("hidden", "0")
            ));

            double defaultGrade = 10D / Preguntas.Count;
            Preguntas.ForEach(
                question => xml.Add(question.ToXML(defaultGrade))
            );
            return xml;
        }
    }
}
