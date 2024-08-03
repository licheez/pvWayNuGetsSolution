namespace PvWay.LoggerService.Abstractions.nc6;

public interface IPgSqlLoggerService: ISqlLoggerService{}
public interface IPgSqlLoggerService<out T>: ISqlLoggerService<T>{}