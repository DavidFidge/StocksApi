using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace StocksApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var minLogLevel = GetLogEventLevel();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(minLogLevel)
                //.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(Environment.GetEnvironmentVariable("STOCKAPI_SEQ_URL") ?? "http://localhost:5341")
                .CreateLogger();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application startup failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static LogEventLevel GetLogEventLevel()
        {
            var defaultLevel = LogEventLevel.Information;
            var logLevel = Environment.GetEnvironmentVariable("STOCKAPI_LOG_LEVEL");

            if (String.IsNullOrEmpty(logLevel))
                return defaultLevel;
            
            if (!Enum.TryParse(logLevel, out LogEventLevel level))
                level = defaultLevel;

            return level;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
