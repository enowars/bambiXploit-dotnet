using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui;

namespace bambixploit
{
    public class Program
    {
        private static readonly JsonSerializerOptions CamelCaseEnumConverterOptions = new()
        {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly Configuration configuration;
        private readonly Storage storage;

        public Program(Exploiter exploiter, Submitter submitter, Storage storage, Configuration configuration)
        {
            this.configuration = configuration;
            this.storage = storage;
            exploiter.Start();
            submitter.Start();
        }

        public static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Missing argument.");
                return 1;
            }

            string command = args[0];
            string arguments = string.Join(' ', args.Skip(1));

            // Parse json configuration
            JsonConfiguration jsonConfiguration;
            try
            {
                string content = File.ReadAllText("bambixploit.json");
                var deserialized =
                    JsonSerializer.Deserialize<JsonConfiguration>(content, CamelCaseEnumConverterOptions);
                if (deserialized == null) throw new Exception("Deserialize returned null");

                jsonConfiguration = deserialized;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Deserialization of config failed.\n{e.Message}\n{e.StackTrace}");
                return 1;
            }

            // Build service collection with everything we need
            var serviceProvider = new ServiceCollection()
                .AddSingleton(new Configuration(
                    new Regex(jsonConfiguration.FlagRegex, RegexOptions.Compiled),
                    command,
                    arguments,
                    jsonConfiguration.Interval,
                    jsonConfiguration.Addresses,
                    IPAddress.Parse(jsonConfiguration.SubmissionAddress),
                    jsonConfiguration.SubmissionPort,
                    jsonConfiguration.DebugSubmission,
                    Guid.NewGuid()
                ))
                .AddSingleton<Program>()
                .AddSingleton<Submitter>()
                .AddSingleton<Exploiter>()
                .AddSingleton<Storage>()
                .BuildServiceProvider(true);

            // Go!
            var program = serviceProvider.GetRequiredService<Program>();
            program.RunUI();
            return 0;
        }

        private void RunUI()
        {
            try
            {
                Application.UseSystemConsole = true;
                Application.Init();

                var top = Application.Top;
                var tabView = new TabView
                {
                    X = 0,
                    Y = 0,
                    Width = Dim.Fill(),
                    Height = Dim.Fill()
                };

                var statusBar = new StatusBar(new[]
                {
                    new(Key.CtrlMask | Key.Q, "~^Q~ Quit", () => Application.RequestStop())
                });
                tabView.AddTab(new TabView.Tab("Submissions", new SubmissionsGraph()), true);
                tabView.AddTab(new TabView.Tab("Logs", new LogsView(storage)), false);
                tabView.AddTab(new TabView.Tab("Debug", new DebugView(configuration)), false);

                top.Add(tabView);
                top.Add(statusBar);
                Application.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Task.Delay(5000).Wait();
            }
            finally
            {
                Application.Shutdown();
            }
        }
    }
}