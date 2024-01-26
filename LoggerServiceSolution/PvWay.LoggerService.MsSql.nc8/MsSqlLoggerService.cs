using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.MsSql.nc8;

internal class MsSqlLoggerService(
    IMsSqlLogWriter logWriter, ILoggerServiceConfig config) : 
    LoggerService.nc8.LoggerService(logWriter, config),
    IMsSqlLoggerService
{
    public Task<int> PurgeLogsAsync(IDictionary<SeverityEnu, TimeSpan> retainDic)
    {
        var cLw = (MsSqlLogWriter)logWriter;
        return cLw.PurgeLogs(retainDic);
    }
}

internal sealed class MsSqlLoggerService<T>(
    IMsSqlLogWriter logWriter, ILoggerServiceConfig config) : 
    MsSqlLoggerService(logWriter, config),
    IMsSqlLoggerService<T>;