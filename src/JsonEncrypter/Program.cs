using Newtonsoft.Json;
using Structr.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace JsonEncrypter
{
    class Program
    {
        private const string _resultSuffix = ".encrypted";

        static void Main(string[] args)
        {
            try
            {
                Console.Write("Path: ");
                var sourcePath = Console.ReadLine();
                if (string.IsNullOrEmpty(sourcePath))
                    throw new InvalidOperationException("Source path is required.");
                if (File.Exists(sourcePath) == false)
                    throw new InvalidOperationException("Source file not found.");

                Console.Write("Passphrase: ");
                var passphrase = Console.ReadLine();
                if (string.IsNullOrEmpty(passphrase))
                    throw new InvalidOperationException("Passphrase is required.");

                var source = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(sourcePath));

                var encrypted = new Dictionary<string, string>();
                foreach (var key in source.Keys)
                {
                    var value = source[key];
                    encrypted[key] = !string.IsNullOrEmpty(value)
                        ? StringEncoder.Encrypt(value, passphrase)
                        : value;
                }

                var dir = Path.GetDirectoryName(sourcePath);
                var sourceFileName = Path.GetFileNameWithoutExtension(sourcePath);
                var sourceExt = Path.GetExtension(sourcePath);
                var resultFileName = sourceFileName + _resultSuffix + sourceExt;
                var resultPath = Path.Combine(dir, resultFileName);
                var result = JsonConvert.SerializeObject(encrypted, Formatting.Indented);

                File.WriteAllText(resultPath, result, Encoding.UTF8);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failure: {ex.Message}");
                Console.ReadKey();
            }
        }
    }
}
