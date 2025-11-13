using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.SeriConsole.nc6;

internal sealed class SerilogConsoleLoggerProvider: IConsoleLoggerProvider
{
    private readonly SeverityEnu _minLogLevel;

    public SerilogConsoleLoggerProvider(
        IOptions<PvWayLoggerServiceConfig> options)
    {
        _minLogLevel = options.Value.MinLevel;
    }
    
    void IDisposable.Dispose()
    {
        // No resources to dispose
    }

    public ILogger CreateLogger(string categoryName)
    {
        var lw = new SerilogConsoleWriter();
        return new SerilogConsoleService(_minLogLevel, lw);
    }
}
