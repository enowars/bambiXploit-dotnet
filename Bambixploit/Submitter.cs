namespace Bambixploit
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;

    public class Submitter
    {
        private const char DummyChar = 'A';
        private readonly Configuration config;
        private readonly Channel<string> inputChannel;
        private readonly ConcurrentDictionary<string, char> transitFlags = new();

        public Submitter(Configuration config)
        {
            this.config = config;
            this.inputChannel = Channel.CreateBounded<string>(new BoundedChannelOptions(100000)
            {
                SingleReader = true,
                SingleWriter = false,
            });
        }

        public void Start()
        {
            Task.Run(this.Run);
        }

        public async Task Enqueue(string flag)
        {
            await this.inputChannel.Writer.WriteAsync(flag);
        }

        private async void Run()
        {
            while (true)
            {
                try
                {
                    var conn = new TcpClient();
                    await conn.ConnectAsync(this.config.SubmissionAddress, this.config.SubmissionPort);
                    if (this.config.DebugSubmission)
                    {
                        await conn.Client.SendAsync(Encoding.ASCII.GetBytes("1\n"), SocketFlags.None, CancellationToken.None);
                    }

                    var responsesTask = Task.Run(async () => await this.HandleResponses(conn.GetStream()));

                    foreach ((var flag, var _) in this.transitFlags)
                    {
                        await conn.Client.SendAsync(Encoding.ASCII.GetBytes($"{flag}\n"), SocketFlags.None, CancellationToken.None);
                    }

                    while (true)
                    {
                        var flag = await this.inputChannel.Reader.ReadAsync();
                        this.transitFlags.TryAdd(flag, DummyChar);
                        await conn.Client.SendAsync(Encoding.ASCII.GetBytes($"{flag}\n"), SocketFlags.None, CancellationToken.None);
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"{e.Message}\n{e.StackTrace}");
                    await Task.Delay(1000);
                }
            }
        }

        private async Task HandleResponses(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);

            // Read until the end of the welcome banner (\n\n)
            string banner = string.Empty;
            bool firstLine = true;
            while (true)
            {
                var line = await reader.ReadLineAsync();
                banner += line + "\n";
                if (line == string.Empty && !firstLine)
                {
                    break;
                }

                firstLine = false;
            }

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (line != null)
                {
                    try
                    {
                        var split = line.Split(' ');
                        var flag = split[0];
                        var result = split[1];

                        this.transitFlags.TryRemove(flag, out var _);

                        if (this.config.DebugSubmission)
                        {
                            if (result.StartsWith("INV"))
                            {
                                Statistics.AddOkFlags(1);
                            }
                        }
                        else
                        {
                            if (result.StartsWith("OK"))
                            {
                                Statistics.AddOkFlags(1);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{e.Message}\n{e.StackTrace}");
                    }
                }
            }
        }
    }
}
