using System;
using System.IO;
using System.Security.Cryptography;

public class RSA
{
    public string PublicKey { get; set; }
    public string PrivateKey { get; set; }

    public void GenerateKeys()
    {
        using (var rsa = new RSACryptoServiceProvider(2048))
        {
            PublicKey = rsa.ToXmlString(false); // Export public key
            PrivateKey = rsa.ToXmlString(true); // Export private key
        }
    }

    public void EncryptFile(string inputFilePath, string outputFilePath)
    {
        using (var rsa = new RSACryptoServiceProvider())
        using (var aes = Aes.Create())
        {
            rsa.FromXmlString(PublicKey);

            // Encrypt the AES key and IV with RSA
            byte[] encryptedAesKey = rsa.Encrypt(aes.Key, false);
            byte[] encryptedAesIV = rsa.Encrypt(aes.IV, false);

            // Encrypt file data with AES
            byte[] fileData = File.ReadAllBytes(inputFilePath);
            byte[] encryptedData;
            using (var ms = new MemoryStream())
            using (var cryptoStream = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(fileData, 0, fileData.Length);
                cryptoStream.FlushFinalBlock();
                encryptedData = ms.ToArray();
            }

            // Write encrypted AES key, IV, and encrypted data to output file
            using (var fs = new FileStream(outputFilePath, FileMode.Create))
            using (var bw = new BinaryWriter(fs))
            {
                bw.Write(encryptedAesKey.Length);
                bw.Write(encryptedAesKey);
                bw.Write(encryptedAesIV.Length);
                bw.Write(encryptedAesIV);
                bw.Write(encryptedData.Length);
                bw.Write(encryptedData);
            }
        }
    }

    public void DecryptFile(string inputFilePath, string outputFilePath)
    {
        using (var rsa = new RSACryptoServiceProvider())
        using (var aes = Aes.Create())
        {
            rsa.FromXmlString(PrivateKey);

            byte[] encryptedData;
            using (var fs = new FileStream(inputFilePath, FileMode.Open))
            using (var br = new BinaryReader(fs))
            {
                // Read encrypted AES key and IV
                int aesKeyLength = br.ReadInt32();
                byte[] encryptedAesKey = br.ReadBytes(aesKeyLength);
                int aesIVLength = br.ReadInt32();
                byte[] encryptedAesIV = br.ReadBytes(aesIVLength);

                // Decrypt AES key and IV
                aes.Key = rsa.Decrypt(encryptedAesKey, false);
                aes.IV = rsa.Decrypt(encryptedAesIV, false);

                // Read encrypted file data
                int encryptedDataLength = br.ReadInt32();
                encryptedData = br.ReadBytes(encryptedDataLength);
            }

            // Decrypt the file data with AES
            byte[] decryptedData;
            using (var ms = new MemoryStream())
            using (var cryptoStream = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(encryptedData, 0, encryptedData.Length);
                cryptoStream.FlushFinalBlock();
                decryptedData = ms.ToArray();
            }

            // Write decrypted data to output file
            File.WriteAllBytes(outputFilePath, decryptedData);
        }
    }
}