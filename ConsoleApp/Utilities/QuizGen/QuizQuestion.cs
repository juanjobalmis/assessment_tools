using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace QuizGen
{
    public class QuizQuestion
    {
        public string Enunciado { get; init; }
        public List<Answer> Respuestas { get; init; }

        public XElement ToXML(double defaultGrade = 1.0D)
        {
            var xml = new XElement(
                "question",
                new XAttribute("type", "multichoice"),
                new XElement("name",
                    new XElement("text", "")
                ),
                new XElement("questiontext",
                    new XAttribute("format", "html"),
                    new XElement("text",
                        new XCData($"<h4>{Enunciado}</h4>")
                    )
                ),
                new XElement("defaultgrade", defaultGrade.ToString(CultureInfo.InvariantCulture)),
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
    }
}
