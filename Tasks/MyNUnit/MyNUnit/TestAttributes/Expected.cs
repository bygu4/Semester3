// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit;

/// <summary>
/// Attribute used for specifying the exception
/// expected to be thrown in the test.
/// </summary>
/// <typeparam name="T">Type of the exception expected.</typeparam>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class Expected<T> : Attribute
    where T : Exception
{
    /// <summary>
    /// Gets type of the expected exception.
    /// </summary>
    public Type ExpectedException { get; } = typeof(T);
}
