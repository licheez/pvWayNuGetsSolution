using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.UTest.nc8;

public static class PvWayUTestLogger
{
    public static IUTestLogWriter CreateUTestLogWriter()
    {
        return new UTestLogWriter();
    }
    
    public static IUTestLoggerService CreateService(
        IUTestLogWriter utLw)
    {
        return new UTestLoggerService(
            new LoggerServiceConfig(SeverityEnu.Trace), 
            utLw);
    }
    
    public static IUTestLoggerService CreateService(
        out IUTestLogWriter utLw)
    {
        utLw = CreateUTestLogWriter();
        return new UTestLoggerService(
            new LoggerServiceConfig(SeverityEnu.Trace), 
            utLw);
    }

    
    public static IUTestLoggerService<T> CreateService<T>(
        IUTestLogWriter utLw)
    {
        return new UTestLoggerService<T>(
            new LoggerServiceConfig(SeverityEnu.Trace), 
            utLw);
    }

    public static IUTestLoggerService<T> CreateService<T>(
        out IUTestLogWriter utLw)
    {
        utLw = CreateUTestLogWriter();
        return new UTestLoggerService<T>(
            new LoggerServiceConfig(SeverityEnu.Trace), 
            utLw);
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