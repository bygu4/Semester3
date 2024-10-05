// Copyright (c) 2024
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
/// </remarks>
/// <param name="port">Port to listen for connections on.</param>
public class Server(int port)
{
    private readonly TcpListener tcpListener = new (IPAddress.Any, port);
    private bool running = false;

    /// <summary>
    /// Gets port from which the server can be accessed.
    /// </summary>
    public int Port { get; private set; } = port;

    /// <summary>
    /// Enables the server to listen for connections.
    /// </summary>
    public void Start()
    {
        this.running = true;
        this.tcpListener.Start();
        while (this.running)
        {
            this.AcceptConnection();
        }
    }

    /// <summary>
    /// Stops the server from listening for connections.
    /// </summary>
    public void Stop()
    {
        this.running = false;
        this.tcpListener.Stop();
    }

    private static async Task ProcessRequest(Socket socket)
    {
        using var stream = new NetworkStream(socket);
        var command = await ReadLineAsync(stream);
        if (command == null)
        {
            return;
        }

        var response = await GetRequestResponse(command);
        await WriteLineAsync(response, stream);
    }

    private static async Task<string> GetRequestResponse(string command)
    {
        var elements = command.Split(' ');
        if (elements.Length != 2)
        {
            throw new InvalidDataException("Invalid request format");
        }

        var (requestTypeRepresentation, path) = (elements[0], elements[1]);
        var requestType = GetRequestType(requestTypeRepresentation);

        return requestType switch
        {
            RequestType.List => GetListCommandResponse(path),
            RequestType.Get => await GetGetCommandResponse(path),
            _ => throw new InvalidEnumArgumentException("Unknown request type"),
        };
    }

    private static string GetListCommandResponse(string path)
    {
        if (!Directory.Exists(path))
        {
            return "-1";
        }

        var files = Directory.GetFiles(path);
        var response = files.Length.ToString();
        foreach (var file in files)
        {
            var isDirectory = Directory.Exists(file);
            response += $" ({file} {isDirectory})";
        }

        return response;
    }

    private static async Task<string> GetGetCommandResponse(string path)
    {
        if (!File.Exists(path))
        {
            return "-1";
        }

        var content = await File.ReadAllBytesAsync(path);
        var size = content.LongLength;
        return $"{size} {content}";
    }

    private static RequestType GetRequestType(string representation)
    {
        var parsed = int.TryParse(representation, out var value);
        if (!parsed)
        {
            throw new InvalidDataException("Invalid request format");
        }

        return (RequestType)value;
    }

    private static async Task<string?> ReadLineAsync(Stream stream)
    {
        using var reader = new StreamReader(stream);
        return await reader.ReadLineAsync();
    }

    private static async Task WriteLineAsync(string? line, Stream stream)
    {
        using var writer = new StreamWriter(stream);
        await writer.WriteLineAsync(line);
    }

    private async void AcceptConnection()
    {
        using var socket = await this.tcpListener.AcceptSocketAsync();
        await ProcessRequest(socket);
        socket.Close();
    }
}
