namespace Test;

using System.Text;
using AssessmentTools.Commands;
using QuizGen;

public class TestGenerateQuiz
{
    [Fact]
    public void TestYamlParser()
    {
        using StreamReader sR = new StreamReader(
                   "encuesta_inicial_pmdm.yaml",
                   Encoding.UTF8);
        string yaml = sR.ReadToEnd();

        Quiz quiz = Quiz.FromYAML(yaml);
        Console.WriteLine(quiz);

        using StreamWriter sW = new StreamWriter(
            "encuesta_inicial_pmdm.xml",
            false,
            Encoding.UTF8);
        sW.WriteLine(quiz.ToXML(Path.GetFileNameWithoutExtension("encuesta_inicial_pmdm.xml")));
    }
    [Fact]
    public void GenerateQuizBank()
    {
        // at qg -v -q BancoDePreguntas.xml
        QuizGeneratorOptions options = new(new string[]
        {
            "ag",
            "-v",
            "-q",
            "BancoDePreguntas.xml"
        });

        QuizGenerator.Generate(options);
    }
}