// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Chat;

using System.Net.Sockets;

/// <summary>
/// Class for reading the given stream and writing to the console.
/// </summary>
public class Reader : IReader
{
    /// <summary>
    /// Start reading from stream and writing to the console.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="token">The cancellation token.</param>
    /// <param name="stopAction">The action to stop the chat if "exit" is read.</param>
    /// <returns>The task representing the work of the reader.</returns>
    public async Task StartReadingFromStream(
        NetworkStream stream,
        CancellationToken token,
        Action stopAction)
    {
        using (stream)
        {
            while (!token.IsCancellationRequested)
            {
                var reader = new StreamReader(stream);
                var line = await reader.ReadLineAsync(token);
                Console.WriteLine(line);
                if (line == "exit")
                {
                    stopAction();
                }
            }
        }
    }
}
