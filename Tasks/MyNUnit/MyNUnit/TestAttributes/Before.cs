// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit;

/// <summary>
/// Attribute used for specifying a method
/// to run before each test in the class.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class Before : Attribute
{
}
