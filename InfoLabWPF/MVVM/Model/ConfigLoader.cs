using System.Configuration;

public class ConfigLoader
{
    public int Modulus { get; private set; }
    public int Multiplier { get; private set; }
    public int Increment { get; private set; }
    public int Seed { get; private set; }
    public int SequenceCount { get; private set; }

    public ConfigLoader()
    {
        LoadConfig();
    }

    private void LoadConfig()
    {
        Modulus = int.Parse(ConfigurationManager.AppSettings["Modulus"]);
        Multiplier = int.Parse(ConfigurationManager.AppSettings["Multiplier"]);
        Increment = int.Parse(ConfigurationManager.AppSettings["Increment"]);
        Seed = int.Parse(ConfigurationManager.AppSettings["Seed"]);
        SequenceCount = int.Parse(ConfigurationManager.AppSettings["SequenceCount"]);
    }
}