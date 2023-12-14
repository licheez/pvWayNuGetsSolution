namespace PvWay.LoggerService.Abstractions.nc8;

public interface ISeriConsoleLoggerService: IConsoleLoggerService;
public interface ISeriConsoleLoggerService<out T>: IConsoleLoggerService<T>;