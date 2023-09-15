using System;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using System.IO;
using RestService.Models;
using RestService.Data;

namespace RestService.Helpers
{
    public static class TokenHelper
    {
        public static string GetToken(string userEmail, string userPassword)
        {
            // Check user's email and password against your DataStore.
            var user = DataStore.Users.FirstOrDefault(u => u.UserEmail == userEmail && u.UserPassword == userPassword);
            if (user == null)
                return null; // or throw an exception based on your requirements

            var token = new Token
            {
                UserId = user.UserId,
                Expires = DateTime.UtcNow.AddMinutes(10),
            };

            var jsonString = JsonSerializer.Serialize(token);
            return Crypto.EncryptStringAES(jsonString);
        }
    }

    internal static class Crypto
    {
        private static readonly byte[] Salt = Encoding.ASCII.GetBytes("B78A07A7-14D8-4890-BC99-9145A14713C1");
        private const string Password = "sharedSecretPassword";

        public static string EncryptStringAES(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");

            string outStr;
            RijndaelManaged aesAlg = null;

            try
            {
                var key = new Rfc2898DeriveBytes(Password, Salt);
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                aesAlg?.Clear();
            }

            return outStr;
        }
    }
}
