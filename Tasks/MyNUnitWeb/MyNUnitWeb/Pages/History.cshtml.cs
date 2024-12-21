// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

#pragma warning disable SA1010 // Opening square brackets should be spaced correctly

namespace MyNUnitWeb.Pages;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyNUnitWeb.Data;

/// <summary>
/// The page model of the test run history.
/// </summary>
/// <param name="dbContext">The database context to use.</param>
public class HistoryModel(TestResultDbContext dbContext)
    : PageModel
{
    /// <summary>
    /// Gets or sets the result of the test run.
    /// </summary>
    public IList<TestResultData> TestResults { get; set; } = [];

    /// <summary>
    /// Displays the history of the test runs.
    /// </summary>
    /// <returns>Task representing the test history obtaining.</returns>
    public async Task OnGetAsync()
    {
        this.TestResults = await dbContext.TestResults.OrderByDescending(t => t.TimeOfRun).ToListAsync();
    }
}
