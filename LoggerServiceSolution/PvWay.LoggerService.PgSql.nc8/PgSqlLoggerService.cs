using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.PgSql.nc8;

internal class PgSqlLoggerService(
    IPgSqlLogWriter logWriter,
    ILoggerServiceConfig config) :
    LoggerService.nc8.LoggerService(logWriter, config),
    IPgSqlLoggerService
{
    public Task<int> PurgeLogsAsync(IDictionary<SeverityEnu, TimeSpan> retainDic)
    {
        var cLw = (PgSqlLogWriter)logWriter;
        return cLw.PurgeLogs(retainDic);
    }
}

internal sealed class PgSqlLoggerService<T>(
    IPgSqlLogWriter logWriter,
    ILoggerServiceConfig config) :
    PgSqlLoggerService(logWriter, config),
    IPgSqlLoggerService<T>;