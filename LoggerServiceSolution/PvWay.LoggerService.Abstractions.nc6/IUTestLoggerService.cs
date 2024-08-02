
namespace PvWay.LoggerService.Abstractions.nc6;

public interface IUTestLoggerService: ILoggerService{};
public interface IUTestLoggerService<out T>: ILoggerService<T>{};