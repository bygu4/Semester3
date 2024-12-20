// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnitWeb.Pages;

using Microsoft.AspNetCore.Mvc.RazorPages;
using MyNUnitWeb.Data;

/// <summary>
/// The page model of a result of the test run.
/// </summary>
public class TestResultModel : PageModel
{
    /// <summary>
    /// Gets or sets the result of the test run.
    /// </summary>
    public TestResult? Result { get; set; }

    /// <summary>
    /// Displays the test result by its id in the database.
    /// </summary>
    /// <param name="id">The id of the test run in the database.</param>
    public void OnGet(int id)
    {
        // get test result from the db.
    }
}
