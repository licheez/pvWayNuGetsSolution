namespace PvWay.LoggerService.Abstractions.nc6;

public interface IMuteLoggerService: ILoggerService{}
public interface IMuteLoggerService<out T>: ILoggerService<T>{}