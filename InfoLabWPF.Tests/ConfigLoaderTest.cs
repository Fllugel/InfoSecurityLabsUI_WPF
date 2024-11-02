using System.Configuration;
namespace InfoLabWPF.Tests;
using InfoLabWPF.MVVM.Model;

[TestClass]
public class ConfigLoaderTest
{
    [TestMethod]
    public void LoadConfigLab1_LoadsCorrectValues()
    {
        var configLoader = new ConfigLoader();
        ConfigurationManager.AppSettings["Lab1.Modulus"] = "10";
        ConfigurationManager.AppSettings["Lab1.Multiplier"] = "20";
        ConfigurationManager.AppSettings["Lab1.Increment"] = "30";
        ConfigurationManager.AppSettings["Lab1.Seed"] = "40";
        ConfigurationManager.AppSettings["Lab1.SequenceCount"] = "50";

        configLoader.LoadConfigLab1();

        Assert.AreEqual(10u, configLoader.Lab1Modulus);
        Assert.AreEqual(20u, configLoader.Lab1Multiplier);
        Assert.AreEqual(30u, configLoader.Lab1Increment);
        Assert.AreEqual(40u, configLoader.Lab1Seed);
        Assert.AreEqual(50u, configLoader.Lab1SequenceCount);
    }

    [TestMethod]
    public void LoadConfigLab2_LoadsCorrectValues()
    {
        var configLoader = new ConfigLoader();
        ConfigurationManager.AppSettings["Lab2.TestInput.0"] = "input1";
        ConfigurationManager.AppSettings["Lab2.ExpectedHash.0"] = "hash1";
        ConfigurationManager.AppSettings["Lab2.TestInput.1"] = "input2";
        ConfigurationManager.AppSettings["Lab2.ExpectedHash.1"] = "hash2";

        var result = configLoader.LoadConfigLab2();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(("input1", "hash1"), result[0]);
        Assert.AreEqual(("input2", "hash2"), result[1]);
    }

    [TestMethod]
    public void LoadConfigLab3_LoadsCorrectValues()
    {
        var configLoader = new ConfigLoader();
        ConfigurationManager.AppSettings["Lab3.MD5.Modulus"] = "100";
        ConfigurationManager.AppSettings["Lab3.MD5.Multiplier"] = "200";
        ConfigurationManager.AppSettings["Lab3.MD5.Increment"] = "300";
        ConfigurationManager.AppSettings["Lab3.MD5.Seed"] = "400";
        ConfigurationManager.AppSettings["Lab3.RC5.WordSize"] = "16";
        ConfigurationManager.AppSettings["Lab3.RC5.Rounds"] = "12";
        ConfigurationManager.AppSettings["Lab3.RC5.PasswordPhraseLength"] = "8";

        configLoader.LoadConfigLab3();

        Assert.AreEqual(100u, configLoader.Lab3MD5Modulus);
        Assert.AreEqual(200u, configLoader.Lab3MD5Multiplier);
        Assert.AreEqual(300u, configLoader.Lab3MD5Increment);
        Assert.AreEqual(400u, configLoader.Lab3MD5Seed);
        Assert.AreEqual(16, configLoader.Lab3RC5WordSize);
        Assert.AreEqual(12, configLoader.Lab3RC5Rounds);
        Assert.AreEqual(8, configLoader.Lab3PasswordPhraseLength);
    }
}