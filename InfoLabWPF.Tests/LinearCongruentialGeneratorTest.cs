using InfoLabWPF.MVVM.Model;
namespace InfoLabWPF.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[TestClass]
public class LinearCongruentialGeneratorTests
{
    [TestMethod]
    public void GenerateSequence_ReturnsCorrectSequence()
    {
        var generator = new LinearCongruentialGenerator(9, 2, 0, 1);
        var expectedSequence = new List<uint> { 2, 4, 8, 7, 5, 1, 2, 4, 8 };
        var actualSequence = generator.GenerateSequence(9).ToList();
        CollectionAssert.AreEqual(expectedSequence, actualSequence);
    }

    [TestMethod]
    public void FindPeriod_ReturnsCorrectPeriod()
    {
        var generator = new LinearCongruentialGenerator(9, 2, 0, 1);
        var period = generator.FindPeriod();
        Assert.AreEqual(6, period);
    }
}