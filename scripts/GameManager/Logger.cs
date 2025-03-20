
namespace GameNamespace.GameManager
{
    using Serilog;
    using System;
    using System.IO;


    public static class Logger
    {
        public static void Init()
        {
            StartUp();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("App", "TowerDefense")
                .WriteTo.File(
                    "Logs/log.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();
        }
        public static void StartUp()
        {
            // Clear out old logs. Eventually we may want to preserve a certain number of logs, but for now clear it
            // out on every new game run.
            if(!Directory.Exists("Logs"))
            {
                return;
            }

            String[] files = Directory.GetFiles("Logs", "*.log");
            foreach(string file in files)
            {
                File.Delete(file);
            }
        }

        public static void Shutdown()
        {
            Log.CloseAndFlush();
        }
    }
}
