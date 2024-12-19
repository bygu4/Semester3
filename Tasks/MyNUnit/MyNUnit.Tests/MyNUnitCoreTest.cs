// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit.Core.Tests;

using NUnit.Framework;

/// <summary>
/// Tests for MyNUnit framework.
/// </summary>
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

    /// <summary>
    /// Tests the correctness of the testing.
    /// </summary>
    /// <param name="path">Path to search for assemblies at.</param>
    /// <param name="testsPassed">Expected number of tests to pass.</param>
    /// <param name="testsFailed">Expected number of tests to fail.</param>
    /// <param name="testsIgnored">Expected number of tests to be ignored.</param>
    /// <returns>Task representing the test.</returns>
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

    /// <summary>
    /// Tests the consistency of the test results.
    /// </summary>
    /// <param name="path">Path to search for assemblies at.</param>
    /// <returns>Task representing the test.</returns>
    [TestCaseSource(nameof(TestDirectoryPaths))]
    public static async Task TestConsistency_RunTestsTwice_TestResultsAreEqual(string path)
    {
        var testResult1 = await MyNUnitCore.RunTestsFromEachAssembly(path);
        var testResult2 = await MyNUnitCore.RunTestsFromEachAssembly(path);
        Assert.That(testResult1, Is.EqualTo(testResult2));
    }

    /// <summary>
    /// Tests the testing for non existing directory.
    /// </summary>
    [TestAttribute]
    public static void TestStability_RunTestsForNonExistentDirectory_ThrowException()
        => Assert.ThrowsAsync<DirectoryNotFoundException>(
            () => MyNUnitCore.RunTestsFromEachAssembly("mboieuibnui"));

    private static string[] TestDirectoryPaths()
    {
        var paths = Directory.GetDirectories(TestProjectsPath);
        Array.Sort(paths);
        return paths;
    }

    private static IEnumerable<TestCaseData> CorrectnessTestCases()
    {
        var testDirectoryPaths = TestDirectoryPaths();
        for (int i = 0; i < testResults.Count; ++i)
        {
            var testDirectoryPath = testDirectoryPaths[i];
            var (passed, failed, ignored) = testResults[i];
            yield return new TestCaseData(testDirectoryPath, passed, failed, ignored);
        }
    }
}
