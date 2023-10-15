using System;
using System.IO;
using System.Text;


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

        private enum ServerOption { GMail = 1, Office365 = 2 }

        private static string Server(ServerOption option)
        => option switch 
        { 
            ServerOption.GMail => "smtp.gmail.com", 
            ServerOption.Office365 => "smtp.office365.com", 
            _ => throw new Exception("Invalid server option") 
        };


        public static SmtpServerCredentials Create(
                string credentialsFile,
                bool askIfNotExist = true)
        {
            const string KEY = "secure";
            Credentials<SmtpServerCredentials> cm = new Credentials<SmtpServerCredentials>(credentialsFile);
            SmtpServerCredentials credentials;

            if (!File.Exists(credentialsFile))
            {
                if (askIfNotExist)
                {
                    Console.WriteLine($"A SMTP Server account credentials are needed...");
                    string smtpServer = Server(ConsoleE.ReadEnumOption<ServerOption>());
                    string user = ConsoleE.ReadEMail("User");
                    string passWord = ConsoleE.ReadPassword("Password");
                    credentials = new SmtpServerCredentials()
                    {
                        Host = smtpServer,
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
