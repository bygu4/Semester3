// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit;

/// <summary>
/// Attribute used for specifying a test method.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class Test : Attribute
{
}
