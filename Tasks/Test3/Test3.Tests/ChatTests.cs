// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Chat.Tests;

/// <summary>
/// Tests for the Chat class.
/// </summary>
public class ChatTests
{
    private const int Port = 42000;
    private const string Address = "localhost";

    private Chat firstChat = new (Port);
    private Chat secondChat = new (Port);

    /// <summary>
    /// Sets up two chat instances before the test.
    /// </summary>
    [SetUp]
    public void SetupChat()
    {
        this.firstChat = new Chat(Port);
        this.secondChat = new Chat(Port, Address);
    }

    /// <summary>
    /// Stops the chat after the test.
    /// </summary>
    [TearDown]
    public void StopChat()
    {
        this.firstChat.Dispose();
        this.secondChat.Dispose();
    }

    /// <summary>
    /// Tests the chat by writing and reading some messages.
    /// </summary>
    /// <param name="linesToReadAndWrite">Lines to write.</param>
    /// <returns>Task representing the test.</returns>
    [TestCaseSource(nameof(LinesToWriteAndRead))]
    public async Task Test_WriteAndReadMessages_MessagesAreWrittenAndReadCorrectly(
        string[] linesToReadAndWrite)
    {
        var firstTask = this.firstChat.EstablishConnection(
            new TestReader(linesToReadAndWrite),
            new TestWriter([]));
        var secondTask = this.secondChat.EstablishConnection(
            new TestReader([]),
            new TestWriter(linesToReadAndWrite));
        await firstTask;
        await secondTask;
    }

    private static List<string[]> LinesToWriteAndRead() =>
    [
        ["fasfsa", "asffsa", "-1", "11", "22"],
        ["asfasf", "as", "asggsgsagsasgagsa"],
        ["2345566"],
        [string.Empty],
        ["qqqqqqqqqqwwwwwwwwwww", "123456789", "matmex He Dlya Bcex"],
    ];
}
