using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using YamlDotNet.Serialization;

namespace QuizGen
{
    public class QuizQuestion
    {

        public string Enunciado { get; init; }
        public List<Answer> Respuestas { get; init; }

        [YamlIgnore]
        public QuestionType Type { get; init; } = QuestionType.Multichoice;
        [YamlIgnore]
        public string Category { get; init; } = default;

        public XElement ToXML(double defaultGrade = 1.0D)
        {
            return Type switch
            {
                QuestionType.Multichoice => MultichoiceToXML(defaultGrade),
                QuestionType.Category => CategoryToXML(),
                QuestionType.Description => DescriptionToXML(),
                _ => throw new System.NotImplementedException("Type of question not implemented yet.")
            };
        }

        private XElement MultichoiceToXML(double grade)
        {
            if (Type != QuestionType.Multichoice)
                throw new System.InvalidOperationException("This method is only for multichoice questions.");

            var xml = new XElement(
                "question",
                new XAttribute("type", Type.ToString().ToLower()),
                new XElement("name",
                    new XElement("text", "")
                ),
                new XElement("questiontext",
                    new XAttribute("format", "html"),
                    new XElement("text",
                        new XCData($"<p>{Enunciado}</p>")
                    )
                ),
                new XElement("defaultgrade", grade.ToString(CultureInfo.InvariantCulture)),
                new XElement("penalty", "0.0000000"),
                new XElement("hidden", "0"),
                new XElement("single", "false"),
                new XElement("shuffleanswers", "true"),
                new XElement("answernumbering", "abc"),
                new XElement("showstandardinstruction", "0"),
                new XElement("correctfeedback",
                    new XAttribute("format", "html"),
                    new XElement("text", "Respuesta correcta")
                ),
                new XElement("partiallycorrectfeedback",
                    new XAttribute("format", "html"),
                    new XElement("text", "Respuesta parcialmente correcta.")
                ),
                new XElement("incorrectfeedback",
                    new XAttribute("format", "html"),
                    new XElement("text", "Respuesta incorrecta.")
                ),
                new XElement("shownumcorrect")
            );

            int correctAnswersCount = Respuestas.Count(respuesta => respuesta.Correcta);
            double correctPercentage = 100D / correctAnswersCount;
            double incorrectPercentage = -100D / (Respuestas.Count - correctAnswersCount);

            Respuestas.ForEach(
                answer => xml.Add(answer.ToXML(
                    correctPercentage, incorrectPercentage
                ))
            );

            return xml;
        }

        private XElement CategoryToXML()
        {
            if (Type != QuestionType.Category)
                throw new System.InvalidOperationException("This method is only for category questions.");
            if (string.IsNullOrEmpty(Category))
                throw new System.InvalidOperationException("Category is empty.");
            var xml = new XElement("question",
                new XAttribute("type", "category"),
                new XElement("category",
                    new XElement("text", Category)
                )
            );

            return xml;
        }

        private XElement DescriptionToXML()
        {
            if (Type != QuestionType.Description)
                throw new System.InvalidOperationException("This method is only for description questions.");

            var quizInstructions = @"
            <p>Instrucciones:</p>
            <p>
            <ol>
            <li>Cada pregunta puede tener <b>una o más</b> de una respuestas correctas, lee bien el enunciado puedes darte pistas.</li>
            <li>La puntuación de cada pregunta se divide entre el número de respuestas correctas.</li>
            <li>Cada respuesta incorrecta resta también en proporción al número de posibilidades incorrectas.</li>
            <li>Tienes dos intentos para realizar la prueba y la calificación final será la más alta.</li>
            <li>Una vez finalice el plazo para realizar la prueba, podrá ver aquellos casos donde ha fallado y <b>no</b> podrá volver a realizarla.</li>
            </ol>
            </p>
            ";

            var xml = new XElement("question",
                new XAttribute("type", Type.ToString().ToLower()),
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
            );

            return xml;
        }
    }
}
