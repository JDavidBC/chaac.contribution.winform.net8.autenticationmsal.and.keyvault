using Microsoft.Extensions.Configuration;
using Serilog;

namespace BabilaFuente.Services;

public static class LoggingSetup
{
    public static void ConfigureLogging(IConfiguration configuration)
    {
        var logPath = configuration["Logging:LogPath"] ?? @"C:\PATH_LOGS";
        var fileName = configuration["Logging:FileName"] ?? "log-.txt";
        var rollingIntervalString = configuration["Logging:RollingInterval"] ?? "Day";

        if (!System.IO.Directory.Exists(logPath))
            System.IO.Directory.CreateDirectory(logPath);

        var rollingInterval = rollingIntervalString.ToLower() switch
        {
            "infinite" => RollingInterval.Infinite,
            "year" => RollingInterval.Year,
            "month" => RollingInterval.Month,
            "day" => RollingInterval.Day,
            "hour" => RollingInterval.Hour,
            "minute" => RollingInterval.Minute,
            _ => RollingInterval.Day
        };

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                path: System.IO.Path.Combine(logPath, fileName),
                rollingInterval: rollingInterval,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }
}

