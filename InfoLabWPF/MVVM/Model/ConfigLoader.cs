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

    public ConfigLoader()
    {
        LoadConfigLab1();
    }

    private void LoadConfigLab1()
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
}
