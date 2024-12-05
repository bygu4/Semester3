namespace Test2.Tests;

public static class ReflectorTest
{
    private const string ExpectedOutputPath1 = "ExpectedOutput/Output1.txt"

    [Test]
    public static async Task TestReflector_PrintStructure_CompareResults()
    {
        var expectedOutput = await File.ReadAllTextAsync(ExpectedOutputPath1)
        await Reflector.PrintStructure(typeof(TestClass1));
        
    }
}
