using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.Hybrid.nc6;

public static class PvWayHybridLogger
{
    // CREATORS
    public static IHybridLoggerService CreateService(
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        params ILogWriter[] logWriters)
    {
        var sConfig = new LoggerServiceConfig(minLogLevel);
        var hConfig = new HybridLoggerConfig(logWriters);
        return new HybridLoggerService(sConfig, hConfig);
    }
    
    // FACTORY
    public static void AddPvWayHybridLoggerServiceFactory(
        this IServiceCollection services,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        params ILogWriter[] logWriters)
    {
        services.AddSingleton<ILoggerServiceFactory<IHybridLoggerService>>(_ => 
            new HybridLoggerServiceFactory(minLogLevel, logWriters));
    }
    
    // SERVICE
    public static void AddPvWayHybridLoggerService(
        this IServiceCollection services,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Singleton,
        params ILogWriter[] logWriters)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));
        
        services.AddSingleton<HybridLoggerConfig>(_ =>
            new HybridLoggerConfig(logWriters));
        
        RegisterService(services, lifetime);
    }
    
    /// <summary>
    /// This will search the service container for a ConsoleLogWriter
    /// and a SqlLogWriter for creating a HybridLoggerConfig
    /// </summary>
    /// <param name="services"></param>
    /// <param name="minLogLevel"></param>
    /// <param name="lifetime"></param>
    public static void AddPvWayHybridLoggerService(
        this IServiceCollection services,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));

        services.AddSingleton<HybridLoggerConfig>(sp =>
        {
            var cLw = sp.GetRequiredService<IConsoleLogWriter>();
            var sLw = sp.GetRequiredService<ISqlLogWriter>();
            return new HybridLoggerConfig(cLw, sLw);
        }); 
        
        RegisterService(services, lifetime);
    }
    
    
    private static void RegisterService(
        IServiceCollection services,
        ServiceLifetime lifetime)
    {
        var descriptors = new List<ServiceDescriptor>
        {
            new(typeof(IHybridLoggerService),
                typeof(HybridLoggerService),
                lifetime),
            new(typeof(IHybridLoggerService<>),
                typeof(HybridLoggerService<>),
                lifetime)
        };
        foreach (var sd in descriptors)
        {
            services.TryAdd(sd);
        }
    } 
}