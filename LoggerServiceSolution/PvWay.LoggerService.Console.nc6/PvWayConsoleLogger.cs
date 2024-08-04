using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.Console.nc6;

public static class PvWayConsoleLogger
{
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

    public static IConsoleLoggerService<T> Create<T>(
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
        
        services.TryAddSingleton<IConsoleLogWriter, ConsoleLogWriter>();
        
        // Injecting the service
        services.AddSingleton(
            typeof(IConsoleLoggerService<>),
            typeof(ConsoleLoggerService<>));
    }
    
}