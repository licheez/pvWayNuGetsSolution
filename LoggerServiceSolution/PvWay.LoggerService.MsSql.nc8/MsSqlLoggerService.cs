using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.MsSql.nc8;

internal class MsSqlLoggerService(
    ILoggerServiceConfig config,
    IMsSqlLogWriter logWriter)
    : LoggerService.nc8.LoggerService(config, logWriter),
      IMsSqlLoggerService
{
    public Task<int> PurgeLogsAsync(IDictionary<SeverityEnu, TimeSpan> retainDic)
    {
        var cLw = (MsSqlLogWriter)logWriter;
        return cLw.PurgeLogs(retainDic);
    }
}

internal sealed class MsSqlLoggerService<T>(
    ILoggerServiceConfig config,
    IMsSqlLogWriter logWriter)
    : MsSqlLoggerService(config, logWriter),
      IMsSqlLoggerService<T>;