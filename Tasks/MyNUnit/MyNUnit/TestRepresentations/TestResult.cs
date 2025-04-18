// Copyright (c) Alexander Bugaev 2024
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
    /// <param name="assemblyName">Name of the test assembly.</param>
    /// <param name="className">Name of the class in which the test method was defined.</param>
    /// <param name="methodName">Name of the test method that was run.</param>
    /// <param name="ignoreReason">Reason to ignore the test.</param>
    /// <param name="errorMessage">Error message in case of test's fail.</param>
    /// <param name="elapsed">Time elapsed during the test run.</param>
    public TestResult(
        string? assemblyName,
        string? className,
        string methodName,
        string? ignoreReason,
        string? errorMessage,
        TimeSpan elapsed)
    {
        this.AssemblyName = assemblyName;
        this.ClassName = className;
        this.MethodName = methodName;
        this.IgnoreReason = ignoreReason;
        this.ErrorMessage = errorMessage;
        this.Elapsed = elapsed;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestResult"/> class.
    /// </summary>
    /// <param name="test">The test to collect results from.</param>
    public TestResult(MyNUnitTest test)
        : this(test.AssemblyName, test.ClassName, test.MethodName, test.IgnoreReason, test.ErrorMessage, test.Elapsed)
    {
    }

    /// <summary>
    /// Gets the id of the test result.
    /// </summary>
    public int TestResultId { get; private set; }

    /// <summary>
    /// Gets the name of the assembly in which the test class was declared.
    /// </summary>
    public string? AssemblyName { get; private set; }

    /// <summary>
    /// Gets the name of the class in which the test method was declared.
    /// </summary>
    public string? ClassName { get; private set; }

    /// <summary>
    /// Gets the name of the test method that was run.
    /// </summary>
    public string MethodName { get; private set; }

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
    public string? IgnoreReason { get; private set; }

    /// <summary>
    /// Gets the error message in case of test's fail.
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Gets the time elapsed during the test run.
    /// </summary>
    public TimeSpan Elapsed { get; private set; }

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
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(TestIndent + $"{this.MethodName}: ignored. Reason: {this.IgnoreReason}");
        }
        else if (this.Passed)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(TestIndent + $"{this.MethodName}: passed [{this.Elapsed.Milliseconds} ms]");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(TestIndent + $"{this.MethodName}: failed! [{this.Elapsed.Milliseconds} ms]");
            Console.WriteLine(ErrorMessageIndent + this.ErrorMessage + '\n');
        }

        Console.ResetColor();
    }
}
