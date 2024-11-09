// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit.Core;

using System.Reflection;

/// <summary>
/// Class for running tests from given class and collecting test results.
/// </summary>
public class ClassTestCollector
{
    private const string TestIndent = "- ";

    private readonly TypeInfo testClass;
    private readonly object? testObject = null;
    private readonly List<MyNUnitTest> tests = new ();

    private MethodInfo? beforeClass = null;
    private MethodInfo? afterClass = null;
    private MethodInfo? beforeTest = null;
    private MethodInfo? afterTest = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassTestCollector"/> class.
    /// </summary>
    /// <param name="testClass">Class to collect tests from.</param>
    public ClassTestCollector(TypeInfo testClass)
    {
        this.testClass = testClass;
        if (this.testClass.GetConstructor(Type.EmptyTypes) is not null)
        {
            this.testObject = Activator.CreateInstance(testClass);
        }
    }

    /// <summary>
    /// Gets the name of the class to test.
    /// </summary>
    public string ClassName => this.testClass.Name;

    /// <summary>
    /// Gets the result of the test run.
    /// </summary>
    public TestResult TestResult { get; private set; } = new ();

    /// <summary>
    /// Run tests defined in the class according to the attributes and collect results.
    /// </summary>
    /// <returns>The test collector instance after the test result collection.</returns>
    public ClassTestCollector CollectAndRunTests()
    {
        foreach (var method in this.testClass.GetMethods())
        {
            this.ResolveMethodAttributes(method);
        }

        this.beforeClass?.Invoke(null, null);
        foreach (var test in this.tests)
        {
            this.beforeTest?.Invoke(this.testObject, null);
            test.Run();
            this.afterTest?.Invoke(this.testObject, null);
        }

        this.afterClass?.Invoke(null, null);
        this.TestResult = new (this.tests);
        return this;
    }

    /// <summary>
    /// Write the summary of the test run for class to the console.
    /// </summary>
    public void WriteTestSummary()
    {
        Console.WriteLine($"{this.ClassName}:");
        foreach (var test in this.tests)
        {
            if (test.Ignored)
            {
                Console.WriteLine(TestIndent + $"{test.MethodName}: ignored. Reason: {test.IgnoreReason}");
            }
            else if (test.Passed)
            {
                Console.WriteLine(TestIndent + $"{test.MethodName}: passed [{test.Elapsed.Milliseconds} ms]");
            }
            else
            {
                Console.WriteLine(TestIndent + $"{test.MethodName}: failed! [{test.Elapsed.Milliseconds} ms]");
                Console.WriteLine(test.ErrorMessage);
            }
        }
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
