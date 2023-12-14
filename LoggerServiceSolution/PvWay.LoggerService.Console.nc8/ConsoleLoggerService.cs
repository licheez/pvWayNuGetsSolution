using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.Console.nc8;

internal sealed class ConsoleLoggerServiceConfig(
    SeverityEnu minLevel = SeverityEnu.Trace) : ILoggerServiceConfig
{
    public SeverityEnu MinLevel { get; } = minLevel;
}

internal sealed class ConsoleLoggerService(
    ILoggerServiceConfig config) :
    LoggerService.nc8.LoggerService(
        new ConsoleLogWriter(), config), 
    IConsoleLoggerService
{
    public ConsoleLoggerService(SeverityEnu minLogLevel =  SeverityEnu.Trace) : 
        this(new ConsoleLoggerServiceConfig(minLogLevel))
    {
    }
}

internal sealed class ConsoleLoggerService<T>(
    ILoggerServiceConfig config) :
    LoggerService<T>(new ConsoleLogWriter(), config),
    IConsoleLoggerService<T>
{
    public ConsoleLoggerService(SeverityEnu minLogLevel = SeverityEnu.Trace): 
        this(new LoggerServiceConfig(minLogLevel))
    {
    }
}

