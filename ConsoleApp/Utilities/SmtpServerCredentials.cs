using System;
using System.IO;


namespace AssessmentTools.Utilities
{
    [Serializable]
    public class SmtpServerCredentials
    {
        public string Host { get; private set; }
        public int Port { get; private set; }
        public string User { get; private set; }
        public string PassWord { get; private set; }
        public bool EnableSSL { get; private set; }
        private SmtpServerCredentials()
        {
            ;
        }
        public static SmtpServerCredentials CreateForGMail(string credentialsFile, bool askIfNotExist = true)
        {
            const string KEY = "secure";
            Credentials<SmtpServerCredentials> cm = new Credentials<SmtpServerCredentials>(credentialsFile);
            SmtpServerCredentials credentials;

            if (!File.Exists(credentialsFile))
            {
                if (askIfNotExist)
                {
                    Console.WriteLine("A GMail account credentials are needed...");
                    string user = ConsoleE.ReadEMail("User");
                    string passWord = ConsoleE.ReadPassword("Password");
                    credentials = new SmtpServerCredentials()
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        User = user,
                        PassWord = passWord,
                        EnableSSL = true
                    };
                    cm.Save(credentials, KEY);
                    Console.WriteLine($"Credentials have been saved in {credentialsFile}\n");                    
                }
                else throw new Exception($"Credentials file {credentialsFile} for mailing do not exist.");
            }
            else
            {
                credentials = cm.Load(KEY);
            }
            return credentials;
        }

        public override string ToString()
        {
            return $"Host: {Host}\n" +
                   $"Port: {Port:D}\n" +
                   $"Host: {User}\n" +
                   $"Pass: {"".PadRight(PassWord.Length, '*')}\n" +
                   $" SSL: {EnableSSL}";
        }
    }
}
