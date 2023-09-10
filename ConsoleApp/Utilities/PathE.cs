using System;
using System.IO;

namespace AssessmentTools.Utilities
{
    public class PathE
    {
        public static string ExecutableDirectory()
        {
            string codeBase = AppDomain.CurrentDomain.BaseDirectory;
            UriBuilder uri = new(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}
