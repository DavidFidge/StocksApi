using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

using StocksApi.Core;

namespace StocksApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var minLogLevel = GetLogEventLevel();
            var entityFrameworkLogLevel = GetEntityFrameworkLogEventLevel();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(minLogLevel)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", entityFrameworkLogLevel)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(Environment.GetEnvironmentVariable(Constants.StocksApiSeqUrl) ?? "http://localhost:5341")
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
            var logLevel = Environment.GetEnvironmentVariable(Constants.StocksApiLogLevel);

            if (String.IsNullOrEmpty(logLevel))
                return defaultLevel;
            
            if (!Enum.TryParse(logLevel, out LogEventLevel level))
                level = defaultLevel;

            return level;
        }

        private static LogEventLevel GetEntityFrameworkLogEventLevel()
        {
            var defaultLevel = LogEventLevel.Warning;
            var logLevel = Environment.GetEnvironmentVariable(Constants.StocksApiEntityFrameworkLogLevel);

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
