using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

public class ConfigLoader
{
    public uint Lab1Modulus { get; private set; }
    public uint Lab1Multiplier { get; private set; }
    public uint Lab1Increment { get; private set; }
    public uint Lab1Seed { get; private set; }
    public uint Lab1SequenceCount { get; private set; }

    public uint Lab3MD5Modulus { get; private set; }
    public uint Lab3MD5Multiplier { get; private set; }
    public uint Lab3MD5Increment { get; private set; }
    public uint Lab3MD5Seed { get; private set; }
    public int Lab3RC5WordSize { get; private set; }
    public int Lab3RC5Rounds { get; private set; }
    public int Lab3PasswordPhraseLength { get; private set; }

    public void LoadConfigLab1()
    {
        Lab1Modulus = uint.Parse(ConfigurationManager.AppSettings["Lab1.Modulus"]);
        Lab1Multiplier = uint.Parse(ConfigurationManager.AppSettings["Lab1.Multiplier"]);
        Lab1Increment = uint.Parse(ConfigurationManager.AppSettings["Lab1.Increment"]);
        Lab1Seed = uint.Parse(ConfigurationManager.AppSettings["Lab1.Seed"]);
        Lab1SequenceCount = uint.Parse(ConfigurationManager.AppSettings["Lab1.SequenceCount"]);
    }
    
    public List<(string Input, string Expected)> LoadConfigLab2()
    {
        var testValues = new List<(string Input, string Expected)>();
        int index = 0;

        while (true)
        {
            string inputKey = $"Lab2.TestInput.{index}";
            string expectedKey = $"Lab2.ExpectedHash.{index}";

            string inputValue = ConfigurationManager.AppSettings[inputKey];
            string expectedValue = ConfigurationManager.AppSettings[expectedKey];

            if (inputValue == null || expectedValue == null) break;

            testValues.Add((inputValue, expectedValue));
            index++;
        }

        return testValues;
    }

    public void LoadConfigLab3()
    {
        Lab3MD5Modulus = uint.Parse(ConfigurationManager.AppSettings["Lab3.MD5.Modulus"]);
        Lab3MD5Multiplier = uint.Parse(ConfigurationManager.AppSettings["Lab3.MD5.Multiplier"]);
        Lab3MD5Increment = uint.Parse(ConfigurationManager.AppSettings["Lab3.MD5.Increment"]);
        Lab3MD5Seed = uint.Parse(ConfigurationManager.AppSettings["Lab3.MD5.Seed"]);
        Lab3RC5WordSize = int.Parse(ConfigurationManager.AppSettings["Lab3.RC5.WordSize"]);
        Lab3RC5Rounds = int.Parse(ConfigurationManager.AppSettings["Lab3.RC5.Rounds"]);
        Lab3PasswordPhraseLength = int.Parse(ConfigurationManager.AppSettings["Lab3.RC5.PasswordPhraseLength"]);
    }
}