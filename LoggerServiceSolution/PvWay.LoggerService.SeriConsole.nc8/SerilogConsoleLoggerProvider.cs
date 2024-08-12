using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.LoggerService.SeriConsole.nc8;

internal sealed class SerilogConsoleLoggerProvider(
    SeverityEnu minLogLevel = SeverityEnu.Trace) : ILoggerProvider
{
    public void Dispose()
    {
    }

    public ILogger CreateLogger(string categoryName)
    {
        return PvWaySerilogConsoleLogger.CreateService(minLogLevel);
    }
}
