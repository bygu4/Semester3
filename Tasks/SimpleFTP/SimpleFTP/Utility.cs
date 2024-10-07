// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace SimpleFTP;

/// <summary>
/// Class containing utility functions for Server and Client classes.
/// </summary>
public static class Utility
{
    /// <summary>
    /// Gets RequestType instance by it's representation.
    /// </summary>
    /// <param name="representation">Representation of RequestType's value.</param>
    /// <returns>Parsed RequestType instance.</returns>
    /// <exception cref="InvalidDataException">Failed to parse RequestType.</exception>
    public static RequestType GetRequestType(string representation)
    {
        var parsed = int.TryParse(representation, out var value);
        if (!parsed)
        {
            throw new InvalidDataException("Failed to parse request type");
        }

        return (RequestType)value;
    }

    /// <summary>
    /// Asynchronously read line from given stream.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <returns>Task containing data about reading's result.</returns>
    public static async Task<string?> ReadLineAsync(Stream stream)
    {
        using var reader = new StreamReader(stream);
        return await reader.ReadLineAsync();
    }

    /// <summary>
    /// Asynchronously write line to given stream.
    /// </summary>
    /// <param name="line">Line to write.</param>
    /// <param name="stream">Stream to write to.</param>
    /// <returns>Task containing data about writing's completion.</returns>
    public static async Task WriteLineAsync(string? line, Stream stream)
    {
        using var writer = new StreamWriter(stream);
        await writer.WriteLineAsync(line);
    }
}
