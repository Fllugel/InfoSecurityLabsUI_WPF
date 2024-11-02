using System.Text;
using InfoLabWPF.MVVM.Model;

namespace InfoLabWPF.Tests;

[TestClass]
public class MD5Tests
{
    [TestMethod]
    public void ComputeHash_ReturnsCorrectHashForKnownInput()
    {
        var md5 = new MD5();
        var input = Encoding.UTF8.GetBytes("hello");
        var hash = md5.ComputeHash(input);
        var expectedHash = new byte[]
            { 0x5d, 0x41, 0x40, 0x2a, 0xbc, 0x4b, 0x2a, 0x76, 0xb9, 0x71, 0x9d, 0x91, 0x10, 0x17, 0xc5, 0x92 };
        CollectionAssert.AreEqual(expectedHash, hash);
    }

    [TestMethod]
    public async Task ComputeHashFromFile_ReturnsCorrectHashForKnownFile()
    {
        var md5 = new MD5();
        var filePath = "testfile.txt";
        await File.WriteAllTextAsync(filePath, "hello");
        var hash = await md5.ComputeHashFromFile(filePath);
        var expectedHash = new byte[]
            { 0x5d, 0x41, 0x40, 0x2a, 0xbc, 0x4b, 0x2a, 0x76, 0xb9, 0x71, 0x9d, 0x91, 0x10, 0x17, 0xc5, 0x92 };
        CollectionAssert.AreEqual(expectedHash, hash);
        File.Delete(filePath);
    }

    [TestMethod]
    [ExpectedException(typeof(FileNotFoundException))]
    public async Task ComputeHashFromFile_ThrowsFileNotFoundExceptionForNonExistentFile()
    {
        var md5 = new MD5();
        await md5.ComputeHashFromFile("nonexistentfile.txt");
    }

    [TestMethod]
    public void LoadInputFromFile_LoadsCorrectContent()
    {
        var filePath = "testfile.txt";
        File.WriteAllText(filePath, "hello");
        MD5.LoadInputFromFile(filePath, out var inputText);
        Assert.AreEqual("hello", inputText);
        File.Delete(filePath);
    }

    [TestMethod]
    [ExpectedException(typeof(FileNotFoundException))]
    public void LoadInputFromFile_ThrowsFileNotFoundExceptionForNonExistentFile()
    {
        MD5.LoadInputFromFile("nonexistentfile.txt", out _);
    }

    [TestMethod]
    public void SaveHashToFile_SavesCorrectHash()
    {
        var filePath = "hashfile.txt";
        var hash = "5d41402abc4b2a76b9719d911017c592";
        var md5 = new MD5();
        md5.SaveHashToFile(filePath, hash);
        var savedHash = File.ReadAllText(filePath);
        Assert.AreEqual(hash, savedHash);
        File.Delete(filePath);
    }
}