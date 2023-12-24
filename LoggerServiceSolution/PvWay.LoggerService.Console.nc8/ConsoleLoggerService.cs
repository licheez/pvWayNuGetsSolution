using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.Console.nc8;

internal sealed class ConsoleLoggerService(
    ILoggerServiceConfig config) :
    LoggerService.nc8.LoggerService(
        new ConsoleLogWriter(), config),
    IConsoleLoggerService;

internal sealed class ConsoleLoggerService<T>(
    ILoggerServiceConfig config) :
    LoggerService<T>(new ConsoleLogWriter(), config),
    IConsoleLoggerService<T>;

