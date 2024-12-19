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

    [SetUp]
    public void SetupChat()
    {
        this.firstChat = new Chat(Port);
        this.secondChat = new Chat(Port, Address);
    }

    [TearDown]
    public async Task StopChat()
    {
        await this.firstChat.Stop();
        await this.secondChat.Stop();
    }

    [TestCaseSource(nameof(LinesToWriteAndRead))]
    public async Task Test_WriteAndReadMessages_MessagesAreWritterAndReadCorrectly(
        string[] linesToReadAndWrite)
    {
        await this.firstChat.EstablishConnection(
            new TestReader(linesToReadAndWrite),
            new Writer());
        await this.secondChat.EstablishConnection(
            new Reader(),
            new TestWriter(linesToReadAndWrite));
    }

    private static List<string[]> LinesToWriteAndRead() =>
    [
        ["fasfsa", "asffsa", "", "11", "22", "exit"],
        ["asfasf", "as", "", "", "exit"],
        ["exit"],
        ["qqqqqqqqqqwwwwwwwwwww", "123456789", "exit"],
    ];
}
