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
    private readonly TypeInfo testClass;
    private readonly object? testObject = null;

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
    /// Gets tests collected from the class.
    /// </summary>
    public List<MyNUnitTest> Tests { get; } = new ();

    /// <summary>
    /// Run tests defined in the class according to the attributes.
    /// </summary>
    /// <returns>The class test collector instance after the test run.</returns>
    public ClassTestCollector CollectAndRunTests()
    {
        foreach (var method in this.testClass.GetMethods())
        {
            this.ResolveMethodAttributes(method);
        }

        this.beforeClass?.Invoke(null, null);
        foreach (var test in this.Tests)
        {
            this.beforeTest?.Invoke(this.testObject, null);
            test.Run();
            this.afterTest?.Invoke(this.testObject, null);
        }

        this.afterClass?.Invoke(null, null);
        return this;
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
            this.Tests.Add(new MyNUnitTest(
                this.testObject,
                method,
                expectedException,
                ignoreReason));
        }
    }
}
