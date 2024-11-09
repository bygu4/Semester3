// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit;

/// <summary>
/// Attribute used for indicating to ignore the test.
/// </summary>
/// <param name="reason">The reason to ignore the test.</param>
[AttributeUsage(AttributeTargets.Method)]
public class Ignore(string reason)
    : Attribute
{
    /// <summary>
    /// Gets the reason to ignore the test.
    /// </summary>
    public string Reason { get; } = reason;
}
