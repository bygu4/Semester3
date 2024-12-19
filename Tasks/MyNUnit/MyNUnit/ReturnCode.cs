// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit.Core;

/// <summary>
/// Return code of MyNUnit console app.
/// </summary>
public enum ReturnCode : int
{
    /// <summary>
    /// Value indicating that all run tests were successful.
    /// </summary>
    AllTestsPassed = 0,

    /// <summary>
    /// Value indicating that not all run tests were successful.
    /// </summary>
    SomeTestsFailed = 1,

    /// <summary>
    /// Value indicating that given arguments were incorrect.
    /// </summary>
    InvalidArguments = 2,

    /// <summary>
    /// Value indicating that target directory was not found.
    /// </summary>
    DirectoryNotFound = 3,
}
