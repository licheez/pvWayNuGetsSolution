namespace PvWay.LoggerService.Abstractions.nc8;

public interface IMuteLoggerService: ILoggerService{}
public interface IMuteLoggerService<out T>: ILoggerService<T>{}