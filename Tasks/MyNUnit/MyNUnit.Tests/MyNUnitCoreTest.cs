// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit.Core.Tests;

using NUnit.Framework;

public static class MyNUnitCoreTest
{
    private const string TestProjectsPath = "TestProjects/";

    private static List<(int, int, int)> testResults =
    [
        (0, 0, 0),
        (0, 0, 0),
        (0, 0, 0),
        (1, 2, 0),
        (3, 2, 2),
        (5, 5, 2),
        (2, 1, 1),
    ];

    private static string[] TestDirectoryPaths
    {
        get
        {
            var paths = Directory.GetDirectories(TestProjectsPath);
            Array.Sort(paths);
            return paths;
        }
    }

    private static IEnumerable<TestCaseData> CorrectnessTestCases()
    {
        for (int i = 0; i < testResults.Count; ++i)
        {
            var testDirectoryPath = TestDirectoryPaths[i];
            var (passed, failed, ignored) = testResults[i];;
            yield return new TestCaseData(testDirectoryPath, passed, failed, ignored);
        }
    }

    [TestCaseSource(nameof(CorrectnessTestCases))]
    public static async Task TestCorrectness_RunTestsForEachAssembly_TestResultsAreCorrect(
        string path,
        int testsPassed,
        int testsFailed,
        int testsIgnored)
    {
        var testResult = await MyNUnitCore.RunTestsFromEachAssembly(path);
        Assert.That(testResult.NumberOfTestsPassed, Is.EqualTo(testsPassed));
        Assert.That(testResult.NumberOfTestsFailed, Is.EqualTo(testsFailed));
        Assert.That(testResult.NumberOfTestsIgnored, Is.EqualTo(testsIgnored));
    }

    [TestCaseSource(nameof(TestDirectoryPaths))]
    public static async Task TestConsistency_RunTestsTwice_TestResultsAreEqual(string path)
    {
        var testResult1 = await MyNUnitCore.RunTestsFromEachAssembly(path);
        var testResult2 = await MyNUnitCore.RunTestsFromEachAssembly(path);
        Assert.That(testResult1, Is.EqualTo(testResult2));
    }

    [TestAttribute]
    public static void TestStability_RunTestsForNonExistentDirectory_ThrowException()
        => Assert.ThrowsAsync<DirectoryNotFoundException>(
            () => MyNUnitCore.RunTestsFromEachAssembly("mboieuibnui"));
}
