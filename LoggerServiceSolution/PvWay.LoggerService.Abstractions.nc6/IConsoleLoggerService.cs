namespace PvWay.LoggerService.Abstractions.nc6;

public interface IConsoleLoggerService: ILoggerService{};

public interface IConsoleLoggerService<out T>: ILoggerService<T>{};