
namespace Chat;

using System.Net;
using System.Net.Sockets;

public class Chat(int port, string? address)
{
    private TcpListener? listener;
    private Task? reader;
    private Task? writer;
    private CancellationTokenSource cancellation = new ();

    public async Task EstablishConnection()
    {
        await this.Start();
        await this.WaitForChatToClose();
        this.listener?.Stop();
    }

    public async Task Start()
    {
        this.cancellation = new CancellationTokenSource();
        TcpClient client;
        if (address is null)
        {
            this.listener = new TcpListener(IPAddress.Any, port);
            client = await this.listener.AcceptTcpClientAsync();
        }
        else
        {
            client = new TcpClient(address, port);
        }

        var stream = client.GetStream();
        this.reader = Reader.StartReadingFromStream(
            stream,
            this.cancellation.Token,
            async () => await this.Stop());
        this.writer = Writer.StartWritingFromConsole(
            stream,
            this.cancellation.Token);
    }

    public async Task Stop()
    {
        this.cancellation.Cancel();
        this.listener?.Stop();
        await this.WaitForChatToClose();
    }

    private async Task WaitForChatToClose()
    {
        if (this.reader is not null)
        {
            await this.reader;
        }
        
        if (this.writer is not null)
        {
            await this.writer;
        }
    }
}
