using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.PgSql.nc8;

internal sealed class PgSqlLoggerService(
    IPgSqlLogWriter logWriter, ILoggerServiceConfig config) : 
    LoggerService.nc8.LoggerService(logWriter, config),
    IMsSqlLoggerService;