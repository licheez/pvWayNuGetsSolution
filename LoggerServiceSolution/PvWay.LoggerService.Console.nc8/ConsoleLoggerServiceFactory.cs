using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.Console.nc8;

internal class ConsoleLoggerServiceFactory(ILoggerServiceConfig config) :
    ILoggerServiceFactory<IConsoleLoggerService>
{
    public IConsoleLoggerService CreateLoggerService()
    {
        return new ConsoleLoggerService(
            config,
            new ConsoleLogWriter());
    }
}

internal class ConsoleLoggerServiceFactory<T>(ILoggerServiceConfig config) :
    ILoggerServiceFactory<IConsoleLoggerService<T>>
{
    public IConsoleLoggerService<T> CreateLoggerService()
    {
        return new ConsoleLoggerService<T>(
            config,
            new ConsoleLogWriter());
    }
}