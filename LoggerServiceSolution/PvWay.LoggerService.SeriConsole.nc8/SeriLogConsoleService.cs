using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.LoggerService.SeriConsole.nc8;

internal sealed class SeriLogConsoleService(
    SeverityEnu minLevel = SeverityEnu.Info) : 
    LoggerService.nc8.LoggerService(
        new SeriLogConsoleWriter(), minLevel);