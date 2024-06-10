using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace AssessmentTools.Utilities
{
    [Serializable]
    public class SmtpServerCredentials
    {
        private const string KEY = "secure";
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string PassWord { get; set; }
        public bool EnableSSL { get; set; }
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

        static string EncryptString(string plaintext)
        {
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            Rfc2898DeriveBytes passwordBytes = new(KEY, 20);

            var encryptor = Aes.Create();
            encryptor.Key = passwordBytes.GetBytes(32);
            encryptor.IV = passwordBytes.GetBytes(16);
            using MemoryStream ms = new();
            using CryptoStream cs = new(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(plaintextBytes, 0, plaintextBytes.Length);
            return Convert.ToBase64String(ms.ToArray());
        }
 
        static string DecryptString(string encrypted)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encrypted);
            Rfc2898DeriveBytes passwordBytes = new(KEY, 20);
            var encryptor = Aes.Create();
            encryptor.Key = passwordBytes.GetBytes(32);
            encryptor.IV = passwordBytes.GetBytes(16);
            using MemoryStream ms = new();
            using CryptoStream cs = new(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(encryptedBytes, 0, encryptedBytes.Length);
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static SmtpServerCredentials Create(
                string credentialsFile,
                bool askIfNotExist = true)
        {
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
                    cm.Save(credentials);
                    Console.WriteLine($"Credentials have been saved in {credentialsFile}\n");
                }
                else throw new Exception($"Credentials file {credentialsFile} for mailing do not exist.");
            }
            else
            {
                credentials = cm.Load();
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
