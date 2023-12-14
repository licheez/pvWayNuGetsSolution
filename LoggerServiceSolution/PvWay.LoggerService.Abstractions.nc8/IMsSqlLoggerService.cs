namespace PvWay.LoggerService.Abstractions.nc8;

public interface IMsSqlLoggerService: ILoggerService;
public interface IMsSqlLoggerService<out T>: ILoggerService<T>;