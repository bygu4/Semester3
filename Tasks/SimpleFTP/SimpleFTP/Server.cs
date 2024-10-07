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
/// <param name="port">Port to listen for connections on.</param>
public class Server(int port)
{
    private readonly TcpListener tcpListener = new (IPAddress.Any, port);

    private Task? serverTask = null;
    private bool running = false;

    /// <summary>
    /// Gets port from which the server can be accessed.
    /// </summary>
    public int Port { get; } = port;

    /// <summary>
    /// Enables the server to listen for connections.
    /// </summary>
    public void Start()
    {
        this.running = true;
        this.tcpListener.Start();
        this.serverTask = new Task(this.ListenForConnections);
    }

    /// <summary>
    /// Stops the server from listening for connections.
    /// </summary>
    public void Stop()
    {
        this.running = false;
        this.serverTask?.Wait();
        this.tcpListener.Stop();
    }

    private static async Task ProcessRequest(Socket socket)
    {
        try
        {
            using var stream = new NetworkStream(socket);
            var request = await Utility.ReadLineAsync(stream);
            if (request == null)
            {
                return;
            }

            var response = await GetRequestResponse(request);
            await Utility.WriteLineAsync(response, stream);
        }
        finally
        {
            socket.Close();
        }
    }

    private static async Task<string> GetRequestResponse(string request)
    {
        var elements = request.Split(' ');
        if (elements.Length != 2)
        {
            throw new InvalidDataException("Invalid request format");
        }

        var (requestTypeRepresentation, path) = (elements[0], elements[1]);
        var requestType = Utility.GetRequestType(requestTypeRepresentation);

        return requestType switch
        {
            RequestType.List => GetListRequestResponse(path),
            RequestType.Get => await GetGetRequestResponse(path),
            _ => throw new InvalidEnumArgumentException("Unknown request type"),
        };
    }

    private static string GetListRequestResponse(string path)
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
            response += $" {file} {isDirectory}";
        }

        return response;
    }

    private static async Task<string> GetGetRequestResponse(string path)
    {
        if (!File.Exists(path))
        {
            return "-1";
        }

        var content = await File.ReadAllBytesAsync(path);
        var size = content.LongLength;
        return $"{size} {Convert.ToBase64String(content)}";
    }

    private void ListenForConnections()
    {
        var connections = new List<Task>();
        while (this.running)
        {
            connections.Add(this.AcceptConnection());
        }

        foreach (var connection in connections)
        {
            connection.Wait();
        }
    }

    private Task AcceptConnection()
    {
        var socket = this.tcpListener.AcceptSocket();
        return ProcessRequest(socket);
    }
}
