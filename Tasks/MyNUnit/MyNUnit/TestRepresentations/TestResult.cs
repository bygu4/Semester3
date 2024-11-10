// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit.Core;

/// <summary>
/// Record representing a result of the test run.
/// </summary>
public sealed record TestResult : IEquatable<TestResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestResult"/> class.
    /// </summary>
    public TestResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestResult"/> class.
    /// </summary>
    /// <param name="numberOfTestsPassed">Number of tests passed.</param>
    /// <param name="numberOfTestsFailed">Number of tests failed.</param>
    /// <param name="numberOfTestsIgnored">Number of tests ignored.</param>
    /// <param name="elapsed">Total time elapsed during the test run.</param>
    public TestResult(
        int numberOfTestsPassed,
        int numberOfTestsFailed,
        int numberOfTestsIgnored,
        TimeSpan elapsed)
    {
        this.NumberOfTestsPassed = numberOfTestsPassed;
        this.NumberOfTestsFailed = numberOfTestsFailed;
        this.NumberOfTestsIgnored = numberOfTestsIgnored;
        this.Elapsed = elapsed;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestResult"/> class.
    /// </summary>
    /// <param name="tests">Collection of tests to collect results from.</param>
    public TestResult(IEnumerable<MyNUnitTest> tests)
    {
        foreach (var test in tests)
        {
            if (test.Ignored)
            {
                ++this.NumberOfTestsIgnored;
            }
            else if (test.Passed)
            {
                ++this.NumberOfTestsPassed;
            }
            else
            {
                ++this.NumberOfTestsFailed;
            }

            this.Elapsed += test.Elapsed;
        }
    }

    /// <summary>
    /// Gets the number of tests passed.
    /// </summary>
    public int NumberOfTestsPassed { get; }

    /// <summary>
    /// Gets the number of tests failed.
    /// </summary>
    public int NumberOfTestsFailed { get; }

    /// <summary>
    /// Gets the number of tests ignored.
    /// </summary>
    public int NumberOfTestsIgnored { get; }

    /// <summary>
    /// Gets the total number of tests.
    /// </summary>
    public int NumberOfTests
        => this.NumberOfTestsPassed + this.NumberOfTestsFailed + this.NumberOfTestsIgnored;

    /// <summary>
    /// Gets the total time elapsed during the test runs.
    /// </summary>
    public TimeSpan Elapsed { get; }

    /// <summary>
    /// Gets a value indicating whether all the run tests were passed.
    /// </summary>
    public bool AllTestsPassed => this.NumberOfTestsFailed == 0;

    public static TestResult operator +(TestResult result1, TestResult result2)
        => new TestResult(
            result1.NumberOfTestsPassed + result2.NumberOfTestsPassed,
            result1.NumberOfTestsFailed + result2.NumberOfTestsFailed,
            result1.NumberOfTestsIgnored + result2.NumberOfTestsIgnored,
            result1.Elapsed + result2.Elapsed);

    /// <summary>
    /// Checks if current test result instance is equal to the given one.
    /// </summary>
    /// <param name="other">The test result to check equality to.</param>
    /// <returns>A value indicating whether the test results are equal.</returns>
    public bool Equals(TestResult? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.NumberOfTestsPassed == other.NumberOfTestsPassed
            && this.NumberOfTestsFailed == other.NumberOfTestsFailed
            && this.NumberOfTestsIgnored == other.NumberOfTestsIgnored;
    }

    /// <summary>
    /// Gets the hash code of current test result instance.
    /// </summary>
    /// <returns>The evaluated hash code.</returns>
    public override int GetHashCode()
        => this.NumberOfTestsPassed + this.NumberOfTestsFailed ^ 2 + this.NumberOfTestsIgnored ^ 3;

    /// <summary>
    /// Write summary of the test run to the console.
    /// </summary>
    public void WriteTestSummary()
    {
        Console.WriteLine(this.AllTestsPassed ? "All tests passed!" : "Some tests failed!");
        Console.WriteLine($"Number of tests passed: {this.NumberOfTestsPassed}");
        Console.WriteLine($"Number of tests failed: {this.NumberOfTestsFailed}");
        Console.WriteLine($"Number of tests ignored: {this.NumberOfTestsIgnored}");
        Console.WriteLine($"Total elapsed time: {this.Elapsed:mm\\:ss\\:fff} min:sec:ms");
    }
}
