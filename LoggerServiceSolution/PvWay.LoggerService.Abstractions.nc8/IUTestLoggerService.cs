
namespace PvWay.LoggerService.Abstractions.nc8;

public interface IUTestLoggerService: ILoggerService;
public interface IUTestLoggerService<out T>: ILoggerService<T>;