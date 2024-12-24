// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit.Core;

/// <summary>
/// Record representing a summary of multiple test runs.
/// </summary>
public sealed record TestSummary : IEquatable<TestSummary>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestSummary"/> class.
    /// </summary>
    public TestSummary()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestSummary"/> class.
    /// </summary>
    /// <param name="testResults">Collection of test results to collect summary from.</param>
    public TestSummary(IEnumerable<TestResult> testResults)
    {
        this.TestResults.UnionWith(testResults);
        foreach (var testResult in this.TestResults)
        {
            if (testResult.Ignored)
            {
                ++this.NumberOfTestsIgnored;
            }
            else if (testResult.Passed)
            {
                ++this.NumberOfTestsPassed;
            }
            else
            {
                ++this.NumberOfTestsFailed;
            }

            this.Elapsed += testResult.Elapsed;
        }
    }

    /// <summary>
    /// Gets the id of the test summary.
    /// </summary>
    public int TestSummaryId { get; private set; }

    /// <summary>
    /// Gets the collection of test results represented in this summary.
    /// </summary>
    public HashSet<TestResult> TestResults { get; } = new ();

    /// <summary>
    /// Gets the number of tests passed.
    /// </summary>
    public int NumberOfTestsPassed { get; private set; }

    /// <summary>
    /// Gets the number of tests failed.
    /// </summary>
    public int NumberOfTestsFailed { get; private set; }

    /// <summary>
    /// Gets the number of tests ignored.
    /// </summary>
    public int NumberOfTestsIgnored { get; private set; }

    /// <summary>
    /// Gets the total number of tests.
    /// </summary>
    public int NumberOfTests
        => this.NumberOfTestsPassed + this.NumberOfTestsFailed + this.NumberOfTestsIgnored;

    /// <summary>
    /// Gets the total time elapsed during the test runs.
    /// </summary>
    public TimeSpan Elapsed { get; private set; }

    /// <summary>
    /// Gets a value indicating whether all the run tests were passed.
    /// </summary>
    public bool AllTestsPassed => this.NumberOfTestsFailed == 0;

    public static TestSummary operator +(TestSummary result1, TestSummary result2)
        => new TestSummary(result1.TestResults.Union(result2.TestResults));

    /// <summary>
    /// Checks if current test summary instance is equal to the given one.
    /// </summary>
    /// <param name="other">The test summary to check equality to.</param>
    /// <returns>A value indicating whether the test summaries are equal.</returns>
    public bool Equals(TestSummary? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.NumberOfTestsPassed == other.NumberOfTestsPassed
            && this.NumberOfTestsFailed == other.NumberOfTestsFailed
            && this.NumberOfTestsIgnored == other.NumberOfTestsIgnored
            && this.TestResults.SetEquals(other.TestResults);
    }

    /// <summary>
    /// Gets the hash code of current test summary instance.
    /// </summary>
    /// <returns>The evaluated hash code.</returns>
    public override int GetHashCode()
        => this.TestResults.Sum(result => result.GetHashCode());

    /// <summary>
    /// Write summary of collected test runs to the console.
    /// </summary>
    public void Write()
    {
        Console.ForegroundColor = this.AllTestsPassed ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine(this.AllTestsPassed ? "All tests passed!" : "Some tests failed!");
        Console.WriteLine($"Number of tests passed: {this.NumberOfTestsPassed}");
        Console.WriteLine($"Number of tests failed: {this.NumberOfTestsFailed}");
        Console.WriteLine($"Number of tests ignored: {this.NumberOfTestsIgnored}");
        Console.WriteLine($"Total elapsed time: {this.Elapsed:mm\\:ss\\:fff} min:sec:ms");
        Console.ResetColor();
    }
}
