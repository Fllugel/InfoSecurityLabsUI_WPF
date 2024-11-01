using InfoLabWPF.MVVM.Model;

namespace InfoLabWPF.Tests;

[TestClass]
public class LinearCongruentialGeneratorTests
{
    [TestMethod]
    public void Next_GeneratesCorrectNextValue()
    {
        var generator = new LinearCongruentialGenerator(9, 2, 3, 1);
        var nextValue = generator.Next();
        Assert.AreEqual(5, nextValue);
    }

    [TestMethod]
    public void GenerateSequence_GeneratesCorrectSequence()
    {
        var generator = new LinearCongruentialGenerator(9, 2, 3, 1);
        var sequence = generator.GenerateSequence(3).ToList();
        CollectionAssert.AreEqual(new List<uint> { 5, 4, 2 }, sequence);
    }

    [TestMethod]
    public void FindPeriod_ReturnsCorrectPeriod()
    {
        var generator = new LinearCongruentialGenerator(9, 2, 3, 1);
        var period = generator.FindPeriod();
        Assert.AreEqual(6, period);
    }

    [TestMethod]
    public void FindPeriod_ReturnsNegativeOneForNoPeriod()
    {
        var generator = new LinearCongruentialGenerator(9, 1, 0, 1);
        var period = generator.FindPeriod();
        Assert.AreEqual(-1, period);
    }

    [TestMethod]
    public void SaveSequence_SavesSequenceToFile()
    {
        var generator = new LinearCongruentialGenerator(9, 2, 3, 1);
        var sequence = generator.GenerateSequence(3);

        // Mock SaveFileDialog and StreamWriter to test SaveSequence method
        // This part is left as a placeholder for actual implementation
    }
}