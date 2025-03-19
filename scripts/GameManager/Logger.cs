
namespace GameNamespace.GameManager
{
    using Serilog;

    public static class Logger
    {
        public static void Init()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("App", "TowerDefense")
                .WriteTo.File(
                    "Logs/log.log",
                    rollingInterval: RollingInterval.Minute,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();
        }

        public static void Shutdown()
        {
            Log.CloseAndFlush();
        }
    }
}
