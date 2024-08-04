using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.SeriConsole.nc6;

public static class PvWaySerilogConsoleLogger
{
    // CREATE
    public static IConsoleLogWriter CreateWriter()
    {
        return new SerilogConsoleWriter();
    }
    
    public static ISeriConsoleLoggerService CreateService(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new SerilogConsoleService(
            new LoggerServiceConfig(minLogLevel),
            new SerilogConsoleWriter());
    }

    public static ISeriConsoleLoggerService<T> CreateService<T>(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new SerilogConsoleService<T>(
            new LoggerServiceConfig(minLogLevel),
            new SerilogConsoleWriter());
    }
    
    // LOG WRITER
    public static void AddPvWaySeriConsoleLogWriter(
        this IServiceCollection services)
    {
        services.TryAddSingleton<
            IConsoleLogWriter, SerilogConsoleWriter>();
    }
    
    // FACTORY
    public static void AddPvWaySeriConsoleLoggerFactory(
        this IServiceCollection services)
    {
        services.TryAddSingleton<
            ILoggerServiceFactory<IConsoleLoggerService>,
            SerilogConsoleFactory>();
    }
    
    // SERVICES
    public static void AddPvWaySeriConsoleLoggerService(
        this IServiceCollection services,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.AddPvWaySeriConsoleLogWriter();
        
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));
        
        var descriptors = new List<ServiceDescriptor>
        {
            new(typeof(ISeriConsoleLoggerService),
                typeof(SerilogConsoleService),
                lifetime),
            new(typeof(IConsoleLoggerService),
                typeof(SerilogConsoleService),
                lifetime),
            new(typeof(ISeriConsoleLoggerService<>),
                typeof(SerilogConsoleService<>),
                lifetime),
            new(typeof(IConsoleLoggerService<>),
                typeof(SerilogConsoleService<>),
                lifetime),
        };
        foreach (var sd in descriptors)
        {
            services.TryAdd(sd);
        }
    }
    
}