namespace PvWay.LoggerService.nc6;

internal class ConsoleLogger : Logger, IPvWayConsoleLoggerService
{
    internal ConsoleLogger() :
        base(new ConsoleLogWriter())
    {
    }
}