// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnitWeb.Data;

using MyNUnit.Core;

/// <summary>
/// Class representing the result of the test run.
/// </summary>
public class TestResultData
{
    /// <summary>
    /// Gets or sets the id of the test run.
    /// </summary>
    public int TestResultId { get; set; }

    /// <summary>
    /// Gets or sets the time of the test run.
    /// </summary>
    public DateTime TimeOfRun { get; set; }

    /// <summary>
    /// Gets or sets the summary of the test run.
    /// </summary>
    public TestSummary Summary { get; set; } = new ();
}
