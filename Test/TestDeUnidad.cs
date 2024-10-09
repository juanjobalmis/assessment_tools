namespace Test;

using System.Net;
using AssessmentTools.Commands;

public class TestDeUnidad
{
    #region TESTS RUBRICAS
    [Fact]
    public void TestGenerateRubricas()
    {
        Preparacion.PreparaTestRubricas();
        // atools rg -v -t -s alumnosRG.csv -r _rubric.xlsx
        RubricGeneratorOptions options = new(new string[]
        {
            "rg",
            "-v",
            "-t", // Test
            "-s",
            "alumnos.csv",
            "-r",
            "_rubric.xlsx"
        });

        RubricGenerator.Generate(options);
    }
    #endregion

    #region TESTS ASSESSMENTS
    [Fact]
    public void GenerateMoodleCsvAssessment()
    {
       Preparacion.PreparaTestAsessment();
        // atools ag -v -s alumnosAG.csv -p moodlecsv
        AssessmentGeneratorOptions options = new(new string[]
        {
            "ag",
            "-v",
            "-s",
            "alumnos.csv",
            "-p",
            "moodlecsv"
        });

        AssessmentGenerator.Generate(options);
    }
    
    [Fact]
    public void GenerateMoodleXmlAssessment()
    {
        Preparacion.PreparaTestAsessment();
        // atools ag -v -s alumnosAG.csv -p moodlexml
        AssessmentGeneratorOptions options = new(new string[]
        {
            "ag",
            "-v",
            "-s",
            "alumnos.csv",
            "-p",
            "moodlexml"
        });

        AssessmentGenerator.Generate(options);
    }

    [Fact]
    public void GenerateMoodleExcelAssessment()
    {
        Preparacion.PreparaTestAsessment();
        // atools ag -v -s alumnosAG.csv -p xlsx
        AssessmentGeneratorOptions options = new(new string[]
        {
            "ag",
            "-v",
            "-s",
            "alumnos.csv",
            "-p",
            "xlsx"
        });

        AssessmentGenerator.Generate(options);
    }
    #endregion

    #region TESTS QUIZ
    [Fact]
    public void GenerateQuizBank()
    {
        Preparacion.PreparaTestQuiz();
        // atools qg -v -t -o testsAules
        QuizGeneratorOptions options = new(new string[]
        {
            "qg",
            "-v",
            "-t",
            "-o",
            "testsAules"
        });

        QuizGenerator.Generate(options);
    }
    #endregion
}