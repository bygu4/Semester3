
namespace Chat;

using System.Net.Sockets;

public static class Reader
{
    public static Task StartReadingFromStream(NetworkStream stream, CancellationToken token)
        => Task.Run(() => ReadFromStreamAndWriteToConsole(stream, token));

    private static async Task ReadFromStreamAndWriteToConsole(
        NetworkStream stream, CancellationToken token)
    {
        using (stream)
        {
            while (!token.IsCancellationRequested)
            {
                var reader = new StreamReader(stream);
                var line = await reader.ReadToEndAsync();
                Console.WriteLine(line);
            }
        }
    }
}
