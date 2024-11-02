using InfoLabWPF.MVVM.Model;
namespace InfoLabWPF.Tests;

[TestClass]
public class RC5Tests
{
    private RC5 CreateRC5Instance()
    {
        byte[] key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
        return new RC5(key, 429490006, 1664525, 10223, 1);
    }

    [TestMethod]
    public void Encrypt_Decrypt_ReturnsOriginalData()
    {
        RC5 rc5 = CreateRC5Instance();
        byte[] data = { 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80 };

        byte[] encrypted = rc5.Encrypt(data);
        byte[] decrypted = rc5.Decrypt(encrypted);

        CollectionAssert.AreEqual(data, decrypted);
    }

    [TestMethod]
    public void Encrypt_EmptyData_ReturnsEmptyArray()
    {
        RC5 rc5 = CreateRC5Instance();
        byte[] data = new byte[0];

        byte[] encrypted = rc5.Encrypt(data);

        Assert.AreEqual(8, encrypted.Length); // Only IV should be present
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Decrypt_InvalidDataLength_ThrowsArgumentException()
    {
        RC5 rc5 = CreateRC5Instance();
        byte[] data = { 0x10, 0x20, 0x30 };

        rc5.Decrypt(data);
    }

    [TestMethod]
    public void EncryptBlock_DecryptBlock_ReturnsOriginalValues()
    {
        RC5 rc5 = CreateRC5Instance();
        uint A = 0x12345678;
        uint B = 0x9ABCDEF0;

        rc5.EncryptBlock(ref A, ref B);
        rc5.DecryptBlock(ref A, ref B);

        Assert.AreEqual((int)0x12345678, (int)A);
        Assert.AreEqual(unchecked((int)0x9ABCDEF0), (int)B);
    }
}