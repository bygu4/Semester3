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

    /// <summary>
    /// Gets the name of the test method to run.
    /// </summary>
    public string MethodName => this.testMethod.Name;

    /// <summary>
    /// Gets the reason to ignore the test.
    /// </summary>
    public string? IgnoreReason { get; } = ignoreReason;

    /// <summary>
    /// Gets a value indicating whether the run test was passed.
    /// </summary>
    public bool Passed { get; private set; }

    /// <summary>
    /// Gets the error message in case of test's fail.
    /// </summary>
    public string? ErrorMessage { get; private set; }

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
        if (this.IgnoreReason is not null)
        {
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
                this.ErrorMessage = e.ToString();
                return;
            }
        }

        if (this.expectedException is not null)
        {
            this.Passed = false;
            this.ErrorMessage = $"Expected exception: {this.expectedException}\n";
            return;
        }
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