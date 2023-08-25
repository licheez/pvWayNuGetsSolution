namespace PvWay.LoggerService.nc6;

public class ConsoleLogger : Logger, IPvWayConsoleLoggerService
{
    internal ConsoleLogger() :
        base(new ConsoleLogWriter())
    {
    }
}