// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Chat;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Class representing a chat.
/// </summary>
/// <param name="port">Port to establish connection on.</param>
/// <param name="address">Address to start a server on.</param>
public class Chat(int port, string? address = null)
{
    private TcpListener? listener;
    private Task? reader;
    private Task? writer;
    private CancellationTokenSource cancellation = new ();

    /// <summary>
    /// Establish connection until it is closed from console.
    /// </summary>
    /// <param name="reader">Reader instance to use.</param>
    /// <param name="writer">Writer instance to use.</param>
    /// <returns>A task representing the connection.</returns>
    public async Task EstablishConnection(IReader reader, IWriter writer)
    {
        await this.Start(reader, writer);
        await this.WaitForChatToClose();
        this.listener?.Stop();
    }

    /// <summary>
    /// Start the server if possible and start or accept the client.
    /// </summary>
    /// <param name="reader">Reader instance to use.</param>
    /// <param name="writer">Writer instance to use.</param>
    /// <returns>The task representing the established connection.</returns>
    public async Task Start(IReader reader, IWriter writer)
    {
        this.cancellation = new CancellationTokenSource();
        TcpClient client;
        if (address is null)
        {
            this.listener = new TcpListener(IPAddress.Any, port);
            this.listener.Start();
            client = await this.listener.AcceptTcpClientAsync();
        }
        else
        {
            client = new TcpClient(address, port);
        }

        var stream = client.GetStream();
        this.reader = reader.StartReadingFromStream(
            stream,
            this.cancellation.Token,
            () => this.cancellation.Cancel());
        this.writer = writer.StartWritingToStream(
            stream,
            this.cancellation.Token,
            () => this.cancellation.Cancel());
    }

    /// <summary>
    /// Stop all the connections.
    /// </summary>
    /// <returns>The task representing the stop of the chat.</returns>
    public async Task Stop()
    {
        this.cancellation.Cancel();
        this.listener?.Stop();
        await this.WaitForChatToClose();
    }

    private async Task WaitForChatToClose()
    {
        if (this.reader is not null)
        {
            await this.reader;
        }

        if (this.writer is not null)
        {
            await this.writer;
        }
    }
}
