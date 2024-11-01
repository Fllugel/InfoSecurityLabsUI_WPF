namespace InfoLabWPF.Tests;
using InfoLabWPF.MVVM.Model;

[TestClass]
public class RC5Test
{
    [TestMethod]
    public void EncryptBlock_EncryptsCorrectly()
    {
        var rc5 = new RC5(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF }, 9, 2, 3, 1);
        uint A = 0x01234567;
        uint B = 0x89ABCDEF;
        rc5.EncryptBlock(ref A, ref B);
        Assert.AreEqual(0xEEDBA521, A);
        Assert.AreEqual(0x6D8F4B15, B);
    }

    [TestMethod]
    public void DecryptBlock_DecryptsCorrectly()
    {
        var rc5 = new RC5(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF }, 9, 2, 3, 1);
        uint A = 0xEEDBA521;
        uint B = 0x6D8F4B15;
        rc5.DecryptBlock(ref A, ref B);
        Assert.AreEqual(0x01234567, A);
        Assert.AreEqual(0x89ABCDEF, B);
    }

    [TestMethod]
    public void Encrypt_EncryptsDataCorrectly()
    {
        var rc5 = new RC5(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF }, 9, 2, 3, 1);
        var data = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF };
        var encrypted = rc5.Encrypt(data);
        Assert.AreEqual(16, encrypted.Length);
    }

    [TestMethod]
    public void Decrypt_DecryptsDataCorrectly()
    {
        var rc5 = new RC5(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF }, 9, 2, 3, 1);
        var encrypted = new byte[]
        {
            0xEE, 0xDB, 0xA5, 0x21,
            0x6D, 0x8F, 0x4B, 0x15,
            0x01, 0x23, 0x45, 0x67,
            0x89, 0xAB, 0xCD, 0xEF
        };
        var decrypted = rc5.Decrypt(encrypted);
        Assert.AreEqual(8, decrypted.Length);
        CollectionAssert.AreEqual(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF }, decrypted);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Decrypt_ThrowsExceptionForInvalidDataLength()
    {
        var rc5 = new RC5(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF }, 9, 2, 3, 1);
        var invalidData = new byte[] { 0x01, 0x23, 0x45 };
        rc5.Decrypt(invalidData);
    }
}