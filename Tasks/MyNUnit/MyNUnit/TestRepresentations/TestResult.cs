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
    private const string TestIndent = "- ";
    private const string ErrorMessageIndent = "=> ";

    /// <summary>
    /// Initializes a new instance of the <see cref="TestResult"/> class.
    /// </summary>
    /// <param name="test">The test to collect results from.</param>
    public TestResult(MyNUnitTest test)
    {
        this.AssemblyName = test.AssemblyName;
        this.ClassName = test.ClassName;
        this.MethodName = test.MethodName;
        this.IgnoreReason = test.IgnoreReason;
        this.ErrorMessage = test.ErrorMessage;
        this.Elapsed = test.Elapsed;
    }

    /// <summary>
    /// Gets the name of the assembly in which the test class was declared.
    /// </summary>
    public string? AssemblyName { get; }

    /// <summary>
    /// Gets the name of the class in which the test method was declared.
    /// </summary>
    public string? ClassName { get; }

    /// <summary>
    /// Gets the name of the test method that was run.
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    /// Gets a value indicating whether the test was passed.
    /// </summary>
    public bool Passed => this.ErrorMessage is null;

    /// <summary>
    /// Gets a value indicating whether the test was ignored.
    /// </summary>
    public bool Ignored => this.IgnoreReason is not null;

    /// <summary>
    /// Gets the reason to ignore the test.
    /// </summary>
    public string? IgnoreReason { get; }

    /// <summary>
    /// Gets the error message in case of test's fail.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets the time elapsed during the test run.
    /// </summary>
    public TimeSpan Elapsed { get; }

    private string Representation =>
        $"{this.AssemblyName}.{this.ClassName}.{this.MethodName}:" +
        $"{this.IgnoreReason}" + (this.Passed ? "passed" : "failed");

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

        return this.Representation == other.Representation;
    }

    /// <summary>
    /// Gets the hash code of current test result instance.
    /// </summary>
    /// <returns>The evaluated hash code.</returns>
    public override int GetHashCode()
        => this.Representation.GetHashCode();

    /// <summary>
    /// Write result of the test run to the console.
    /// </summary>
    public void Write()
    {
        if (this.Ignored)
        {
            Console.WriteLine(TestIndent + $"{this.MethodName}: ignored. Reason: {this.IgnoreReason}");
        }
        else if (this.Passed)
        {
            Console.WriteLine(TestIndent + $"{this.MethodName}: passed [{this.Elapsed.Milliseconds} ms]");
        }
        else
        {
            Console.WriteLine(TestIndent + $"{this.MethodName}: failed! [{this.Elapsed.Milliseconds} ms]");
            Console.WriteLine(ErrorMessageIndent + this.ErrorMessage + '\n');
        }
    }
}
