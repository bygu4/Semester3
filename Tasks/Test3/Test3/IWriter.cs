// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Chat;

using System.Net.Sockets;

/// <summary>
/// Interface representing the writer for chat.
/// </summary>
public interface IWriter
{
    /// <summary>
    /// Start writing to the given stream.
    /// </summary>
    /// <param name="stream">Stream to write to.</param>
    /// <param name="token">The cancellation token.</param>
    /// <param name="stopAction">The action to stop the chat if "exit" is written.</param>
    /// <returns>The task representing the work of the writer.</returns>
    public Task StartWritingToStream(
        NetworkStream stream,
        CancellationToken token,
        Action stopAction);
}
