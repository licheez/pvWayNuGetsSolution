namespace PvWay.LoggerService.Abstractions.nc8;

public interface ISqlLoggerService: ILoggerService
{
    Task<int> PurgeLogsAsync(IDictionary<SeverityEnu, TimeSpan> retainDic);
}

public interface ISqlLoggerService<out T>: ISqlLoggerService, ILoggerService<T>{}