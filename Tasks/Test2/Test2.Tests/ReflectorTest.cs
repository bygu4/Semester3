// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Test2.Tests;

using NUnit.Framework;

public static class ReflectorTest
{
    private const string ExpectedOutputPath1 = "ExpectedOutput/Output1.txt";
    private const string ResultPath = "TestClass1.cs";

    [Test]
    public static async Task TestReflector_PrintStructure_CompareResults()
    {
        var expectedOutput = await File.ReadAllTextAsync(ExpectedOutputPath1);
        await Reflector.PrintStructure(typeof(TestClass1));
        var actualOutput = await File.ReadAllTextAsync(ResultPath);
        Assert.That(actualOutput, Is.EqualTo(expectedOutput));
    }

    [Test]
    public static async Task TestReflector_PrintSomeTypes_NotThrowException()
    {
        await Reflector.PrintStructure(typeof(Int32));
        await Reflector.PrintStructure(typeof(String));
    }
}
