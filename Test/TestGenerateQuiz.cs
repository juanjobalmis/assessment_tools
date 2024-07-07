namespace Test;

using System.Text;
using AssessmentTools.Commands;
using QuizGen;

public class TestGenerateQuiz
{
    public TestGenerateQuiz()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        if (!currentDirectory.EndsWith("assets"))
            Directory.SetCurrentDirectory(Path.Combine(currentDirectory, "..", "..", "..", "assets"));
    }

    // [Fact]
    // public void GenerateYamlQuizFromMock()
    // {        
    //     File.WriteAllText("QuizMock.yaml", QuizGeneratorMock.Get().ToYAML());
    // }

    [Fact]
    public void GenerateQuizBank()
    {
        // at qg -v -o BancoDePreguntas
        QuizGeneratorOptions options = new(new string[]
        {
            "ag",
            "-v",
            "-o",
            "BancoDePreguntas"
        });

        QuizGenerator.Generate(options);
    }
}