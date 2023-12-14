using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.MsSql.nc8;

internal sealed class MsSqlLoggerService(
    IMsSqlLogWriter logWriter, ILoggerServiceConfig config) : 
    LoggerService.nc8.LoggerService(logWriter, config),
    IMsSqlLoggerService;

internal sealed class MsSqlLoggerService<T>(
    ILogWriter logWriter, ILoggerServiceConfig config) : 
    LoggerService<T>(logWriter, config),
    IMsSqlLoggerService<T>;