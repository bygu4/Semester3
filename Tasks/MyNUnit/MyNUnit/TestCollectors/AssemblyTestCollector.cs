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
    private const string TestIndent = "- ";

    private readonly Assembly testAssembly = testAssembly;
    private readonly List<ClassTestCollector> classTestCollectors = new ();

    /// <summary>
    /// Gets the name of the assembly to test.
    /// </summary>
    public string AssemblyName => this.testAssembly.GetName().ToString();

    /// <summary>
    /// Gets the number of tests passed.
    /// </summary>
    public int NumberOfTestsPassed { get; private set; }

    /// <summary>
    /// Gets the number of tests failed.
    /// </summary>
    public int NumberOfTestsFailed { get; private set; }

    /// <summary>
    /// Gets the number of tests skipped.
    /// </summary>
    public int NumberOfTestsSkipped { get; private set; }

    /// <summary>
    /// Gets the total time elapsed during the test runs.
    /// </summary>
    public TimeSpan Elapsed { get; private set; }

    /// <summary>
    /// Gets a value indicating whether all the run tests were passed.
    /// </summary>
    public bool AllTestsPassed { get; private set; } = true;

    /// <summary>
    /// Run tests from each class found in the assembly.
    /// Each class is tested in parallel.
    /// </summary>
    /// <returns>The assembly test collector instance after the test run.</returns>
    public async Task<AssemblyTestCollector> CollectAndRunTests()
    {
        var testClasses = this.testAssembly.DefinedTypes.Where(t => t.IsClass);
        var tasks = new List<Task<ClassTestCollector>>();

        foreach (var testClass in testClasses)
        {
            tasks.Add(Task.Run(() => new ClassTestCollector(testClass).CollectAndRunTests()));
        }

        foreach (var task in tasks)
        {
            var testCollector = await task;
            this.classTestCollectors.Add(testCollector);
            this.UpdateTestResults(testCollector);
        }

        return this;
    }

    /// <summary>
    /// Write the summary of collected tests to the console.
    /// </summary>
    public void WriteTestSummary()
    {
        Console.Write('\n');
        Console.WriteLine($"{this.AssemblyName} test results:");
        foreach (var testCollector in this.classTestCollectors)
        {
            this.WriteClassTestResults(testCollector);
        }

        Console.WriteLine(this.AllTestsPassed ? "All tests passed!" : "Some tests failed!");
        Console.WriteLine($"Number of tests passed: {this.NumberOfTestsPassed}");
        Console.WriteLine($"Number of tests failed: {this.NumberOfTestsFailed}");
        Console.WriteLine($"Number of tests skipped: {this.NumberOfTestsSkipped}");
        Console.WriteLine($"Total elapsed time: {this.Elapsed:mm-ss} mm-ss");
    }

    private void UpdateTestResults(ClassTestCollector testCollector)
    {
        foreach (var test in testCollector.Tests)
        {
            this.AllTestsPassed &= test.Passed;
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

    private void WriteClassTestResults(ClassTestCollector testCollector)
    {
        Console.WriteLine($"{testCollector.ClassName}:");
        foreach (var test in testCollector.Tests)
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
}
