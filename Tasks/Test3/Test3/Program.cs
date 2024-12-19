// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Chat;

/// <summary>
/// The console chat.
/// </summary>
public static class Program
{
    /// <summary>
    /// The main entry point of the program.
    /// Starts chat with given port and address.
    /// </summary>
    /// <param name="args">Port and an address to start server with.</param>
    /// <returns>The task representing the work of the chat.</returns>
    public static async Task Main(string[] args)
    {
        if (args.Length != 1 && args.Length != 2)
        {
            Console.WriteLine("Bad args");
        }

        var port = int.Parse(args[0]);
        var address = args.Length == 2 ? args[1] : null;
        var chat = new Chat(port, address);
        await chat.EstablishConnection();
    }
}
