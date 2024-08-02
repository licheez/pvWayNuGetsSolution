using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.Console.nc6;

public static class PvWayConsoleLogger
{
    public static IConsoleLoggerService Create(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new ConsoleLoggerService(
            new LoggerServiceConfig(minLogLevel));
    }

    public static IConsoleLoggerService<T> Create<T>(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new ConsoleLoggerService<T>(
            new LoggerServiceConfig(minLogLevel));
    }
    
    public static void AddPvWayConsoleLoggerService(
        this IServiceCollection services,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));
        
        services.AddSingleton<
            ILoggerServiceFactory<IConsoleLoggerService>,
            ConsoleLoggerServiceFactory>();

        services.AddSingleton(
            typeof(IConsoleLoggerService<>),
            typeof(ConsoleLoggerService<>));

        var sd = new ServiceDescriptor(
            typeof(ILoggerService), 
            typeof(ConsoleLoggerService),
            lifetime);
        services.Add(sd);
        
        var sd2 = new ServiceDescriptor(
            typeof(IConsoleLoggerService),
            typeof(ConsoleLoggerService),
            lifetime);
        services.Add(sd2);
    }
    
}