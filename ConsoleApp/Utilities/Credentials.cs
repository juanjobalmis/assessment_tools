using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace AssessmentTools.Utilities
{
    public class Credentials<T> where T : class
    {
        public string File { get; private set; }
        public Credentials(string file)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
        }

        private byte[] Get128bitsKey(string accessKey)
        {
            const int KeySize = 128;
            byte[] key = new SHA1CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(accessKey));
            byte[] key128 = new byte[KeySize / 8];
            Array.Copy(key, 0, key128, 0, key128.Length);
            return key128;
        }

        public void Save(T data, string accessKey)
        {
            try
            {
                using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
                {
                    aes.KeySize = 128;
                    byte[] key = Get128bitsKey(accessKey);
                    using (FileStream fs = new FileStream(File, FileMode.Create, FileAccess.Write))
                    using (CryptoStream cs = new CryptoStream(fs, aes.CreateEncryptor(key, key), CryptoStreamMode.Write))
                    {
                        new BinaryFormatter().Serialize(cs, data);
                    }
                }
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
                using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
                {
                    aes.KeySize = 128;
                    byte[] key = Get128bitsKey(accessKey);
                    using (FileStream fs = new FileStream(File, FileMode.Open, FileAccess.Read))
                    using (CryptoStream cs = new CryptoStream(fs, aes.CreateDecryptor(key, key), CryptoStreamMode.Read))
                    {
                        return new BinaryFormatter().Deserialize(cs) as T
                            ??
                            throw new NullReferenceException($"There is no valid {typeof(T).Name} in {File}");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Impossible to load {typeof(T).Name} in {File}", e);
            }
        }
    }
}
