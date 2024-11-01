namespace InfoLabWPF.Tests;
using InfoLabWPF.MVVM.Model;

[TestClass]
public class RSATest
{
    [TestMethod]
    public void GenerateKeys_CreatesNonEmptyKeys()
    {
        var rsa = new RSA();
        rsa.GenerateKeys();
        Assert.IsFalse(string.IsNullOrEmpty(rsa.PublicKey));
        Assert.IsFalse(string.IsNullOrEmpty(rsa.PrivateKey));
    }

    [TestMethod]
    public void EncryptFile_EncryptsAndCreatesOutputFile()
    {
        var rsa = new RSA();
        rsa.GenerateKeys();
        var inputFilePath = "input.txt";
        var outputFilePath = "output.enc";
        File.WriteAllText(inputFilePath, "Test data");
        rsa.EncryptFile(inputFilePath, outputFilePath);
        Assert.IsTrue(File.Exists(outputFilePath));
        File.Delete(inputFilePath);
        File.Delete(outputFilePath);
    }

    [TestMethod]
    public void DecryptFile_DecryptsCorrectly()
    {
        var rsa = new RSA();
        rsa.GenerateKeys();
        var inputFilePath = "input.txt";
        var encryptedFilePath = "encrypted.enc";
        var decryptedFilePath = "decrypted.txt";
        File.WriteAllText(inputFilePath, "Test data");
        rsa.EncryptFile(inputFilePath, encryptedFilePath);
        rsa.DecryptFile(encryptedFilePath, decryptedFilePath);
        var decryptedData = File.ReadAllText(decryptedFilePath);
        Assert.AreEqual("Test data", decryptedData);
        File.Delete(inputFilePath);
        File.Delete(encryptedFilePath);
        File.Delete(decryptedFilePath);
    }

    [TestMethod]
    [ExpectedException(typeof(FileNotFoundException))]
    public void EncryptFile_ThrowsExceptionForNonExistentInputFile()
    {
        var rsa = new RSA();
        rsa.GenerateKeys();
        rsa.EncryptFile("nonexistent.txt", "output.enc");
    }

    [TestMethod]
    [ExpectedException(typeof(FileNotFoundException))]
    public void DecryptFile_ThrowsExceptionForNonExistentInputFile()
    {
        var rsa = new RSA();
        rsa.GenerateKeys();
        rsa.DecryptFile("nonexistent.enc", "output.txt");
    }
}