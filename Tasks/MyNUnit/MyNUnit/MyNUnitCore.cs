// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit.Core;

using System.Reflection;

/// <summary>
/// The core of MyNUnit testing framework.
/// Used for running tests from assemblies at the given path.
/// </summary>
public static class MyNUnitCore
{
    /// <summary>
    /// Run tests from each assembly found at the given path.
    /// Each assembly is tested in parallel.
    /// </summary>
    /// <param name="path">Path to look for assemblies at.</param>
    /// <returns>A value indicating whether all the tests were passed.</returns>
    public static async Task<bool> RunTestsFromEachAssembly(string path)
    {
        var testAssemblies = Directory.EnumerateFiles(path, "*.dll").Select(Assembly.LoadFrom);
        var tasks = new List<Task<AssemblyTestCollector>>();

        foreach (var testAssembly in testAssemblies)
        {
            tasks.Add(new AssemblyTestCollector(testAssembly).CollectAndRunTests());
        }

        var allTestsPassed = true;
        foreach (var task in tasks)
        {
            var collector = await task;
            collector.CollectAndWriteTestResults();
            allTestsPassed &= collector.AllTestsPassed;
        }

        return allTestsPassed;
    }
}
