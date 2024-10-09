using System.Text;

namespace Test;


public static class Preparacion
{
    private record StudentData(
        string NameCSV,
        string NameAules,
        string Group,
        string ID,
        string Mail,
        string NickName,
        string GitHubLogin,
        string NIA);

    private static List<StudentData> studentData = [
        new (
            NameCSV:"MUÑOZ SÁNCHEZ, MARÍA",
            NameAules:"MARÍA MUÑOZ SÁNCHEZ",
            Group:"A",
            ID:"10000011",
            Mail:"maría@alu.edu.gva.es",
            NickName:"muñoz",
            GitHubLogin:"",
            NIA:"10000011"),
        new (
            NameCSV:"SÁNCHEZ MARTÍNEZ, OSCAR",
            NameAules:"oscar sánchez martínez",
            Group:"A",
            ID:"10000012",
            Mail:"oscar@alu.edu.gva.es",
            NickName:"oscar",
            GitHubLogin:"",
            NIA:"10000012"),
        new (
            NameCSV:"OLEKSANDER, VÍCTOR",
            NameAules:"VÍCTOR OLEKSANDER",
            Group:"B",
            ID:"10000013",
            Mail:"victor@alu.edu.gva.es",
            NickName:"oleksander",
            GitHubLogin:"",
            NIA:"10000013"),
        new (
            NameCSV:"PÉREZ CASCALES, RUBÉN",
            NameAules:"ruben perez",
            Group:"B",
            ID:"10000014",
            Mail:"ruben@alu.edu.gva.es",
            NickName:"rubén",
            GitHubLogin:"",
            NIA:"10000014")
    ];

    public static void SituateEnDirectorioBase(string directorioBaseTest)
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
    private static void CreateStudentsCSV()
    {
        StringBuilder output = new("Name,Group,ID,Mail,NickName,GitHubLogin,NIA\n");
        
        studentData.ForEach(student => {
            output.Append($"\"{student.NameCSV}\",{student.Group},{student.ID},\"{student.Mail}\",{student.NickName},{student.GitHubLogin},{student.NIA}\n");
        });

        File.WriteAllText(".\\alumnos.csv", output.ToString());
    }

    private static void CreaDescargaAules()
    {
        for (int i = 0; i < studentData.Count; i++)
        {
            if (Directory.Exists(studentData[i].NameAules))
                Directory.Delete(studentData[i].NameAules, true);
            Directory.CreateDirectory($"{studentData[i].NameAules}_{new string(i.ToString()[0], 4)}");
        }
    }

    private static void BorraResultadosTestDeRubricas()
    {
        foreach (string entrada in Directory.GetFileSystemEntries(Directory.GetCurrentDirectory()))
        {
            bool esResultadoPrueba = !entrada.EndsWith("_rubric.xlsx");
            if (esResultadoPrueba)
            {
                if (entrada.EndsWith(".xlsx") || entrada.EndsWith(".csv"))
                    File.Delete(entrada);
                else
                    Directory.Delete(entrada, true);
            }
        }
    }

    public static void PreparaTestRubricas()
    {
        string directorioBaseTest = "rg";
        SituateEnDirectorioBase(directorioBaseTest);
        BorraResultadosTestDeRubricas();
        CreateStudentsCSV();
        CreaDescargaAules();
    }

    public static void PreparaTestAsessment()
    {
        string directorioBaseTest = "ag";
        SituateEnDirectorioBase(directorioBaseTest);
        CreateStudentsCSV();
    }

    public static void PreparaTestQuiz()
    {
        string directorioBaseTest = "qg";
        SituateEnDirectorioBase(directorioBaseTest);

        if (Directory.Exists("testsAules"))
            Directory.Delete("testsAules", true);

    }
}
