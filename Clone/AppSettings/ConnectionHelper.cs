using System.Security.Cryptography;
using System.Text;

namespace Indotalent.AppSettings
{
    public static class ConnectionHelper
    {
        private const string EncryptionKey = "HappyNewYearR@diant";

        public static string Encrypt(string plainText)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32).Substring(0, 32));
            byte[] iv = new byte[16];

            using Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = iv;

            using MemoryStream ms = new();
            using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            cs.Write(plainBytes, 0, plainBytes.Length);
            cs.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string cipherText)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32).Substring(0, 32));
            byte[] iv = new byte[16];

            using Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = iv;

            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using MemoryStream ms = new();
            using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);

            cs.Write(cipherBytes, 0, cipherBytes.Length);
            cs.FlushFinalBlock();

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}