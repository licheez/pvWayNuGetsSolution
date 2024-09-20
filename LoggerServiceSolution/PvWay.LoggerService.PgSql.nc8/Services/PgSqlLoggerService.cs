using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.PgSql.nc8.Services;

internal class PgSqlLoggerService(
    ILoggerServiceConfig config,
    IPgSqlLogWriter logWriter)
    : LoggerService.nc8.LoggerService(config, logWriter),
      IPgSqlLoggerService
{
    public Task<int> PurgeLogsAsync(IDictionary<SeverityEnu, TimeSpan> retainDic)
    {
        var cLw = (PgSqlLogWriter)logWriter;
        return cLw.PurgeLogs(retainDic);
    }
}

internal sealed class PgSqlLoggerService<T>(
    ILoggerServiceConfig config,
    IPgSqlLogWriter logWriter)
    : PgSqlLoggerService(config, logWriter),
      IPgSqlLoggerService<T>;