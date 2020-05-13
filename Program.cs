using System;
using System.IO;
using System.Security.Cryptography;

namespace IssueEncryptionStreamEmpty
{
    class Program
    {
        static void Main(string[] args)
        {
            var encryptor = GetEncryptor();
            var text = "Under 15 characters this text causes problems";
            while (text.Length >= 0)
            {
                text = text.Substring(0, text.Length - 1);
                Console.WriteLine($"Result Method A with {text.Length} characters: {GetWorkingEncrypted(text, encryptor)}");
                Console.WriteLine($"Result Method B with {text.Length} characters: {GetNonWorkingEncrypted(text, encryptor)}");
            }
        }

        private static string GetWorkingEncrypted(string text, ICryptoTransform encryptor)
        {
            using var memoryStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                using var writer = new StreamWriter(cryptoStream);
                writer.Write(text);
            }

            var encryptedData = memoryStream.ToArray();
            if (encryptedData.Length == 0)
            {
                throw new Exception($"Encrypted data is 0 for text {text}");
            }
            var encryptedText = Convert.ToBase64String(encryptedData);
            return encryptedText;
        }
        
        private static string GetNonWorkingEncrypted(string text, ICryptoTransform encryptor)
        {
            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            using var writer = new StreamWriter(cryptoStream);
            writer.Write(text);
            writer.Flush();

            var encryptedData = memoryStream.ToArray();
            if (encryptedData.Length == 0)
            {
                throw new Exception($"Encrypted data is 0 for text \"{text}\" with length {text.Length}");
            }
            var encryptedText = Convert.ToBase64String(encryptedData);
            return encryptedText;
        }

        private static ICryptoTransform GetEncryptor()
        {
            var aesManaged =
                new AesManaged
                {
                    Padding = PaddingMode.PKCS7
                };
            
            return aesManaged.CreateEncryptor();
        }
    }
}