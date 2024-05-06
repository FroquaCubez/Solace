using Serilog.Events;
using Serilog;
using CommandLine;

namespace ViennaDotNet.EventBus.Server
{
    internal static class Program
    {
        class Options
        {
            [Option("port", Default = 5532, Required = false, HelpText = "Port to listen on")]
            public int Port { get; set; }
        }
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
                server = new NetworkServer(new Server(), options.Port);
            }
            catch (IOException ex)
            {
                Log.Fatal(ex.ToString());
                Environment.Exit(1);
                return;
            }

            server.run();
        }
    }
}
