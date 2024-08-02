namespace PvWay.LoggerService.Abstractions.nc6;

public interface IMsSqlLoggerService: ISqlLoggerService{};
public interface IMsSqlLoggerService<out T>: ISqlLoggerService<T>{};