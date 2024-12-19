// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Chat.Tests;

using System.Net.Sockets;

public class TestReader(IList<string> linesToRead)
    : IReader
{
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
                foreach (var expectedLine in linesToRead)
                {
                    var actualLine = await reader.ReadLineAsync();
                    Assert.That(actualLine, Is.EqualTo(expectedLine));
                }

                stopAction();
            }
        }
    }
}
