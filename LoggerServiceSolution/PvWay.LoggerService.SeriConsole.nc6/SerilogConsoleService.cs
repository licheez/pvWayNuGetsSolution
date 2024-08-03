using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.SeriConsole.nc6;

internal sealed class SerilogConsoleService : 
    LoggerService.nc6.LoggerService,
    ISeriConsoleLoggerService
{
    public SerilogConsoleService(ILoggerServiceConfig config) : 
        base(config,new SerilogConsoleWriter())
    {
    }
}

internal sealed class SerilogConsoleService<T> : 
    LoggerService<T>,
    ISeriConsoleLoggerService<T>
{
    public SerilogConsoleService(ILoggerServiceConfig config) : 
        base(config, new SerilogConsoleWriter())
    {
    }
}