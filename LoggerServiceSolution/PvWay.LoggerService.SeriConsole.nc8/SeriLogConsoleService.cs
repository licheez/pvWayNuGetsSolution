using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.SeriConsole.nc8;

internal sealed class SeriLogConsoleService(
    ILoggerServiceConfig config) : 
    LoggerService.nc8.LoggerService(
        new SeriLogConsoleWriter(), config),
    ISeriConsoleLoggerService;