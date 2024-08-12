namespace PvWay.LoggerService.Abstractions.nc8;

public interface IHybridLoggerService: ILoggerService{}

public interface IHybridLoggerService<out T>: ILoggerService<T>{}