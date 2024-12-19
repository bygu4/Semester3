
namespace Chat;

using System.Net.Sockets;

public static class Writer
{
    public static async Task StartWritingFromConsole(
        NetworkStream stream,
        CancellationToken token)
    {
        using (stream)
        {
            while (!token.IsCancellationRequested)
            {
                var line = Console.ReadLine();
                var writer = new StreamWriter(stream);
                await writer.WriteLineAsync(line);
                await writer.FlushAsync();
            }
        }
    }
}
