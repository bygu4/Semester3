// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace SimpleFTP;

/// <summary>
/// Enum of possible FTP server request types.
/// </summary>
public enum RequestType
{
    /// <summary>
    /// Undefined request type.
    /// </summary>
    None = 0,

    /// <summary>
    /// Request for listing all the files in a specified directory.
    /// </summary>
    List = 1,

    /// <summary>
    /// Request for downloading a specified file.
    /// </summary>
    Get = 2,
}
