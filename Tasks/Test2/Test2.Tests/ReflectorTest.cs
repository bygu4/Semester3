namespace Test2.Tests;

using NUnit.Framework;

public static class ReflectorTest
{
    private const string ExpectedOutputPath1 = "ExpectedOutput/Output1.txt";
    private const string ResultPath = "TestClass.cs";

    [Test]
    public static async Task TestReflector_PrintStructure_CompareResults()
    {
        var expectedOutput = await File.ReadAllTextAsync(ExpectedOutputPath1);
        await Reflector.PrintStructure(typeof(TestClass1));
        var actualOutput = await File.ReadAllTextAsync(ResultPath);
        Assert.That(actualOutput, Is.EqualTo(expectedOutput));
    }

    public static async Task TestReflector_PrintSomeTypes()
    {
        await Reflector.PrintStructure(typeof(Int32));
        await Reflector.PrintStructure(typeof(String));
    }
}
