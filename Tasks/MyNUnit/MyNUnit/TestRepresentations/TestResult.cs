// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using MyNUnit.Core;

/// <summary>
/// Record representing a result of the test run.
/// </summary>
public record TestResult
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
    /// <param name="numberOfTestsSkipped">Number of tests skipped.</param>
    /// <param name="elapsed">Total time elapsed during the test run.</param>
    public TestResult(
        int numberOfTestsPassed,
        int numberOfTestsFailed,
        int numberOfTestsSkipped,
        TimeSpan elapsed)
    {
        this.NumberOfTestsPassed = numberOfTestsPassed;
        this.NumberOfTestsFailed = numberOfTestsFailed;
        this.NumberOfTestsSkipped = numberOfTestsSkipped;
        this.Elapsed = elapsed;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestResult"/> class.
    /// </summary>
    /// <param name="tests">Collection of tests to collect results from.</param>
    public TestResult(IList<MyNUnitTest> tests)
    {
        foreach (var test in tests)
        {
            if (test.Ignored)
            {
                ++this.NumberOfTestsSkipped;
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
    /// Gets the number of tests skipped.
    /// </summary>
    public int NumberOfTestsSkipped { get; }

    /// <summary>
    /// Gets the total time elapsed during the test runs.
    /// </summary>
    public TimeSpan Elapsed { get; }

    /// <summary>
    /// Gets a value indicating whether all the run tests were passed.
    /// </summary>
    public bool AllTestsPassed => this.NumberOfTestsFailed == 0;

    public static TestResult operator +(TestResult result1, TestResult result2)
    {
        return new TestResult(
            result1.NumberOfTestsPassed + result2.NumberOfTestsPassed,
            result1.NumberOfTestsFailed + result2.NumberOfTestsFailed,
            result1.NumberOfTestsSkipped + result2.NumberOfTestsSkipped,
            result1.Elapsed + result2.Elapsed);
    }

    /// <summary>
    /// Write the summary of test results to the console.
    /// </summary>
    public void WriteTestSummary()
    {
        Console.WriteLine(this.AllTestsPassed ? "All tests passed!" : "Some tests failed!");
        Console.WriteLine($"Number of tests passed: {this.NumberOfTestsPassed}");
        Console.WriteLine($"Number of tests failed: {this.NumberOfTestsFailed}");
        Console.WriteLine($"Number of tests skipped: {this.NumberOfTestsSkipped}");
        Console.WriteLine($"Total elapsed time: {this.Elapsed:mm-ss} mm-ss");
    }
}
