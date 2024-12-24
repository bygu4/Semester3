// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

#pragma warning disable SA1010 // Opening square brackets should be spaced correctly

namespace MyNUnitWeb.Pages;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyNUnit.Core;
using MyNUnitWeb.Data;

/// <summary>
/// The page model of a result of the test run.
/// </summary>
/// <param name="dbContext">The database context to use.</param>
public class TestResultModel(TestRunDbContext dbContext)
    : PageModel
{
    /// <summary>
    /// Gets or sets the result of the test run.
    /// </summary>
    public TestRun? TestRun { get; set; }

    /// <summary>
    /// Gets the dictionary containing a test result collection by the assembly name.
    /// </summary>
    public Dictionary<string, List<TestResult>> TestResultsByAssembly { get; } = new ();

    /// <summary>
    /// Displays the test result by its id in the database.
    /// </summary>
    /// <param name="id">The id of the test run in the database.</param>
    /// <returns>The task representing the test result obtaining.</returns>
    public async Task OnGetAsync(int id)
    {
        this.TestRun = await dbContext.TestRuns
            .Where(t => t.TestRunId == id)
            .Include(t => t.Summary.TestResults)
            .FirstOrDefaultAsync();
        this.InitializeTestResults();
    }

    private void InitializeTestResults()
    {
        if (this.TestRun is null)
        {
            return;
        }

        var testResults = this.TestRun.Summary.TestResults.OrderBy(t => t.ClassName);
        foreach (var testResult in testResults)
        {
            var assemblyName = testResult.AssemblyName is not null ? testResult.AssemblyName : string.Empty;
            if (this.TestResultsByAssembly.TryGetValue(assemblyName, out var value))
            {
                value.Add(testResult);
            }
            else
            {
                this.TestResultsByAssembly[assemblyName] = [testResult];
            }
        }
    }
}
