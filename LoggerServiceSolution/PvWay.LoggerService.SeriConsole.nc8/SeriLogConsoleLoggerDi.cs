using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.SeriConsole.nc8;

public static class SeriLogConsoleLoggerDi
{
    public static void AddPvWaySeriConsoleLoggerService(
        this IServiceCollection services,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));
        
        var sd = new ServiceDescriptor(
            typeof(ILoggerService),
            typeof(SeriLogConsoleService),
            lifetime);
        services.Add(sd);
        
        var sd2 = new ServiceDescriptor(
            typeof(ISeriConsoleLoggerService),
            typeof(SeriLogConsoleService),
            lifetime);
        services.Add(sd2);
    }
    
}