// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

#pragma warning disable SA1011 // Closing square brackets should be spaced correctly

namespace SimpleFTP;

using System.Net.Sockets;

/// <summary>
/// Simple client for making file transfer requests.
/// </summary>
/// <param name="hostName">Domain name of host to connect to.</param>
/// <param name="port">Port to  connect to.</param>
public class Client(string hostName, int port)
{
    /// <summary>
    /// Gets domain name to which client can connect.
    /// </summary>
    public string HostName { get; } = hostName;

    /// <summary>
    /// Gets port which client can access.
    /// </summary>
    public int Port { get; } = port;

    /// <summary>
    /// Gets a list of files that are stored in specified directory on the server.
    /// </summary>
    /// <param name="path">Path of the directory to list files from.</param>
    /// <returns>An array of (path, isDirectory) tuples,
    /// or null if directory was not found.</returns>
    public async Task<(string, bool)[]?> List(string path)
    {
        using var client = new TcpClient(this.HostName, this.Port);
        using var stream = client.GetStream();
        await MakeListRequest(path, stream);
        return await ReceiveListResponse(stream);
    }

    /// <summary>
    /// Downloads specified file from the server.
    /// </summary>
    /// <param name="path">Path of the file to download.</param>
    /// <returns>Bytes of the specified file,
    /// of null if file was not found.</returns>
    public async Task<byte[]?> Get(string path)
    {
        using var client = new TcpClient(this.HostName, this.Port);
        using var stream = client.GetStream();
        await MakeGetRequest(path, stream);
        return ReceiveGetResponse(stream);
    }

    private static async Task MakeListRequest(string path, Stream stream)
        => await Utility.WriteLineAsync(
            $"{(int)RequestType.List} {path}", stream);

    private static async Task MakeGetRequest(string path, Stream stream)
        => await Utility.WriteLineAsync(
            $"{(int)RequestType.Get} {path}", stream);

    private static async Task<(string, bool)[]?> ReceiveListResponse(Stream stream)
    {
        var response = await ReadListResponse(stream);
        var elements = response.Split(' ');
        var directorySize = int.Parse(elements[0]);
        if (directorySize == -1)
        {
            return null;
        }

        var files = new (string, bool)[directorySize];
        for (int i = 0; i < directorySize; ++i)
        {
            var path = elements[1 + (2 * i)];
            var isDirectory = bool.Parse(elements[2 + (2 * i)]);
            files[i] = (path, isDirectory);
        }

        return files;
    }

    private static byte[]? ReceiveGetResponse(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var fileSize = reader.ReadInt64();
        if (fileSize == -1)
        {
            return null;
        }

        var content = new byte[fileSize];
        for (int i = 0; i < fileSize; ++i)
        {
            content[i] = reader.ReadByte();
        }

        return content;
    }

    private static async Task<string> ReadListResponse(Stream stream)
    {
        var response = await Utility.ReadLineAsync(stream);
        if (response == null)
        {
            throw new ArgumentNullException(nameof(response));
        }

        return response;
    }
}
