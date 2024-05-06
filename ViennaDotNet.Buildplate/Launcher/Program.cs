using CommandLine;
using CommandLine.Text;
using Serilog;
using ViennaDotNet.DB;
using ViennaDotNet.EventBus.Client;
using ViennaDotNet.ObjectStore.Client;

namespace ViennaDotNet.Buildplate.Launcher
{
    internal static class Program
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        class Options
        {
            [Option("eventbus", Default = "localhost:5532", Required = false, HelpText = "Event bus address")]
            public string EventBusConnectionString { get; set; }
            [Option("publicAddress", Required = true, HelpText = "Public server address to report in instance info")]
            public string PublicAddress { get; set; }
            [Option("bridgeJar", Required = true, HelpText = "Fountain bridge JAR file")]
            public string BridgeJar { get; set; }
            [Option("serverTemplateDir", Required = true, HelpText = "Minecraft/Fabric server template directory, containing the Fabric JAR, mods, and libraries")]
            public string ServerTemplateDir { get; set; }
            [Option("fabricJarName", Required = true, HelpText = "Name of the Fabric JAR to run within the server template directory")]
            public string FabricJarName { get; set; }
            [Option("connectorPluginJar", Required = true, HelpText = "Fountain connector plugin JAR")]
            public string ConnectorPluginJar { get; set; }
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        static void Main(string[] args)
        {
            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/debug.txt", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 8338607, outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .MinimumLevel.Debug()
                .CreateLogger();

            Log.Logger = log;

            Console.CancelKeyPress += (sender, e) =>
            {
                Log.Information("Ctrl+C received, ignored");
                e.Cancel = true;
            };

            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
            {
                Log.Fatal($"Unhandeled exception: {e.ExceptionObject}");
                Log.CloseAndFlush();
                Environment.Exit(1);
            };

            ParserResult<Options> res = Parser.Default.ParseArguments<Options>(args);

            Options options;
            if (res is Parsed<Options> parsed)
                options = parsed.Value;
            else if (res is NotParsed<Options> notParsed)
            {
                if (res.Errors.Any(error => error is HelpRequestedError))
                    Environment.Exit(2);
                else if (res.Errors.Any(error => error is VersionRequestedError))
                    Environment.Exit(3);
                else
                    Environment.Exit(1);
                return;
            }
            else
                return;

            Log.Information("Connecting to event bus");
            EventBusClient eventBusClient;
            try
            {
                eventBusClient = EventBusClient.create(options.EventBusConnectionString);
            }
            catch (EventBusClientException ex)
            {
                Log.Fatal($"Could not connect to event bus: {ex}");
                Environment.Exit(1);
                return;
            }
            Log.Information("Connected to event bus");

            string javaCmd = JavaLocator.locateJava();
            Starter starter = new Starter(eventBusClient, options.EventBusConnectionString, options.PublicAddress, javaCmd, options.BridgeJar, options.ServerTemplateDir, options.FabricJarName, options.ConnectorPluginJar);
            PreviewGenerator previewGenerator = new PreviewGenerator(javaCmd, options.BridgeJar);
            InstanceManager instanceManager = new InstanceManager(eventBusClient, starter, previewGenerator);

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                // maybe works better like this?
                //new Thread(() =>
                //{
                instanceManager.shutdown();
                //});
            };

            while (true) Thread.Sleep(1000);
        }
    }
}
