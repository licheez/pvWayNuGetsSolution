using Microsoft.Extensions.Logging;

namespace PvWay.LoggerService.nc6;

internal class MsLogger : Logger, IPvWayMsLoggerService
{
    public MsLogger(
        ILogger msLogger) : base(
        new MsLogWriter(msLogger))
    {
    }
}