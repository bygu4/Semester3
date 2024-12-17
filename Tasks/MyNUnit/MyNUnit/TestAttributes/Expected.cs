// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit;

/// <summary>
/// Attribute used for specifying the exception
/// expected to be thrown in the test.
/// </summary>
/// <param name="exceptionType">Type of the exception expected.</param>
[AttributeUsage(AttributeTargets.Method)]
public class Expected(Type exceptionType)
    : Attribute
{
    /// <summary>
    /// Gets type of the expected exception.
    /// </summary>
    public Type ExceptionType { get; } = exceptionType;
}
