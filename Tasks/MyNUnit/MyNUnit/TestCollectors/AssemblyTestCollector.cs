// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit.Core;

using System.Reflection;

/// <summary>
/// Class for running tests from given assembly and collecting test results.
/// </summary>
/// <param name="testAssembly">Assembly to collect tests from.</param>
public class AssemblyTestCollector(Assembly testAssembly)
{
    private readonly Assembly testAssembly = testAssembly;
    private readonly List<ClassTestCollector> testCollectors = new ();

    /// <summary>
    /// Gets the name of the assembly to test.
    /// </summary>
    public string? AssemblyName => this.testAssembly.GetName().Name;

    /// <summary>
    /// Gets the result of the test run.
    /// </summary>
    public TestResult TestResult { get; private set; } = new ();

    /// <summary>
    /// Run tests from each class found in the assembly and collect results.
    /// Each class is tested in parallel.
    /// </summary>
    /// <returns>The test collector instance after the test result collection.</returns>
    public async Task<AssemblyTestCollector> CollectAndRunTests()
    {
        var testClasses = this.testAssembly.GetExportedTypes().Where(t => t.IsClass);
        var tasks = new List<Task<ClassTestCollector>>();

        foreach (var testClass in testClasses)
        {
            tasks.Add(Task.Run(() => new ClassTestCollector(testClass).CollectAndRunTests()));
        }

        foreach (var task in tasks)
        {
            var testCollector = await task;
            this.testCollectors.Add(testCollector);
            this.TestResult += testCollector.TestResult;
        }

        return this;
    }

    /// <summary>
    /// Write summary of the test run for assembly to the console.
    /// </summary>
    public void WriteTestSummary()
    {
        if (this.TestResult.NumberOfTests == 0)
        {
            return;
        }

        Console.WriteLine($"\n{this.AssemblyName} test results:\n");
        foreach (var testCollector in this.testCollectors)
        {
            testCollector.WriteTestSummary();
        }

        Console.Write('\n');
        this.TestResult.WriteTestSummary();
    }
}
