namespace PvWay.LoggerService.Abstractions.nc6;

public interface ILoggerServiceFactory<out T>
{
    T CreateLoggerService();
}