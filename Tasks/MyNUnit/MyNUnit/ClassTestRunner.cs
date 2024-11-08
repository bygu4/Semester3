// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit.Core;

using System.Reflection;

/// <summary>
/// Class for running tests from the given class.
/// </summary>
public class ClassTestRunner
{
    private readonly TypeInfo testClass;
    private readonly object? testObject = null;
    private readonly List<MyNUnitTest> tests = new ();

    private MethodInfo? beforeClass = null;
    private MethodInfo? afterClass = null;

    private MethodInfo? beforeTest = null;
    private MethodInfo? afterTest = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassTestRunner"/> class.
    /// </summary>
    /// <param name="testClass">Class to run tests from.</param>
    public ClassTestRunner(TypeInfo testClass)
    {
        this.testClass = testClass;
        if (this.testClass.GetConstructor(Type.EmptyTypes) is not null)
        {
            this.testObject = Activator.CreateInstance(testClass);
        }
    }

    /// <summary>
    /// Gets a value indicating whether all the run tests were passed.
    /// </summary>
    public bool AllTestsPassed { get; private set; }

    /// <summary>
    /// Gets the summary about the tests run.
    /// </summary>
    public string? Summary { get; private set; }

    /// <summary>
    /// Gets the total number of tests run.
    /// </summary>
    public int NumberOfTests { get; private set; }

    /// <summary>
    /// Gets the number of tests passed.
    /// </summary>
    public int NumberOfTestsPassed { get; private set; }

    /// <summary>
    /// Gets the number of tests failed.
    /// </summary>
    public int NumberOfTestsFailed => this.NumberOfTests - this.NumberOfTestsPassed;

    /// <summary>
    /// Gets the total time elapsed during the test runs.
    /// </summary>
    public TimeSpan Elapsed { get; private set; }

    /// <summary>
    /// Run tests defined in the class according to the attributes.
    /// </summary>
    public void RunTestsInClass()
    {
        foreach (var method in this.testClass.GetMethods())
        {
            this.ResolveMethodAttributes(method);
        }

        this.AllTestsPassed = true;
        this.Summary = $"{this.testClass.Name}:\n";

        this.beforeClass?.Invoke(null, null);
        foreach (var test in this.tests)
        {
            this.beforeTest?.Invoke(this.testObject, null);
            test.Run();
            this.afterTest?.Invoke(this.testObject, null);

            this.AllTestsPassed &= test.Passed;
            this.Summary += test.Message;
            this.Elapsed += test.Elapsed;
            ++this.NumberOfTests;
            if (test.Passed)
            {
                ++this.NumberOfTestsPassed;
            }
        }

        this.afterClass?.Invoke(null, null);
    }

    private void ResolveMethodAttributes(MethodInfo method)
    {
        bool isTest = false;
        Type? expectedException = null;
        string? ignoreReason = null;

        foreach (var attribute in method.GetCustomAttributes())
        {
            var attributeType = attribute.GetType();
            if (attributeType == typeof(Test))
            {
                isTest = true;
            }
            else if (attributeType == typeof(Expected))
            {
                expectedException = ((Expected)attribute).ExceptionType;
            }
            else if (attributeType == typeof(Ignore))
            {
                ignoreReason = ((Ignore)attribute).Reason;
            }
            else if (attributeType == typeof(BeforeClass))
            {
                this.beforeClass = method;
            }
            else if (attributeType == typeof(AfterClass))
            {
                this.afterClass = method;
            }
            else if (attributeType == typeof(Before))
            {
                this.beforeTest = method;
            }
            else if (attributeType == typeof(After))
            {
                this.afterTest = method;
            }
        }

        if (isTest)
        {
            this.tests.Add(new MyNUnitTest(
                this.testObject,
                method,
                expectedException,
                ignoreReason));
        }
    }
}
