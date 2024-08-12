using Microsoft.Extensions.Configuration;
using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.nc6;

internal class LoggerServiceConfig : ILoggerServiceConfig
{
    public SeverityEnu MinLevel { get; }

    public LoggerServiceConfig(IConfiguration? config)
    {
        var minLevelCode = config?["minLogLevel"] ?? "T";
        MinLevel = minLevelCode.ToLower() switch
        {
            "trace" or "t" or "verbose" or "v" => SeverityEnu.Trace,
            "debug" or "d" => SeverityEnu.Debug,
            "info" or "information" or "i" => SeverityEnu.Info,
            "warning" or "w" => SeverityEnu.Warning,
            "error" or "e" => SeverityEnu.Error,
            "fatal" or "f" or "critic" or "critical" or "c" => SeverityEnu.Fatal,
            _ => SeverityEnu.Trace
        };
    }

    public LoggerServiceConfig(SeverityEnu minLogLevel)
    {
        MinLevel = minLogLevel;
    }
}