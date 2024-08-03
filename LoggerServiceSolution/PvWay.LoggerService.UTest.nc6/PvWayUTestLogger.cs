using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.UTest.nc6;

public static class PvWayUTestLogger
{
    public static IUTestLogWriter CreateUTestLogWriter()
    {
        return new UTestLogWriter();
    }
    
    public static IUTestLoggerService Create(
        IUTestLogWriter utLw)
    {
        return new UTestLoggerService(
            utLw, new LoggerServiceConfig(SeverityEnu.Trace));
    }
    
    public static IUTestLoggerService<T> Create<T>(
        IUTestLogWriter utLw)
    {
        return new UTestLoggerService<T>(
            utLw, new LoggerServiceConfig(SeverityEnu.Trace));
    }
    
    /// <summary>
    /// Injects a transient IUTestLoggerService
    /// </summary>
    /// <param name="services"></param>
    /// <returns>IUTestLogWriter</returns>
    public static IUTestLogWriter AddPvWayUTestLoggerService(
        this IServiceCollection services)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(SeverityEnu.Trace));
        
        var logWriter = new UTestLogWriter();
        services.TryAddTransient<IUTestLogWriter>(_ => logWriter);
        services.TryAddTransient<ILoggerService, UTestLoggerService>();
        services.TryAddTransient<IUTestLoggerService, UTestLoggerService>();
        
        return logWriter;
    }
}