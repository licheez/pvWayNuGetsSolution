using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.LoggerService.Console.nc8;

internal sealed class ConsoleLoggerService(
    SeverityEnu minLevel = SeverityEnu.Info) :
    LoggerService.nc8.LoggerService(
        new ConsoleLogWriter(), minLevel);

