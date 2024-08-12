namespace PvWay.LoggerService.Abstractions.nc8;

public interface IPgSqlLoggerService: ISqlLoggerService{}
public interface IPgSqlLoggerService<out T>: ISqlLoggerService<T>{}