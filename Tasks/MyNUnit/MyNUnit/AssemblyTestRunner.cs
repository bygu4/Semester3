// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit.Core;

using System.Reflection;

/// <summary>
/// Class for running tests from the given assembly.
/// </summary>
/// <param name="testAssembly">Assembly to run tests from.</param>
public class AssemblyTestRunner(Assembly testAssembly)
{
    private readonly Assembly testAssembly = testAssembly;

    /// <summary>
    /// Run tests from each class found in the assembly.
    /// Each class is tested in parallel.
    /// </summary>
    /// <returns>A task representing the test run.</returns>
    public async Task<(bool, string)> RunTestsForEachClass()
    {
        var testClasses = this.testAssembly.DefinedTypes.Where(t => t.IsClass);
        var classTestRunners = new List<ClassTestRunner>();
        var testRuns = new List<Task>();

        var numberOfTestsPassed = 0;
        var numberOfTestsFailed = 0;
        var elapsedTime = default(TimeSpan);

        foreach (var testClass in testClasses)
        {
            var runner = new ClassTestRunner(testClass);
            classTestRunners.Add(runner);
            testRuns.Add(Task.Run(runner.RunTestsInClass));
        }

        var allTestsPassed = true;
        var assemblyTestsSummary = $"Running tests from {this.testAssembly.GetName()}:\n";
        for (int i = 0; i < testRuns.Count; ++i)
        {
            await testRuns[i];
            var runner = classTestRunners[i];

            allTestsPassed &= runner.AllTestsPassed;
            assemblyTestsSummary += runner.Summary;
            numberOfTestsPassed += runner.NumberOfTestsPassed;
            numberOfTestsFailed += runner.NumberOfTestsFailed;
            elapsedTime += runner.Elapsed;
        }

        assemblyTestsSummary += $"Tests passed: {numberOfTestsPassed}\n";
        assemblyTestsSummary += $"Tests failed: {numberOfTestsFailed}\n";
        assemblyTestsSummary += $"Total elapsed time: {elapsedTime}\n\n";
        return (allTestsPassed, assemblyTestsSummary);
    }
}
