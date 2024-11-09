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

    private static IEnumerable<TestCaseData> TestCases()
    {
        for (int i = 0; i < testResults.Count; ++i)
        {
            var testDirectoryPath = TestDirectoryPaths[i];
            var (passed, failed, skipped) = testResults[i];
            var testResult = new TestResult(passed, failed, skipped, default);
            yield return new TestCaseData(testDirectoryPath, testResult);
        }
    }

    [TestCaseSource(nameof(TestCases))]
    public static async Task TestMyNUnitCore_RunTestsForEachAssembly_GetTestResults(
        string path, TestResult expectedTestResult)
    {
        var actualTestResult = await MyNUnitCore.RunTestsFromEachAssembly(path);
        Assert.That(actualTestResult, Is.EqualTo(expectedTestResult));
    }

    [TestAttribute]
    public static void TestMyNUnitCore_RunTestsForNonExistentDirectory_ThrowException()
        => Assert.ThrowsAsync<DirectoryNotFoundException>(
            () => MyNUnitCore.RunTestsFromEachAssembly("mboieuibnui"));
}
