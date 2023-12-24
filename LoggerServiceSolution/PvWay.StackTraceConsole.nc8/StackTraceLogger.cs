using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.StackTraceConsole.nc8;

internal class StackTraceLogger() : LoggerService.nc8.LoggerService(
    new StackTraceLogWriter(), 
    new LoggerServiceConfig(SeverityEnu.Trace));