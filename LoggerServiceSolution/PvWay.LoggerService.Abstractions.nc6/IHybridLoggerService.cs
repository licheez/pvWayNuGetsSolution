namespace PvWay.LoggerService.Abstractions.nc6;

public interface IHybridLoggerService: ILoggerService{}

public interface IHybridLoggerService<out T>: ILoggerService<T>{}