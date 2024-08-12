using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.SeriConsole.nc8;

internal sealed class SerilogConsoleService(
    ILoggerServiceConfig config,
    IConsoleLogWriter logWriter)
    :
        LoggerService.nc8.LoggerService(config, logWriter),
        ISeriConsoleLoggerService;

internal sealed class SerilogConsoleService<T>(
    ILoggerServiceConfig config,
    IConsoleLogWriter logWriter)
    :
        LoggerService<T>(config, logWriter),
        ISeriConsoleLoggerService<T>;