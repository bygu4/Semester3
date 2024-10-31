// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace CheckSum.Tests;

public class CheckSumUtilsTest
{
    private const string TestFilesPath = "TestFiles/";

    public static IEnumerable<TestCaseData> TestCases()
    {
        foreach (var testDirectory in Directory.GetDirectories(TestFilesPath))
        {
            yield return new TestCaseData(testDirectory);
        }
    }

    [TestCaseSource(nameof(TestCases))]
    public async Task TestCheckSumEvaluation(string path)
    {
        var hash1 = CheckSumUtils.GetCheckSumSequentially(path);
        var hash2 = await CheckSumUtils.GetCheckSumConcurrently(path);
        Assert.That(hash1, Is.EqualTo(hash2));
    }
}
