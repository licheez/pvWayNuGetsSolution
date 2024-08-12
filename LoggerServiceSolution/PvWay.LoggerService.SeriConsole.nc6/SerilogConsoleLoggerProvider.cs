using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.SeriConsole.nc6;

internal sealed class SerilogConsoleLoggerProvider: ILoggerProvider
{
    private readonly SeverityEnu _minLogLevel;

    public SerilogConsoleLoggerProvider(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        _minLogLevel = minLogLevel;
    }
    
    public void Dispose()
    {
    }

    public ILogger CreateLogger(string categoryName)
    {
        return PvWaySerilogConsoleLogger.CreateService(_minLogLevel);
    }
}
