// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Chat;

using System.Net.Sockets;

/// <summary>
/// Class for reading from console and writing to the given stream.
/// </summary>
public static class Writer
{
    /// <summary>
    /// Start reading from the console and writing to the stream.
    /// </summary>
    /// <param name="stream">Stream to write to.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The task representing the work of the writer.</returns>
    public static async Task StartWritingFromConsole(
        NetworkStream stream,
        CancellationToken token)
    {
        using (stream)
        {
            while (!token.IsCancellationRequested)
            {
                var line = Console.ReadLine();
                var writer = new StreamWriter(stream);
                await writer.WriteLineAsync(line);
                await writer.FlushAsync();
            }
        }
    }
}
