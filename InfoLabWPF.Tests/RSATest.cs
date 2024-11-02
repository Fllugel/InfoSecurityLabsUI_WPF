using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Security.Cryptography;
using RSA = InfoLabWPF.MVVM.Model.RSA;

namespace InfoLabWPF.Tests
{
    [TestClass]
    public class RSATests
    {
        private RSA rsa;

        [TestInitialize]
        public void Setup()
        {
            rsa = new RSA();
            rsa.GenerateKeys();
        }

        [TestMethod]
        public void GenerateKeys_CreatesValidKeys()
        {
            Assert.IsFalse(string.IsNullOrEmpty(rsa.PublicKey));
            Assert.IsFalse(string.IsNullOrEmpty(rsa.PrivateKey));
        }

        [TestMethod]
        public void EncryptFile_DecryptFile_RestoresOriginalContent()
        {
            string originalContent = "This is a test.";
            string inputFilePath = "testInput.txt";
            string encryptedFilePath = "testEncrypted.bin";
            string decryptedFilePath = "testDecrypted.txt";

            File.WriteAllText(inputFilePath, originalContent);

            rsa.EncryptFile(inputFilePath, encryptedFilePath);
            rsa.DecryptFile(encryptedFilePath, decryptedFilePath);

            string decryptedContent = File.ReadAllText(decryptedFilePath);

            Assert.AreEqual(originalContent, decryptedContent);

            File.Delete(inputFilePath);
            File.Delete(encryptedFilePath);
            File.Delete(decryptedFilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void DecryptFile_WithInvalidPrivateKey_ThrowsException()
        {
            string originalContent = "This is a test.";
            string inputFilePath = "testInput.txt";
            string encryptedFilePath = "testEncrypted.bin";
            string decryptedFilePath = "testDecrypted.txt";

            File.WriteAllText(inputFilePath, originalContent);

            rsa.EncryptFile(inputFilePath, encryptedFilePath);

            // Ensure the encrypted file exists before attempting to decrypt
            Assert.IsTrue(File.Exists(encryptedFilePath));

            RSA rsaInvalid = new RSA();
            rsaInvalid.GenerateKeys(); // Generate a different key pair
            rsaInvalid.DecryptFile(encryptedFilePath, decryptedFilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void EncryptFile_WithNonExistentInputFile_ThrowsException()
        {
            string inputFilePath = "nonExistentInput.txt";
            string encryptedFilePath = "testEncrypted.bin";

            rsa.EncryptFile(inputFilePath, encryptedFilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void DecryptFile_WithNonExistentInputFile_ThrowsException()
        {
            string inputFilePath = "nonExistentInput.bin";
            string decryptedFilePath = "testDecrypted.txt";

            rsa.DecryptFile(inputFilePath, decryptedFilePath);
        }
    }
}