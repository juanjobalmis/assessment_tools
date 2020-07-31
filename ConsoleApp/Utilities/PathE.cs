using System;
using System.IO;
using System.Reflection;

namespace AssessmentTools.Utilities
{
    public class PathE
    {
        public static string ExecutableDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}
