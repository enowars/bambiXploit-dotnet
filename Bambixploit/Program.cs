﻿namespace Bambixploit
{
    using System;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Terminal.Gui;

    public enum ExploitTemplate
    {
        Http,
        Binary,
    }

    public class Program
    {
        private static readonly JsonSerializerOptions CamelCaseEnumConverterOptions = new()
        {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
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

        public static async Task<int> Main(string[] args)
        {
            var templateCommand = new Command("template", "template command description")
            {
                new Argument<ExploitTemplate>("templateType", "one of the supported templates"),
            };
            templateCommand.Handler = CommandHandler.Create<ExploitTemplate>(Template);

            var pwnCommand = new Command("pwn", "pwn command description")
            {
                new Argument<string>("command", "the exploit command to run")
                {
                    Arity = ArgumentArity.ExactlyOne,
                },
                new Argument<string[]>("arguments", "arguments for the exploit"),
            };
            pwnCommand.Handler = CommandHandler.Create<string, string[]>(Pwn);

            var rootCommand = new RootCommand();
            rootCommand.AddCommand(templateCommand);
            rootCommand.AddCommand(pwnCommand);

            return await rootCommand.InvokeAsync(args);
        }

        public static void Template(ExploitTemplate template)
        {
            ExploitTemplates.PrintTemplate(template);
        }

        public static void Pwn(string command, string[] arguments)
        {
            var argumentsString = string.Join(' ', arguments);
            Console.WriteLine($"Pwn {command} {argumentsString}");
            if (command.Length < 1)
            {
                Console.WriteLine($"Missing argument.");
                return;
            }

            // Parse json configuration
            JsonConfiguration jsonConfiguration;
            try
            {
                string content = File.ReadAllText("bambixploit.json");
                var deserialized = JsonSerializer.Deserialize<JsonConfiguration>(content, CamelCaseEnumConverterOptions);
                if (deserialized == null)
                {
                    throw new Exception("Deserialize returned null");
                }

                jsonConfiguration = deserialized;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Deserialization of config failed.\n{e.Message}\n{e.StackTrace}");
                return;
            }

            return;

            // Build service collection with everything we need
            var serviceProvider = new ServiceCollection()
                .AddSingleton(new Configuration(
                    new Regex(jsonConfiguration.FlagRegex, RegexOptions.Compiled),
                    command,
                    argumentsString,
                    jsonConfiguration.Interval,
                    jsonConfiguration.Addresses,
                    IPAddress.Parse(jsonConfiguration.SubmissionAddress),
                    jsonConfiguration.SubmissionPort,
                    jsonConfiguration.DebugSubmission,
                    Guid.NewGuid()))
                .AddSingleton<Program>()
                .AddSingleton<Submitter>()
                .AddSingleton<Exploiter>()
                .AddSingleton<Storage>()
                .BuildServiceProvider(validateScopes: true);

            // Go!
            var program = serviceProvider.GetRequiredService<Program>();
            program.RunUI();
        }

        private void RunUI()
        {
            try
            {
                Application.UseSystemConsole = true;
                Application.Init();

                var top = Application.Top;
                var tabView = new TabView()
                {
                    X = 0,
                    Y = 0,
                    Width = Dim.Fill(),
                    Height = Dim.Fill(),
                };

                var statusBar = new StatusBar(new StatusItem[]
                {
                    new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", () => Application.RequestStop()),
                });
                tabView.AddTab(new TabView.Tab("Submissions", new SubmissionsGraph()), true);
                tabView.AddTab(new TabView.Tab("Logs", new LogsView(this.storage)), false);
                tabView.AddTab(new TabView.Tab("Debug", new DebugView(this.configuration)), false);

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
