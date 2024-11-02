using System;
using System.IO;
using System.Security.Cryptography;

namespace InfoLabWPF.MVVM.Model
{
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
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.FromXmlString(PublicKey);

                byte[] aesKey = new byte[32]; // 256-bit key
                RandomNumberGenerator.Fill(aesKey);

                using (var aes = new AesGcm(aesKey))
                {
                    // Encrypt the AES key with RSA
                    byte[] encryptedAesKey = rsa.Encrypt(aesKey, RSAEncryptionPadding.OaepSHA1);

                    // Encrypt file data with AES
                    byte[] fileData = File.ReadAllBytes(inputFilePath);
                    byte[] encryptedData = new byte[fileData.Length];
                    byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
                    byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];
                    RandomNumberGenerator.Fill(nonce);

                    aes.Encrypt(nonce, fileData, encryptedData, tag);

                    // Write encrypted AES key, nonce, tag, and encrypted data to output file
                    using (var fs = new FileStream(outputFilePath, FileMode.Create))
                    using (var bw = new BinaryWriter(fs))
                    {
                        bw.Write(encryptedAesKey.Length);
                        bw.Write(encryptedAesKey);
                        bw.Write(nonce.Length);
                        bw.Write(nonce);
                        bw.Write(tag.Length);
                        bw.Write(tag);
                        bw.Write(encryptedData.Length);
                        bw.Write(encryptedData);
                    }
                }
            }
        }

        public void DecryptFile(string inputFilePath, string outputFilePath)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.FromXmlString(PrivateKey);

                byte[] encryptedData;
                using (var fs = new FileStream(inputFilePath, FileMode.Open))
                using (var br = new BinaryReader(fs))
                {
                    // Read encrypted AES key, nonce, tag, and encrypted data
                    int encryptedAesKeyLength = br.ReadInt32();
                    byte[] encryptedAesKey = br.ReadBytes(encryptedAesKeyLength);
                    int nonceLength = br.ReadInt32();
                    byte[] nonce = br.ReadBytes(nonceLength);
                    int tagLength = br.ReadInt32();
                    byte[] tag = br.ReadBytes(tagLength);
                    int encryptedDataLength = br.ReadInt32();
                    encryptedData = br.ReadBytes(encryptedDataLength);

                    // Decrypt AES key
                    byte[] aesKey = rsa.Decrypt(encryptedAesKey, RSAEncryptionPadding.OaepSHA1);

                    using (var aes = new AesGcm(aesKey))
                    {
                        // Decrypt the file data with AES
                        byte[] decryptedData = new byte[encryptedData.Length];
                        aes.Decrypt(nonce, encryptedData, tag, decryptedData);

                        // Write decrypted data to output file
                        File.WriteAllBytes(outputFilePath, decryptedData);
                    }
                }
            }
        }
    }
}