using System;
using System.IO;
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

        public void Save(T data)
        {
            try
            {
                using StreamWriter sw = new(File, false, Encoding.UTF8);
                sw.Write(JsonSerializer.Serialize(data));
            }
            catch (Exception e)
            {
                throw new Exception($"Impossible to create credential file {File}", e);
            }
        }
        public T Load()
        {
            try
            {
                using StreamReader sr = new(File, Encoding.UTF8);
                return JsonSerializer.Deserialize<T>(sr.ReadToEnd());
            }
            catch (Exception e)
            {
                throw new Exception($"Impossible to load {typeof(T).Name} in {File}", e);
            }
        }
    }
}
