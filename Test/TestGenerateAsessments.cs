namespace Test;

using AssessmentTools.Commands;

public class TestGenerateAsessments
{
    [Fact]
    public void SetActivityDirectory()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(Path.Combine(currentDirectory, "..", "..", "..", "Actividad1"));
    }
    [Fact]
    public void GenerateMoodleCsvAssessment()
    {
        // at ag -v -s alumnos.csv -t moodlecsv
        AssessmentGeneratorOptions options = new(new string[]
        {
            "ag",
            "-v",
            "-s",
            "alumnos.csv",
            "-t",
            "moodlecsv"
        });

        AssessmentGenerator.Generate(options);
    }
    [Fact]
    public void GenerateMoodleXmlAssessment()
    {
        // at ag -v -s alumnos.csv -t moodlexml
        AssessmentGeneratorOptions options = new(new string[]
        {
            "ag",
            "-v",
            "-s",
            "alumnos.csv",
            "-t",
            "moodlexml"
        });

        AssessmentGenerator.Generate(options);
    }
    [Fact]
    public void GenerateMoodleExcelAssessment()
    {
        // at ag -v -s alumnos.csv -t xlsx
        AssessmentGeneratorOptions options = new(new string[]
        {
            "ag",
            "-v",
            "-s",
            "alumnos.csv",
            "-t",
            "xlsx"
        });

        AssessmentGenerator.Generate(options);
    }    
}