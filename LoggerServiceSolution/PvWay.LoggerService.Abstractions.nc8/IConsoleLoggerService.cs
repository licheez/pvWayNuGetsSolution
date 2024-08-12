namespace PvWay.LoggerService.Abstractions.nc8;

public interface IConsoleLoggerService: ILoggerService{}

public interface IConsoleLoggerService<out T>: ILoggerService<T>{}