
namespace GameNamespace.GameManager
{
    using Serilog;

    public static class Logger
    {
        public static void Init()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public static void Shutdown()
        {
            Log.CloseAndFlush();
        }
    }
}
