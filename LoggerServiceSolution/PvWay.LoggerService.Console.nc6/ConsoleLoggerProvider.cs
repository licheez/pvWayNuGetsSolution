using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.Console.nc6;

internal sealed class ConsoleLoggerProvider: ILoggerProvider
{
    private readonly SeverityEnu _minLogLevel;

    public ConsoleLoggerProvider(SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        _minLogLevel = minLogLevel;
    }
    
    public void Dispose()
    {
    }

    public ILogger CreateLogger(string categoryName)
    {
        return PvWayConsoleLogger.CreateService(_minLogLevel);
    }
}