using InfoLabWPF.MVVM.Model;

namespace InfoLabWPF.Tests;

[TestClass]
public class GCDTestTests
{
    [TestMethod]
    public void EstimatePi_ValidSequence_ReturnsExpectedResult()
    {
        // Arrange
        var gcdTest = new GCDTest();
        var sequence = new List<uint> { 15, 28, 33, 14, 21, 22 };

        // Act
        double result = gcdTest.EstimatePi(sequence);

        // Assert
        Assert.IsTrue(result > 2.0 && result < 4.0, "EstimatePi did not return a value within the expected range.");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void EstimatePi_SequenceWithLessThanTwoNumbers_ThrowsException()
    {
        // Arrange
        var gcdTest = new GCDTest();
        var sequence = new List<uint> { 15 };

        // Act
        gcdTest.EstimatePi(sequence);

        // Assert is handled by ExpectedException
    }
}