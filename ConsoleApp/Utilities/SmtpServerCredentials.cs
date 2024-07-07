using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace AssessmentTools.Utilities
{
    [Serializable]
    public class SmtpServerCredentials : ICloneable
    {
        private const string KEY = "secure";
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string PassWord { get; set; }
        public bool EnableSSL { get; set; }

        private enum ServerOption { GMail = 1, Office365 = 2 }

        private static string Server(ServerOption option)
        => option switch
        {
            ServerOption.GMail => "smtp.gmail.com",
            ServerOption.Office365 => "smtp.office365.com",
            _ => throw new Exception("Invalid server option")
        };

        private static byte[] GenerateRandomSalt()
        {
            byte[] salt = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private static string EncryptString(string plaintext)
        {
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            byte[] saltBytes = GenerateRandomSalt();
            using (Rfc2898DeriveBytes passwordBytes = new(KEY, saltBytes, 10000)) // Incrementamos el conteo de iteraciones
            {
                using (Aes encryptor = Aes.Create())
                {
                    encryptor.Key = passwordBytes.GetBytes(32);
                    encryptor.IV = passwordBytes.GetBytes(16);
                    using (MemoryStream ms = new())
                    {
                        ms.Write(saltBytes, 0, saltBytes.Length); // Almacenamos la sal al principio del stream
                        using (CryptoStream cs = new(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(plaintextBytes, 0, plaintextBytes.Length);
                            cs.FlushFinalBlock(); // Aseguramos que todos los datos se escriban
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        private static string DecryptString(string encrypted)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encrypted);
            byte[] saltBytes = new byte[16];
            Array.Copy(encryptedBytes, 0, saltBytes, 0, saltBytes.Length); // Extraemos la sal

            using (Rfc2898DeriveBytes passwordBytes = new(KEY, saltBytes, 10000)) // Mismo conteo de iteraciones
            {
                using (Aes decryptor = Aes.Create())
                {
                    decryptor.Key = passwordBytes.GetBytes(32);
                    decryptor.IV = passwordBytes.GetBytes(16);
                    using (MemoryStream ms = new())
                    {
                        using (CryptoStream cs = new(ms, decryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(encryptedBytes, saltBytes.Length, encryptedBytes.Length - saltBytes.Length);
                            cs.FlushFinalBlock(); // Aseguramos que todos los datos se escriban
                        }
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }

        public static SmtpServerCredentials SimpleFactory(
                string credentialsFile,
                bool askIfNotExist = true)
        {
            Credentials<SmtpServerCredentials> cm = new Credentials<SmtpServerCredentials>(credentialsFile);
            SmtpServerCredentials credentials;

            if (!File.Exists(credentialsFile))
            {
                if (askIfNotExist)
                {
                    Console.WriteLine($"A GMail SMTP Server account credentials are needed...");
                    // string smtpServer = Server(ConsoleE.ReadEnumOption<ServerOption>());
                    string smtpServer = Server(ServerOption.GMail);
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
                    SmtpServerCredentials cifradas = credentials.Clone() as SmtpServerCredentials;
                    cifradas.PassWord = EncryptString(cifradas.PassWord);
                    cm.Save(cifradas);
                    Console.WriteLine($"Credentials have been saved in {credentialsFile}\n");
                }
                else throw new Exception($"Credentials file {credentialsFile} for mailing do not exist.");
            }
            else
            {
                credentials = cm.Load();
                credentials.PassWord = DecryptString(credentials.PassWord);
                Console.WriteLine($"Credentials loaded from {credentialsFile}:\n{credentials}\n");
            }
            return credentials;
        }

        public override string ToString()
        {
            return $"Host: {Host}\n" +
                   $"Port: {Port:D}\n" +
                   $"Host: {User}\n" +
                   $"Pass: {PassWord}\n" +
                   $" SSL: {EnableSSL}";
        }

        public object Clone()
        {
            return new SmtpServerCredentials()
            {
                Host = Host,
                Port = Port,
                User = User,
                PassWord = PassWord,
                EnableSSL = EnableSSL
            };
        }
    }
}
