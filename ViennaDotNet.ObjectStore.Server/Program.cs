using CommandLine;
using Serilog;

namespace ViennaDotNet.ObjectStore.Server
{
    internal static class Program
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        class Options
        {
            [Option("dataDir", Default = "data", Required = false, HelpText = "Directory where data is stored")]
            public string DataDir { get; set; }

            [Option("port", Default = 5396, Required = false, HelpText = "Port to listen on")]
            public int Port { get; set; }
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

            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
            {
                Log.Fatal($"Unhandeled exception: {e.ExceptionObject}");
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

            NetworkServer server;
            try
            {
                server = new NetworkServer(new Server(new DataStore(new DirectoryInfo(options.DataDir))), options.Port);
            }
            catch (Exception ex) when (
                ex is IOException
                || ex is DataStore.DataStoreException
            )
            {
                Log.Fatal(ex.ToString());
                Environment.Exit(1);
                return;
            }

            server.run();
        }
    }
}
