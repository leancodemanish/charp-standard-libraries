using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LeanCode.Standard.Encryption
{
    public static class AesEncryption
    {
        private static readonly byte[] _salt = Encoding.ASCII.GetBytes("p9806642mhkM7d5");
        private const string SHARED_KEY = "{127051CD-17C6-44F9-87D2-A3CA3F7140E1}";

        public static string Encrypt(this string plainText) => Encrypt(plainText, SHARED_KEY);

        /// <summary>
        /// Encrypt the given string using AES.  The string can be decrypted using 
        /// DecryptStringAES().  The sharedSecret parameters must match.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
        public static string Encrypt(this string plainText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentNullException("plainText");
            }

            if (string.IsNullOrEmpty(sharedSecret))
            {
                throw new ArgumentNullException("sharedSecret");
            }

            string outStr = null; // Encrypted string to return
            RijndaelManaged aesAlg = null; // RijndaelManaged object used to encrypt the data.

            try
            {
                var key = new Rfc2898DeriveBytes(sharedSecret, _salt);

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
                aesAlg?.Dispose();
            }

            return outStr;
        }

        public static string Decrypt(this string cipherText) => Decrypt(cipherText, SHARED_KEY);

        /// <summary>
        /// Decrypt the given string.  Assumes the string was encrypted using 
        /// AESEncryptDecrypt.Encrypt(), using an identical sharedSecret.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
        public static string Decrypt(this string cipherText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentNullException("cipherText");
            }

            if (string.IsNullOrEmpty(sharedSecret))
            {
                throw new ArgumentNullException("sharedSecret");
            }

            RijndaelManaged aesAlg = null;
            string plaintext = null;

            try
            {
                var key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                var bytes = Convert.FromBase64String(cipherText);

                using (var msDecrypt = new MemoryStream(bytes))
                {
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = ReadByteArray(msDecrypt);

                    var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                aesAlg?.Clear();
                aesAlg?.Dispose();
            }

            return plaintext;
        }

        private static byte[] ReadByteArray(this Stream s)
        {
            var rawLength = new byte[sizeof(int)];

            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            var buffer = new byte[BitConverter.ToInt32(rawLength, 0)];

            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }
    }
}
