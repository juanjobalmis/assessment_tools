namespace Test;

using AssessmentTools.Commands;

public class TestGenerateAsessments
{
    public TestGenerateAsessments()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        if (!currentDirectory.EndsWith("assets"))
            Directory.SetCurrentDirectory(Path.Combine(currentDirectory, "..", "..", "..", "assets"));
    }
    [Fact]
    public void GenerateMoodleCsvAssessment()
    {
        // at ag -v -s alumnos.csv -p moodlecsv
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
        // at ag -v -s alumnos.csv -p moodlexml
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
        // at ag -v -s alumnos.csv -p xlsx
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
}