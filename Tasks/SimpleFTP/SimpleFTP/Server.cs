// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.ComponentModel;
using System.Net;
using System.Net.Sockets;

namespace SimpleFTP;

/// <summary>
/// Simple server for processing file transfer requests.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Server"/> class.
/// Creates a server with specified local IP address and port.
/// </remarks>
/// <param name="localAddress">Local IP address to use.</param>
/// <param name="port">Port to listen for connections on.</param>
public class Server(IPAddress localAddress, int port)
    : IDisposable
{
    private readonly TcpListener tcpListener = new (localAddress, port);

    private CancellationTokenSource tokenSource = new ();
    private Task? serverTask = null;

    /// <summary>
    /// Gets local IP address used by the server.
    /// </summary>
    public IPAddress LocalAddress { get; } = localAddress;

    /// <summary>
    /// Gets port from which the server can be accessed.
    /// </summary>
    public int Port { get; } = port;

    /// <summary>
    /// Enables the server to listen for connections.
    /// </summary>
    public void Start()
    {
        this.tokenSource = new CancellationTokenSource();
        this.tcpListener.Start();
        this.serverTask = this.ListenForConnections();
    }

    /// <summary>
    /// Stops the server from listening for connections.
    /// </summary>
    /// <returns>The task representing the server stop.</returns>
    public async Task Stop()
    {
        this.tokenSource.Cancel();
        this.tcpListener.Stop();
        if (this.serverTask is not null)
        {
            await this.serverTask;
        }
    }

    /// <summary>
    /// Stops the server and releases all used resources.
    /// </summary>
    public void Dispose() => this.Stop().Wait();

    private static async Task ProcessRequest(Socket socket)
    {
        using (socket)
        {
            using var stream = new NetworkStream(socket);
            var request = await Utility.ReadLineAsync(stream);
            if (request == null)
            {
                return;
            }

            await RespondToRequest(request, stream);
        }
    }

    private static async Task RespondToRequest(string request, Stream stream)
    {
        var elements = request.Split(' ');
        if (elements.Length != 2)
        {
            return;
        }

        var (requestTypeRepresentation, path) = (elements[0], elements[1]);
        var requestType = Utility.GetRequestType(requestTypeRepresentation);

        switch (requestType)
        {
            case RequestType.List:
                await RespondToListRequest(path, stream);
                return;
            case RequestType.Get:
                RespondToGetRequest(path, stream);
                return;
            case RequestType.None:
                return;
            default:
                throw new InvalidEnumArgumentException("Unknown request type");
        }
    }

    private static async Task RespondToListRequest(string path, Stream stream)
    {
        using var writer = new StreamWriter(stream);
        if (!Directory.Exists(path))
        {
            await writer.WriteLineAsync("-1");
            return;
        }

        var files = Directory.GetFileSystemEntries(path);
        await writer.WriteAsync($"{files.Length}");
        foreach (var file in files)
        {
            var isDirectory = Directory.Exists(file);
            var pathOfTheFile = Utility.GetUniversalPath(file);
            await writer.WriteAsync($" {pathOfTheFile} {isDirectory}");
        }

        await writer.WriteAsync('\n');
    }

    private static void RespondToGetRequest(string path, Stream stream)
    {
        using var writer = new BinaryWriter(stream);
        if (!File.Exists(path))
        {
            writer.Write(-1L);
            return;
        }

        using var fileStream = File.OpenRead(path);
        writer.Write(fileStream.Length);
        for (int i = 0; i < fileStream.Length; ++i)
        {
            writer.Write((byte)fileStream.ReadByte());
        }
    }

    private async Task ListenForConnections()
    {
        var connections = new List<Task>();
        while (!this.tokenSource.IsCancellationRequested)
        {
            try
            {
                var socket = await this.tcpListener.AcceptSocketAsync(
                    this.tokenSource.Token);
                connections.Add(ProcessRequest(socket));
            }
            catch (OperationCanceledException)
            {
                continue;
            }
        }

        foreach (var connection in connections)
        {
            await connection;
        }
    }
}
