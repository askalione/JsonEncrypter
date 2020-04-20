using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace JsonEncrypter
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourcePath = GetPath(args);
            if (string.IsNullOrEmpty(sourcePath))
            {
                sourcePath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().Location).LocalPath), "source.json");
            }
            if (!File.Exists(sourcePath))
            {
                Console.WriteLine("Source file not found. Use source.json or -path argument.");
                return;
            }
            Console.WriteLine($"Detect source: {sourcePath}");            

            var items = JsonConvert.DeserializeObject<Dictionary<string, EncryptingItem>>(File.ReadAllText(sourcePath));
            Console.WriteLine("Encrypting...");
            items = Encrypt(items);

            var dir = Path.GetDirectoryName(sourcePath);
            var sourceFileName = Path.GetFileNameWithoutExtension(sourcePath);
            var sourceExt = Path.GetExtension(sourcePath);
            var resultFileName = sourceFileName + ".encrypted" + sourceExt;
            var resultPath = Path.Combine(dir, resultFileName);
            var result = JsonConvert.SerializeObject(items, Formatting.Indented);

            Console.WriteLine("Save encrypted result...");
            File.WriteAllText(resultPath, result, Encoding.UTF8);
        }

        private static string GetPath(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-path" && i != args.Length - 1)
                {
                    return args[i + 1]?.Trim();
                }
            }

            return null;
        }

        private static Dictionary<string, EncryptingItem> Encrypt(Dictionary<string, EncryptingItem> items)
        {
            var encrypters = new Dictionary<string, Encrypter>();
            

            foreach(var name in items.Keys)
            {
                var item = items[name];
                if (!string.IsNullOrEmpty(item.Key) && !string.IsNullOrEmpty(item.Value))
                {
                    if (!encrypters.ContainsKey(item.Key))
                        encrypters.Add(item.Key, new Encrypter(item.Key));
                    var encrypter = encrypters[item.Key];
                    item.Value = encrypter.Encrypt(item.Value);
                }
            }

            return items;
        }
    }
}
