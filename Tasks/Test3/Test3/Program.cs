namespace Chat;

public static class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length != 1 && args.Length != 2)
        {
            Console.WriteLine("Bad args");
        }

        var port = int.Parse(args[0]);
        var address = args.Length == 2 ? args[1] : null;
        var chat = new Chat(port, address);
        await chat.Start();
    }
}
