using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.UTest.nc6;

internal sealed class UTestLoggerService : 
    LoggerService.nc6.LoggerService, 
    IUTestLoggerService
{
    public UTestLoggerService(
        ILoggerServiceConfig config, 
        IUTestLogWriter logWriter) : base(config, logWriter)
    {
    }
}

internal sealed class UTestLoggerService<T> : 
    LoggerService<T>, 
    IUTestLoggerService<T>
{
    public UTestLoggerService(
        ILoggerServiceConfig config, 
        IUTestLogWriter logWriter) : base(config, logWriter)
    {
    }
}