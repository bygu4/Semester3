// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Chat.Tests;

using System.Net.Sockets;

public class TestWriter(IList<string> linesToWrite)
    : IWriter
{
    public async Task StartWritingToStream(
        NetworkStream stream,
        CancellationToken token,
        Action stopAction)
    {
        using (stream)
        {
            while (!token.IsCancellationRequested)
            {
                var writer = new StreamWriter(stream);
                foreach (var line in linesToWrite)
                {
                    await writer.WriteLineAsync(line);
                    await writer.FlushAsync();
                }

                stopAction();
            }
        }
    }
}
