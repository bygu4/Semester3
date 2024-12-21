// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnitWeb.Data;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// The test result database context.
/// </summary>
/// <param name="options">The options to be used by the database context.</param>
public class TestResultDbContext(DbContextOptions<TestResultDbContext> options)
    : DbContext(options)
{
    /// <summary>
    /// Gets the set of test results stored in the database.
    /// </summary>
    public DbSet<TestResultData> TestResults => this.Set<TestResultData>();
}
