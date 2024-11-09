// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit;

/// <summary>
/// Exception representing a fail of the assert.
/// </summary>
/// <param name="message">The message of the exception.</param>
public class MyAssertException(string message)
    : SystemException(message)
{
}
