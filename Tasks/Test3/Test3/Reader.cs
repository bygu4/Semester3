
namespace Chat;

using System.Net.Sockets;

public static class Reader
{
    public static async Task StartReadingFromStream(
        NetworkStream stream,
        CancellationToken token,
        Action stopAction)
    {
        using (stream)
        {
            while (!token.IsCancellationRequested)
            {
                var reader = new StreamReader(stream);
                var line = await reader.ReadToEndAsync();
                Console.WriteLine(line);
                if (line == "exit")
                {
                    stopAction();
                }
            }
        }
    }
}
