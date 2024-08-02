using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.Console.nc6;

internal sealed class ConsoleLoggerService :
    LoggerService.nc6.LoggerService,
    IConsoleLoggerService
{
    public ConsoleLoggerService(
        ILoggerServiceConfig config): base(new ConsoleLogWriter(), config)
    {
    }

}

internal sealed class ConsoleLoggerService<T> :
    LoggerService<T>,
    IConsoleLoggerService<T>
{
    public ConsoleLoggerService(ILoggerServiceConfig config) : 
        base(new ConsoleLogWriter(), config)
    {
    }
}
