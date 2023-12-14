namespace PvWay.LoggerService.Abstractions.nc8;

public interface IPgSqlLoggerService: ILoggerService;
public interface IPgSqlLoggerService<out T>: ILoggerService<T>;