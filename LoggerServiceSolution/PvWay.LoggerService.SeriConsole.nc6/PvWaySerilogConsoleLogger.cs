using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.SeriConsole.nc6;

public static class PvWaySerilogConsoleLogger
{
    // CREATE
    public static ISeriConsoleLoggerService Create(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new SerilogConsoleService(
            new LoggerServiceConfig(minLogLevel),
            new SerilogConsoleWriter());
    }

    public static ISeriConsoleLoggerService<T> Create<T>(
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
        services.TryAddSingleton<IConsoleLogWriter, SerilogConsoleWriter>();
    }
    
    // FACTORY
    public static void AddPvWaySeriConsoleLoggerFactory(
        this IServiceCollection services)
    {
        services.AddSingleton<
            ILoggerServiceFactory<IConsoleLoggerService>,
            SerilogConsoleFactory>();
    }
    
    // SERVICES
    public static void AddPvWaySeriConsoleLoggerService(
        this IServiceCollection services,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));
        
        var sd = new ServiceDescriptor(
            typeof(ISeriConsoleLoggerService<>),
            typeof(SerilogConsoleService<>),
            lifetime);
        services.Add(sd);
    }
    
}