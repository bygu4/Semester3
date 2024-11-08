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
    /// <returns>A value indicating whether the tests were passed.</returns>
    public static async Task<bool> RunTestsForEachAssembly(string path)
    {
        var testAssemblies = Directory.EnumerateFiles(path, "*.dll").Select(Assembly.LoadFrom);
        var testRuns = new List<Task<(bool, string)>>();
        foreach (var testAssembly in testAssemblies)
        {
            testRuns.Add(new AssemblyTestRunner(testAssembly).RunTestsForEachClass());
        }

        var allTestsPassed = true;
        foreach (var testRun in testRuns)
        {
            var (assemblyTestsPassed, assemblyTestsSummary) = await testRun;
            allTestsPassed &= assemblyTestsPassed;
            Console.Write(assemblyTestsSummary);
        }

        return allTestsPassed;
    }
}
