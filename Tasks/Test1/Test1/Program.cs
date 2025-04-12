// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace CheckSum;

using System.Diagnostics;

/// <summary>
/// Program that compares the time of sequential and
/// concurrent check sum evaluation variations.
/// </summary>
public static class Program
{
    /// <summary>
    /// Main entry point of the program.
    /// </summary>
    /// <param name="args">First argument is the path of
    /// the file to evaluate the check sum of.</param>
    public static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("Invalid arguments.\nExpected: Path of the file");
        }

        var path = args[0];
        var stopWatch = new Stopwatch();

        try
        {
            stopWatch.Restart();
            CheckSumUtils.GetCheckSumSequentially(path);
            stopWatch.Stop();
            Console.WriteLine($"Time of the sequential evaluation: {stopWatch.Elapsed}");

            stopWatch.Restart();
            CheckSumUtils.GetCheckSumConcurrently(path).Wait();
            stopWatch.Stop();
            Console.WriteLine($"Time of the concurrent evaluation: {stopWatch.Elapsed}");
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
    }
}
