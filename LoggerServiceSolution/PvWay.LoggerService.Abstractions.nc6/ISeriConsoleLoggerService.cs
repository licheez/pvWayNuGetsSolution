namespace PvWay.LoggerService.Abstractions.nc6;

public interface ISeriConsoleLoggerService: IConsoleLoggerService{};
public interface ISeriConsoleLoggerService<out T>: IConsoleLoggerService<T>{};