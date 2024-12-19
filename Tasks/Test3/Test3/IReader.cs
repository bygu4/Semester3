// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Chat;

using System.Net.Sockets;

/// <summary>
/// Interface representing the reader for chat.
/// </summary>
public interface IReader
{
    /// <summary>
    /// Start reading from the given stream.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="token">The cancellation token.</param>
    /// <param name="stopAction">The action to stop the chat if "exit" is read.</param>
    /// <returns>The task representing the work of the reader.</returns>
    public Task StartReadingFromStream(
        NetworkStream stream,
        CancellationToken token,
        Action stopAction);
}
