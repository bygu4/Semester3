// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Chat.Tests;

using System.Net.Sockets;

public class TestWriter(IList<string> linesToWrite)
    : Writer
{
    public new async Task StartWritingFromConsole(
        NetworkStream stream,
        CancellationToken token)
    {
        using (stream)
        {
            var writer = new StreamWriter(stream);
            foreach (var line in linesToWrite)
            {
                await writer.WriteLineAsync(line);
                await writer.FlushAsync();
            }
        }
    }
}
