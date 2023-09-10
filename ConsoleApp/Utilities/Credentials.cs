using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AssessmentTools.Utilities
{
    public class Credentials<T> where T : class
    {
        public string File { get; private set; }
        public Credentials(string file)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
        }

        static string EncryptString(string plaintext, string password)
        {
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            Rfc2898DeriveBytes passwordBytes = new(password, 20);

            var encryptor = Aes.Create();
            encryptor.Key = passwordBytes.GetBytes(32);
            encryptor.IV = passwordBytes.GetBytes(16);
            using MemoryStream ms = new();
            using CryptoStream cs = new(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(plaintextBytes, 0, plaintextBytes.Length);
            return Convert.ToBase64String(ms.ToArray());
        }
 
        static string DecryptString(string encrypted, string password)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encrypted);
            Rfc2898DeriveBytes passwordBytes = new(password, 20);
            var encryptor = Aes.Create();
            encryptor.Key = passwordBytes.GetBytes(32);
            encryptor.IV = passwordBytes.GetBytes(16);
            using MemoryStream ms = new();
            using CryptoStream cs = new(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(encryptedBytes, 0, encryptedBytes.Length);
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public void Save(T data, string accessKey)
        {
            try
            {
                using StreamWriter sw = new(File, false, Encoding.UTF8);
                sw.Write(EncryptString(JsonSerializer.Serialize(data), accessKey));
            }
            catch (Exception e)
            {
                throw new Exception($"Impossible to create credential file {File}", e);
            }
        }
        public T Load(string accessKey)
        {
            try
            {
                using StreamReader sr = new(File, Encoding.UTF8);
                return JsonSerializer.Deserialize<T>(DecryptString(sr.ReadToEnd(), accessKey));
            }
            catch (Exception e)
            {
                throw new Exception($"Impossible to load {typeof(T).Name} in {File}", e);
            }
        }
    }
}
