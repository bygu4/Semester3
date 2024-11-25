// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit.Core;

using System.Reflection;

/// <summary>
/// The core of MyNUnit testing framework.
/// Used for running tests from assemblies at the given path and collecting test results.
/// </summary>
public static class MyNUnitCore
{
    /// <summary>
    /// Run tests from each assembly found at the given path and collect results.
    /// Each assembly is tested in parallel.
    /// </summary>
    /// <param name="path">Path to look for assemblies at.</param>
    /// <param name="writeToConsole">Write info about the test run to the console.</param>
    /// <returns>The test summary representation.</returns>
    public static async Task<TestSummary> RunTestsFromEachAssembly(
        string path, bool writeToConsole = false)
    {
        var testAssemblies = Directory.EnumerateFiles(path, "*.dll").Select(Assembly.LoadFrom);
        var tasks = new List<Task<AssemblyTestCollector>>();

        foreach (var testAssembly in testAssemblies)
        {
            if (writeToConsole)
            {
                Console.WriteLine($"Running tests from {testAssembly.GetName()}");
            }

            tasks.Add(new AssemblyTestCollector(testAssembly).CollectAndRunTests());
        }

        var testSummary = new TestSummary();
        foreach (var task in tasks)
        {
            var testCollector = await task;
            testSummary += testCollector.TestSummary;
            if (writeToConsole)
            {
                testCollector.WriteTestSummary();
            }
        }

        return testSummary;
    }
}
