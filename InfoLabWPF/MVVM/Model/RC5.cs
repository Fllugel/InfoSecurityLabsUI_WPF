using System;
using System.IO;
using System.Security.Cryptography;

namespace InfoLabWPF.MVVM.Model
{
    public class RC5
    {
        private readonly byte[] _key;

        public RC5(string passwordPhrase)
        {
            using (var sha256 = SHA256.Create())
            {
                _key = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passwordPhrase));
            }
        }

        public void EncryptStream(Stream inputStream, Stream outputStream, Action<double> progressCallback)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.GenerateIV(); 
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                outputStream.Write(aes.IV, 0, aes.IV.Length);

                using (var cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    long totalBytesRead = 0;
                    long inputLength = inputStream.Length;

                    while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        cryptoStream.Write(buffer, 0, bytesRead);
                        totalBytesRead += bytesRead;
                        progressCallback((double)totalBytesRead / inputLength * 100); 
                    }
                }
            }
        }

        public void DecryptStream(Stream inputStream, Stream outputStream)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] iv = new byte[16];
                inputStream.Read(iv, 0, iv.Length);
                aes.IV = iv;

                using (var cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;

                    while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outputStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }
    }
}
