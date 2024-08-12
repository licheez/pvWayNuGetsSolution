using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.Console.nc8;

internal sealed class ConsoleLoggerService(
    ILoggerServiceConfig config,
    IConsoleLogWriter lw) :
    LoggerService.nc8.LoggerService(config, lw),
    IConsoleLoggerService;

internal sealed class ConsoleLoggerService<T>(
    ILoggerServiceConfig config,
    IConsoleLogWriter lw) :
    LoggerService<T>(config, lw),
    IConsoleLoggerService<T>;

