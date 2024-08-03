using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.PgSql.nc6;

internal class PgSqlLoggerService :
    LoggerService.nc6.LoggerService,
    IPgSqlLoggerService
{
    private readonly IPgSqlLogWriter _logWriter;

    public PgSqlLoggerService(
        ILoggerServiceConfig config, 
        IPgSqlLogWriter logWriter) : base(config, logWriter)
    {
        _logWriter = logWriter;
    }

    public Task<int> PurgeLogsAsync(IDictionary<SeverityEnu, TimeSpan> retainDic)
    {
        var cLw = (PgSqlLogWriter)_logWriter;
        return cLw.PurgeLogs(retainDic);
    }
}

internal sealed class PgSqlLoggerService<T> :
    PgSqlLoggerService,
    IPgSqlLoggerService<T>
{
    public PgSqlLoggerService(
        ILoggerServiceConfig config, 
        IPgSqlLogWriter logWriter) : base(config, logWriter)
    {
    }
}