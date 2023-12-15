using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.SeriConsole.nc8;

internal sealed class SerilogConsoleService(
    ILoggerServiceConfig config) : 
    LoggerService.nc8.LoggerService(
        new SerilogConsoleWriter(), config),
    ISeriConsoleLoggerService;
    
internal sealed class SerilogConsoleService<T>(
    ILoggerServiceConfig config) : 
    LoggerService<T>(
        new SerilogConsoleWriter(), config),
    ISeriConsoleLoggerService<T>;