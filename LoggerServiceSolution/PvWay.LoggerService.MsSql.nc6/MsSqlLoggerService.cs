using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.MsSql.nc6;

internal class MsSqlLoggerService : 
    LoggerService.nc6.LoggerService,
    IMsSqlLoggerService
{
    private readonly IMsSqlLogWriter _logWriter;

    public MsSqlLoggerService(
        ILoggerServiceConfig config, 
        IMsSqlLogWriter logWriter) : base(config, logWriter)
    {
        _logWriter = logWriter;
    }

    public Task<int> PurgeLogsAsync(IDictionary<SeverityEnu, TimeSpan> retainDic)
    {
        var cLw = (MsSqlLogWriter)_logWriter;
        return cLw.PurgeLogs(retainDic);
    }
}

internal sealed class MsSqlLoggerService<T> : 
    MsSqlLoggerService,
    IMsSqlLoggerService<T>
{
    public MsSqlLoggerService(
        ILoggerServiceConfig config, 
        IMsSqlLogWriter logWriter) : base(config, logWriter)
    {
    }
}