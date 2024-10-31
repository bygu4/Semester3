// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace CheckSum.Tests;

public static class CheckSumUtilsTest
{
    private const string TestFilesPath = "TestFiles/";

    public static IEnumerable<TestCaseData> TestCases()
    {
        foreach (var testFile in Directory.GetFileSystemEntries(TestFilesPath))
        {
            yield return new TestCaseData(testFile);
        }
    }

    [TestCaseSource(nameof(TestCases))]
    public static async Task TestCheckSumEvaluation_CorrectCases_CheckSumsAreEqual(string path)
    {
        var hash1 = CheckSumUtils.GetCheckSumSequentially(path);
        var hash2 = await CheckSumUtils.GetCheckSumConcurrently(path);
        Assert.That(hash1, Is.EqualTo(hash2));
    }

    [Test]
    public static void TestCheckSumEvaluation_UnexistentFile_ThrowException()
    {
        Assert.Throws<FileNotFoundException>(
            () => CheckSumUtils.GetCheckSumSequentially("agsgdffdghgf"));
        Assert.ThrowsAsync<FileNotFoundException>(
            async () => await CheckSumUtils.GetCheckSumConcurrently("agsgdffdghgf"));
    }
}
