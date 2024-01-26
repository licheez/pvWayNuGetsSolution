namespace PvWay.LoggerService.Abstractions.nc8;

public interface IMsSqlLoggerService: ISqlLoggerService;
public interface IMsSqlLoggerService<out T>: ISqlLoggerService<T>;