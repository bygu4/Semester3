// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit.Core;

using System.Diagnostics;
using System.Reflection;

/// <summary>
/// Class representing a test.
/// </summary>
/// <param name="testObject">An instance of the object containing the test method.</param>
/// <param name="testMethod">The test method to run.</param>
/// <param name="expectedException">The exception expected to be thrown.</param>
/// <param name="ignoreReason">The reason to ignore the test.</param>
public class MyNUnitTest(
    object? testObject,
    MethodInfo testMethod,
    Type? expectedException,
    string? ignoreReason)
{
    private readonly object? testObject = testObject;
    private readonly MethodInfo testMethod = testMethod;
    private readonly Type? expectedException = expectedException;
    private readonly string? ignoreReason = ignoreReason;

    /// <summary>
    /// Gets a value indicating whether the run test was passed.
    /// </summary>
    public bool Passed { get; private set; }

    /// <summary>
    /// Gets the message containing the test result.
    /// </summary>
    public string? Message { get; private set; }

    /// <summary>
    /// Gets the time elapsed during the test run.
    /// </summary>
    public TimeSpan Elapsed { get; private set; }

    /// <summary>
    /// Run the test.
    /// </summary>
    public void Run()
    {
        this.Passed = true;
        this.Message = $"-- {this.testMethod.Name}: ";
        if (this.ignoreReason is not null)
        {
            this.Message += $"ignored\nReason: {this.ignoreReason}\n";
            return;
        }

        try
        {
            this.InvokeTestMethodAndCountTime();
        }
        catch (Exception e)
        {
            if (e.GetType() != this.expectedException)
            {
                this.Passed = false;
                this.Message += $"failed! [{this.Elapsed.Milliseconds} ms]\n";
                this.Message += $"Message: {e.Message}\n";
                this.Message += $"Stack Trace:\n{e.StackTrace}\n";
                return;
            }
        }

        if (this.expectedException is not null)
        {
            this.Passed = false;
            this.Message += $"failed! [{this.Elapsed.Milliseconds} ms]\n";
            this.Message += $"Expected exception: {this.expectedException}\n";
            return;
        }

        this.Message += $"passed [{this.Elapsed.Milliseconds} ms]\n";
    }

    private void InvokeTestMethodAndCountTime()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            this.testMethod.Invoke(this.testObject, null);
        }
        finally
        {
            this.Elapsed = stopwatch.Elapsed;
        }
    }
}
