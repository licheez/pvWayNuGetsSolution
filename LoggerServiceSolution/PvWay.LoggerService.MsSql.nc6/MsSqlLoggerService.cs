using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.MsSql.nc6;

internal class MsSqlLoggerService : 
    LoggerService.nc6.LoggerService,
    IMsSqlLoggerService
{
    private readonly IMsSqlLogWriter _logWriter1;

    public MsSqlLoggerService(IMsSqlLogWriter logWriter, ILoggerServiceConfig config) : base(logWriter, config)
    {
        _logWriter1 = logWriter;
    }

    public Task<int> PurgeLogsAsync(IDictionary<SeverityEnu, TimeSpan> retainDic)
    {
        var cLw = (MsSqlLogWriter)_logWriter1;
        return cLw.PurgeLogs(retainDic);
    }
}

internal sealed class MsSqlLoggerService<T> : 
    MsSqlLoggerService,
    IMsSqlLoggerService<T>
{
    public MsSqlLoggerService(IMsSqlLogWriter logWriter, ILoggerServiceConfig config) : base(logWriter, config)
    {
    }
}