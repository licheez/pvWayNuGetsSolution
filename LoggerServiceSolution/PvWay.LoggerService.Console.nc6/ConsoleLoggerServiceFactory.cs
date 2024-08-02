using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.Console.nc6;

internal class ConsoleLoggerServiceFactory :
    ILoggerServiceFactory<IConsoleLoggerService>
{
    private readonly ILoggerServiceConfig _config;

    public ConsoleLoggerServiceFactory(ILoggerServiceConfig config)
    {
        _config = config;
    }

    public IConsoleLoggerService CreateLoggerService()
    {
        return new ConsoleLoggerService(_config);
    }
}

internal class ConsoleLoggerServiceFactory<T> :
    ILoggerServiceFactory<IConsoleLoggerService<T>>
{
    private readonly ILoggerServiceConfig _config;

    public ConsoleLoggerServiceFactory(ILoggerServiceConfig config)
    {
        _config = config;
    }

    public IConsoleLoggerService<T> CreateLoggerService()
    {
        return new ConsoleLoggerService<T>(_config);
    }
}