using System.Text;
using QuizGen;

namespace Test;

public class TestGenerateQuiz
{
    [Fact]
    public void SetActivityDirectory()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(Path.Combine(currentDirectory, "..", "..", "..", "Quizzes"));
    }
    [Fact]
    public void GenerateQuiz()
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
}