using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.SeriConsole.nc8;

public static class PvWaySerilogConsoleLogger
{
    public static ISeriConsoleLoggerService Create(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new SerilogConsoleService(
            new LoggerServiceConfig(minLogLevel));
    }

    public static ISeriConsoleLoggerService<T> Create<T>(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new SerilogConsoleService<T>(
            new LoggerServiceConfig(minLogLevel));
    }
    
    public static void AddPvWaySeriConsoleLoggerService(
        this IServiceCollection services,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));
        
        var sd = new ServiceDescriptor(
            typeof(ILoggerService),
            typeof(SerilogConsoleService),
            lifetime);
        services.Add(sd);
        
        var sd2 = new ServiceDescriptor(
            typeof(ISeriConsoleLoggerService),
            typeof(SerilogConsoleService),
            lifetime);
        services.Add(sd2);
    }
    
}