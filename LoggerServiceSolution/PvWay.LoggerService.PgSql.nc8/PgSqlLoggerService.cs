using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.PgSql.nc8;

internal sealed class PgSqlLoggerService(
    IPgSqlLogWriter logWriter, ILoggerServiceConfig config) : 
    LoggerService.nc8.LoggerService(logWriter, config),
    IPgSqlLoggerService;
    
internal sealed class PgSqlLoggerService<T>(
    IPgSqlLogWriter logWriter, ILoggerServiceConfig config) : 
    LoggerService<T>(logWriter, config),
    IPgSqlLoggerService<T>;