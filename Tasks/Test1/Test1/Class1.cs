// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Security.Cryptography;
using System.Text;

namespace Test1;

/// <summary>
/// Class for evaluating file chack sum.
/// </summary>
public static class CheckSumEvaluator
{
    /// <summary>
    /// Evaluate the check sum of given file sequentially.
    /// </summary>
    /// <param name="path">Path of the file to get check sum of.</param>
    /// <returns>An array of bytes: evaluated check sum.</returns>
    /// <exception cref="FileNotFoundException">File was not found.</exception>
    public static byte[] GetCheckSumSequentially(string path)
    {
        if (File.Exists(path))
        {
            return GetFileCheckSumSequentially(path);
        }

        if (Directory.Exists(path))
        {
            return GetDirectoryCheckSumSequentially(path);
        }

        throw new FileNotFoundException();
    }

    /// <summary>
    /// Evaluate the check sum of given file concurrently.
    /// </summary>
    /// <param name="path">Path of the file to get check sum of.</param>
    /// <returns>A task with result as an array of bytes: evaluated check sum.</returns>
    /// <exception cref="FileNotFoundException">File was not found.</exception>
    public static async Task<byte[]> GetCheckSumConcurrently(string path)
    {
        if (File.Exists(path))
        {
            return await GetFileCheckSumConcurrently(path);
        }

        if (Directory.Exists(path))
        {
            return GetDirectoryCheckSumConcurrently(path);
        }

        throw new FileNotFoundException();
    }

    private static byte[] GetFileCheckSumSequentially(string path)
    {
        var content = File.ReadAllBytes(path);
        return MD5.HashData(content);
    }

    private static byte[] GetDirectoryCheckSumSequentially(string path)
    {
        var result = new List<byte>();
        var name = Path.GetFileName(path);
        var nameHash = MD5.HashData(Encoding.Unicode.GetBytes(name));
        result.AddRange(nameHash);
        foreach (var entry in Directory.GetFileSystemEntries(path))
        {
            result.AddRange(GetCheckSumSequentially(entry));
        }

        return result.ToArray();
    }

    private static async Task<byte[]> GetFileCheckSumConcurrently(string path)
    {
        var content = await File.ReadAllBytesAsync(path);
        return MD5.HashData(content);
    }

    private static byte[] GetDirectoryCheckSumConcurrently(string path)
    {
        var tasks = new List<Task<byte[]>>();
        var name = Path.GetFileName(path);
        tasks.Add(Task.Run(() => MD5.HashData(Encoding.Unicode.GetBytes(name))));
        foreach (var entry in Directory.GetFileSystemEntries(path))
        {
            tasks.Add(GetCheckSumConcurrently(entry));
        }

        int size = 0;
        foreach (var task in tasks)
        {
            size += task.Result.Length;
        }

        var result = new byte[size];
        for (int taskI = 0; taskI < tasks.Count; ++taskI)
        {
            var task = tasks[taskI];
            for (int i = 0; i < task.Result.Length; ++i)
            {
                result[i] = task.Result[i];
            }
        }

        return result;
    }
}
