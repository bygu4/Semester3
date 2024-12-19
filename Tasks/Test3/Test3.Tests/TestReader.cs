// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Chat.Tests;

using System.Net.Sockets;

/// <summary>
/// Chat reader for the test.
/// </summary>
/// <param name="linesToRead">Lines expected to be read.</param>
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
