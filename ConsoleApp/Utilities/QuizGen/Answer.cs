
using System.Globalization;
using System.Xml.Linq;

namespace QuizGen
{
    public class Answer
    {
        public string Texto { get; init; }
        public bool Correcta { get; init; }

        public XElement ToXML(
            double correctPercentage = 100,
            double incorrectPercentage = 0
        )
        {
            double grade = Correcta ? correctPercentage : incorrectPercentage;
            return new XElement("answer",
                new XAttribute("fraction", grade.ToString("F3", CultureInfo.InvariantCulture)),
                new XAttribute("format", "html"),
                new XElement("text",
                    new XCData($"<p>{Texto}</p>")
                )
            );
        }
    }
}
