
using System.Collections.Generic;

namespace QuizGen
{
    public static class QuizGeneratorMock
    {
        static public Quiz Get()
        {
            return new Quiz
            {
                Preguntas = new List<QuizQuestion>
                {
                    new QuizQuestion
                    {
                        Enunciado = "¿Qué condición mínima es necesaria para <b>ejecutar</b> aplicaciones .NET en un sistema operativo?",
                        Respuestas = new List<Answer>
                        {
                            new Answer { Texto = "Tener el CLR instalado.", Correcta = true },
                            new Answer { Texto = "Tener el Runtime instalado.", Correcta = true },
                            new Answer { Texto = "Tener el SDK .NET Framework instalado.", Correcta = false },
                            new Answer { Texto = "Utilizar el sistema operativo Windows.", Correcta = false }
                        }
                    },
                    new QuizQuestion
                    {
                        Enunciado = "¿Qué es un <b>ensamble</b> o <b>ensamblado</b> en .NET?",
                        Respuestas = new List<Answer>
                        {
                            new Answer { Texto = "Es un fichero con el código ensamblador nativo de la máquina.", Correcta = false },
                            new Answer { Texto = "Es un fichero que contiene solo el código en IL para el CLR.", Correcta = false },
                            new Answer { Texto = "Es un fichero que contiene el código en IL más otro contenido necesario para el CLR", Correcta = true },
                            new Answer { Texto = "Son <b>siempre</b> ejecutables.", Correcta = false }
                        }
                    },
                    new QuizQuestion
                    {
                        Enunciado = "Busca en Internet y deduce que queremos decir con <b><em>'Ingeniería Inversa'</em></b> del IL",
                        Respuestas = new List<Answer>
                        {
                            new Answer { Texto = "Crackear un programa que precisa de número de licencia para funcionar.", Correcta = false },
                            new Answer { Texto = "Utilizar algún tipo de Ramsonware para cifrar el IL.", Correcta = false },
                            new Answer { Texto = "Deducir el código de alto nivel o cómo se hizo un prográma a partir de su IL.", Correcta = true },
                            new Answer { Texto = "Ninguna de las respuestas anteriores es cierta.", Correcta = false }
                        }
                    },
                    new QuizQuestion
                    {
                        Enunciado = "¿Qué diferencia hay entre <b>.NET Framework</b> y el <b>.NET Core</b>?",
                        Respuestas = new List<Answer>
                        {
                            new Answer { Texto = "El .NET Core está pensado para desarrollos basados en Microservicios en la nube, CI/CD y Contenedores", Correcta = true },
                            new Answer { Texto = "El .NET Framework es más moderno.", Correcta = false },
                            new Answer { Texto = "El .NET Core es multiplataforma.", Correcta = true },
                            new Answer { Texto = "El .NET Core permite usar versiones superiores del lenguaje C#.", Correcta = true }
                        }
                    },
                    new QuizQuestion
                    {
                        Enunciado = "¿Cual será la <b>siguiente</b> versión con LTS de .NET?",
                        Respuestas = new List<Answer>
                        {
                            new Answer { Texto = ".NET Core 5", Correcta = false },
                            new Answer { Texto = ".NET 5", Correcta = false },
                            new Answer { Texto = ".NET 6", Correcta = true },
                            new Answer { Texto = ".NET Core 3.3", Correcta = false }
                        }
                    },
                    new QuizQuestion
                    {
                        Enunciado = "Indica cual de las siguientes afirmaciones es <b>cierta</b>.",
                        Respuestas = new List<Answer>
                        {
                            new Answer { Texto = "El lenguaje C# no está estandarizado.", Correcta = false },
                            new Answer { Texto = "C#9 no se podrá usar hasta la versión .NET 5", Correcta = true },
                            new Answer { Texto = ".NET 5 es de código abierto", Correcta = true },
                            new Answer { Texto = ".NET solo permite desarrollos en un único lenguaje de programación que es C#", Correcta = false }
                        }
                    },
                    new QuizQuestion
                    {
                        Enunciado = "¿Qué es el JIT?",
                        Respuestas = new List<Answer>
                        {
                            new Answer { Texto = "Se encarga del proceso de compilación a IL.", Correcta = false },
                            new Answer { Texto = "Se encarga de traducir el IL al código nativo de la máquina.", Correcta = true },
                            new Answer { Texto = "Son unas librerías de código que vienen por defecto en la propia plataforma .NET.", Correcta = false },
                            new Answer { Texto = "Se encarga de leer los metadatos de un ensamblado.", Correcta = false }
                        }
                    },
                    new QuizQuestion
                    {
                        Enunciado = "Indica cual de las siguientes afirmaciones es <b>falsa</b>.",
                        Respuestas = new List<Answer>
                        {
                            new Answer { Texto = "El <b>CLS</b> contiene son los tipos comunes a todos los lenguajes que se pueden usar en la plataforma y así asegurar interoprabilidad entre ellos.", Correcta = false },
                            new Answer { Texto = "El <b>CTS</b> contiene son los tipos comunes a todos los lenguajes que se pueden usar en la plataforma y así asegurar interoprabilidad entre ellos.", Correcta = true },
                            new Answer { Texto = "Las BCL son unas librerías.", Correcta = false },
                            new Answer { Texto = "El espacio de nombres <b>System.IO</b> define los tipos que me permitirán realizar la entrada y salida por consola.", Correcta = false }
                        }
                    }
                }
            };
        }
    }
}