namespace PvWay.LoggerService.Abstractions.nc8;

public interface ILoggerServiceFactory<out T>
{
    T CreateLoggerService();
}