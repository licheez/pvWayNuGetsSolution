using Microsoft.Extensions.Options;
using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.SeriConsole.nc6;

internal sealed class SerilogConsoleService : 
    BaseLoggerService,
    ISeriConsoleLoggerService
{
    public SerilogConsoleService(
        IOptions<PvWayLoggerServiceConfig> options,
        IConsoleLogWriter logWriter) : 
        base(options.Value.MinLevel,logWriter)
    {
    }

    public SerilogConsoleService(
        SeverityEnu minLogLevel,
        IConsoleLogWriter logWriter) : base(minLogLevel, logWriter)
    {
    }
}
