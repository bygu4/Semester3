// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit;

/// <summary>
/// Static class containing assert utility.
/// </summary>
public static class MyAssert
{
    /// <summary>
    /// Assert that the given condition is true.
    /// </summary>
    /// <param name="condition">The evaluated condition.</param>
    public static void That(bool condition)
    {
        if (!condition)
        {
            throw new MyAssertException("Evaluated condition was false.");
        }
    }
}
