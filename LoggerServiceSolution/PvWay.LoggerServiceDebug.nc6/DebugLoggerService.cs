using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerServiceDebug.nc6;

internal class DebugLoggerService: LoggerService.nc6.LoggerService
{
    public DebugLoggerService() : 
        base(new DebugLoggerServiceConfig(), 
            new DebugLoggerWriter())
    {
    }
}

internal class DebugLoggerServiceConfig : ILoggerServiceConfig
{
    public SeverityEnu MinLevel => SeverityEnu.Trace;
}

 