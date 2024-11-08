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
    public bool AllTestsPassed { get; private set; }

    /// <summary>
    /// Run tests from each class found in the assembly.
    /// Each class is tested in parallel.
    /// </summary>
    /// <returns>The assembly test collector instance after test run.</returns>
    public async Task<AssemblyTestCollector> CollectAndRunTests()
    {
        var testClasses = this.testAssembly.DefinedTypes.Where(t => t.IsClass);
        var tasks = new List<Task<ClassTestCollector>>();

        foreach (var testClass in testClasses)
        {
            var runner = new ClassTestCollector(testClass);
            tasks.Add(Task.Run(() => new ClassTestCollector(testClass).CollectAndRunTests()));
        }

        foreach (var task in tasks)
        {
            this.classTestCollectors.Add(await task);
        }

        return this;
    }

    /// <summary>
    /// Updates current instance properties and writes test summary to the console.
    /// </summary>
    public void CollectAndWriteTestResults()
    {
        this.AllTestsPassed = true;
        Console.WriteLine($"{this.AssemblyName} test results:");
        foreach (var classTestCollector in this.classTestCollectors)
        {
            this.CollectAndWriteClassTestResults(classTestCollector);
        }

        Console.Write('\n');
        Console.WriteLine(this.AllTestsPassed ? "All tests passed!" : "Some tests failed!");
        Console.WriteLine($"Number of tests passed: {this.NumberOfTestsPassed}");
        Console.WriteLine($"Number of tests failed: {this.NumberOfTestsFailed}");
        Console.WriteLine($"Number of tests skipped: {this.NumberOfTestsSkipped}");
        Console.WriteLine($"Total elapsed time: {this.Elapsed:mm-ss} mm-ss\n");
    }

    private void CollectAndWriteClassTestResults(ClassTestCollector collector)
    {
        this.AllTestsPassed = true;
        Console.WriteLine($"{collector.ClassName}:");
        foreach (var test in collector.Tests)
        {
            this.AllTestsPassed &= test.Passed;
            if (test.IgnoreReason is not null)
            {
                Console.WriteLine(TestIndent + $"{test.MethodName}: ignored. Reason: {test.IgnoreReason}");
                ++this.NumberOfTestsSkipped;
            }
            else if (test.Passed)
            {
                Console.WriteLine(TestIndent + $"{test.MethodName}: passed [{test.Elapsed.Milliseconds} ms]");
                ++this.NumberOfTestsPassed;
            }
            else
            {
                Console.WriteLine(TestIndent + $"{test.MethodName}: failed! [{test.Elapsed.Milliseconds} ms]");
                Console.WriteLine(test.ErrorMessage);
                ++this.NumberOfTestsFailed;
            }

            this.Elapsed += test.Elapsed;
        }
    }
}
