using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.LoggerService.Console.nc8;

internal sealed class ConsoleLoggerProvider(
    SeverityEnu minLogLevel = SeverityEnu.Trace) : ILoggerProvider
{
    public void Dispose()
    {
    }

    public ILogger CreateLogger(string categoryName)
    {
        return PvWayConsoleLogger.CreateService(minLogLevel);
    }
}