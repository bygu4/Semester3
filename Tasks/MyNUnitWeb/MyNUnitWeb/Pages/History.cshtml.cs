// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

#pragma warning disable SA1010 // Opening square brackets should be spaced correctly

namespace MyNUnitWeb.Pages;

using Microsoft.AspNetCore.Mvc.RazorPages;
using MyNUnitWeb.Data;

/// <summary>
/// The page model of the test run history.
/// </summary>
public class HistoryModel : PageModel
{
    /// <summary>
    /// Gets or sets the result of the test run.
    /// </summary>
    public IList<TestResult> TestResults { get; set; } = [];

    /// <summary>
    /// Displays the history of the test runs.
    /// </summary>
    public void OnGet()
    {
        // get all test results from db
    }
}
