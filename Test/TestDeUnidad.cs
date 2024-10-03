namespace Test;

using System.Net;
using AssessmentTools.Commands;

public class TestDeUnidad
{

    private static void SituateEnDirectorioBase(string directorioBaseTest)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        if (!currentDirectory.EndsWith(directorioBaseTest))
        {
            if (Directory.Exists(Path.Combine(currentDirectory, "..", directorioBaseTest)))
                Directory.SetCurrentDirectory(Path.Combine(currentDirectory, "..", directorioBaseTest));
            else
                Directory.SetCurrentDirectory(Path.Combine(currentDirectory, "..", "..", "..", "assets", directorioBaseTest));
        }
    }

    private static void PreparaTestRubricas()
    {
        string directorioBaseTest = "rg";
        SituateEnDirectorioBase(directorioBaseTest);

        foreach (string entrada in Directory.GetFileSystemEntries(Directory.GetCurrentDirectory()))
        {
            bool esResultadoPrueba = !entrada.EndsWith("_rubric.xlsx") && !entrada.EndsWith("alumnosRG.csv");
            if (esResultadoPrueba)
            {
                if (entrada.EndsWith(".xlsx"))
                    File.Delete(entrada);
                else
                    Directory.Delete(entrada, true);
            }
        }

        string[] alumnos = {
            "NOMBRE1 APELLIDO18 APELLIDO19",
            "NOMBRE2 APELLIDO28 APELLIDO29",
            "NOMBRE3 APELLIDO38 APELLIDO39",
            "NOMBRE4 APELLIDÓ48 APELLIDO49"
        };

        for (int i = 0; i < alumnos.Length; i++)
        {
            if (Directory.Exists(alumnos[i]))
                Directory.Delete(alumnos[i], true);
            Directory.CreateDirectory($"{alumnos[i]}_{new String(i.ToString()[0], 4)}");
        }
    }

    private static void PreparaTestAsessment()
    {
        string directorioBaseTest = "ag";
        SituateEnDirectorioBase(directorioBaseTest);
    }

    private static void PreparaTestQuiz()
    {
        string directorioBaseTest = "qg";
        SituateEnDirectorioBase(directorioBaseTest);
        
        if (Directory.Exists("testsAules"))
            Directory.Delete("testsAules", true);

    }

    #region TESTS RUBRICAS
    [Fact]
    public void TestGenerateRubricas()
    {
        PreparaTestRubricas();
        // atools rg -v -t -s alumnosRG.csv -r _rubric.xlsx
        RubricGeneratorOptions options = new(new string[]
        {
            "rg",
            "-v",
            "-t", // Test
            "-s",
            "alumnosRG.csv",
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
        PreparaTestAsessment();
        // atools ag -v -s alumnosAG.csv -p moodlecsv
        AssessmentGeneratorOptions options = new(new string[]
        {
            "ag",
            "-v",
            "-s",
            "alumnosAG.csv",
            "-p",
            "moodlecsv"
        });

        AssessmentGenerator.Generate(options);
    }
    
    [Fact]
    public void GenerateMoodleXmlAssessment()
    {
        PreparaTestAsessment();
        // atools ag -v -s alumnosAG.csv -p moodlexml
        AssessmentGeneratorOptions options = new(new string[]
        {
            "ag",
            "-v",
            "-s",
            "alumnosAG.csv",
            "-p",
            "moodlexml"
        });

        AssessmentGenerator.Generate(options);
    }

    [Fact]
    public void GenerateMoodleExcelAssessment()
    {
        PreparaTestAsessment();
        // atools ag -v -s alumnosAG.csv -p xlsx
        AssessmentGeneratorOptions options = new(new string[]
        {
            "ag",
            "-v",
            "-s",
            "alumnosAG.csv",
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
        PreparaTestQuiz();
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