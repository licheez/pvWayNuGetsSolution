using Microsoft.Extensions.Logging;

namespace PvWay.LoggerService.nc6;

internal class MsConsoleLogger : MsLogger, IPvWayMsConsoleLoggerService
{
    public MsConsoleLogger() :
        base(LoggerFactory
             .Create(builder => builder.AddConsole())
             .CreateLogger("PvWayMsConsoleLogger"))
    {
    }
}