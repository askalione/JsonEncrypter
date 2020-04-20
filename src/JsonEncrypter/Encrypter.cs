using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace JsonEncrypter
{
    public class Encrypter
    {
        private readonly byte[] _encryptionKeyBytes;

        public Encrypter(string encryptionKey)
        {
            if (encryptionKey == null)
            {
                throw new ArgumentNullException(nameof(encryptionKey));
            }

            using (var sha = new SHA256Managed())
            {
                _encryptionKeyBytes =
                    sha.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
            }
        }

        public string Encrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;


            var buffer = Encoding.UTF8.GetBytes(value);

            using (var inputStream = new MemoryStream(buffer, false))
            using (var outputStream = new MemoryStream())
            using (var aes = new AesManaged
            {
                Key = _encryptionKeyBytes
            })
            {
                var iv = aes.IV;
                outputStream.Write(iv, 0, iv.Length);
                outputStream.Flush();

                var encryptor = aes.CreateEncryptor(_encryptionKeyBytes, iv);
                using (var cryptoStream = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write))
                {
                    inputStream.CopyTo(cryptoStream);
                }

                return Convert.ToBase64String(outputStream.ToArray());
            }
        }
    }
}
