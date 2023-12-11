using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.Console.nc8;

public static class ConsoleLoggerDi
{
    public static void AddPvWayConsoleLoggerService(
        this IServiceCollection services,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));
        
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