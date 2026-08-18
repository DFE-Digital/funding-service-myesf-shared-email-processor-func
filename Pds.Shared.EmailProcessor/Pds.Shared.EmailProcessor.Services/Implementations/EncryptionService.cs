using Pds.Shared.EmailProcessor.Services.Interfaces;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Pds.Shared.EmailProcessor.Services.Implementations
{
    /// <summary>
    /// The encryption service class.
    /// </summary>
    public class EncryptionService : IEncryptionService
    {
        /// <inheritdoc />
        public string DecryptStringFromHex(string symmetricKey, string cipherText)
        {
            byte[] buffer = Convert.FromHexString(cipherText);

            return DecryptString(symmetricKey, buffer);
        }

        /// <inheritdoc />
        public string EncryptStringToHex(string symmetricKey, string plainText)
        {
            return Convert.ToHexString(EncryptString(symmetricKey, plainText));
        }

        private string DecryptString(string symmetricKey, byte[] buffer)
        {
            byte[] iv = new byte[16];

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(symmetricKey);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        private byte[] EncryptString(string symmetricKey, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(symmetricKey);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return array;
        }
    }
}
