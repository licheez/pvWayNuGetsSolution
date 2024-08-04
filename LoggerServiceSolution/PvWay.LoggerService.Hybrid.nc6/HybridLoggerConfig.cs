using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.Hybrid.nc6;

public sealed class HybridLoggerConfig
{
    public ILogWriter[] LogWriters { get; }

    public HybridLoggerConfig(
        params ILogWriter[] logWriters)
    {
        LogWriters = logWriters;
    }
}