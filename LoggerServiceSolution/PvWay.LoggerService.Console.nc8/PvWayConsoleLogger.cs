using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.Console.nc8;

public static class PvWayConsoleLogger
{
    // LOGGER PROVIDER
    public static ILoggerProvider GetProvider(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new ConsoleLoggerProvider(minLogLevel);
    }
    
    // CREATORS
    public static IConsoleLogWriter CreateWriter()
    {
        return new ConsoleLogWriter();
    }

    public static IConsoleLoggerService CreateService(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new ConsoleLoggerService(
            new LoggerServiceConfig(minLogLevel),
            new ConsoleLogWriter());
    }

    public static IConsoleLoggerService<T> CreateService<T>(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new ConsoleLoggerService<T>(
            new LoggerServiceConfig(minLogLevel),
            new ConsoleLogWriter());
    }

   // WRITER 
    public static void AddPvWayConsoleLogWriter(
        this IServiceCollection services)
    {
        services.TryAddSingleton<IConsoleLogWriter, ConsoleLogWriter>();
    }
   
    // FACTORY
    public static void AddPvWayConsoleLoggerFactory(
        this IServiceCollection services)
    {
        services.AddSingleton<
            ILoggerServiceFactory<IConsoleLoggerService>,
            ConsoleLoggerServiceFactory>();
    }
    
    // SERVICE
    public static void AddPvWayConsoleLoggerService(
        this IServiceCollection services,
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));
        
        services.AddPvWayConsoleLogWriter();

        const ServiceLifetime lifetime = ServiceLifetime.Singleton;
        var descriptors = new List<ServiceDescriptor>
        {
            new(typeof(IConsoleLoggerService),
                typeof(ConsoleLoggerService),
                lifetime),
            new(typeof(IConsoleLoggerService<>),
                typeof(ConsoleLoggerService<>),
                lifetime)
        };
        foreach (var sd in descriptors)
        {
            services.TryAdd(sd);
        }
    }
    
}