using Microsoft.Extensions.DependencyInjection;
using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.LoggerService.SeriConsole.nc8;

public static class SeriLogConsoleLoggerDi
{
    public static void AddPvWaySeriConsoleLoggerService(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Transient,
        SeverityEnu minLogLevel = SeverityEnu.Info)
    {
        var sd = new ServiceDescriptor(
            typeof(ILoggerService),
            _ => new SeriLogConsoleService(minLogLevel),
            lifetime);
        services.Add(sd);
        
        var sd2 = new ServiceDescriptor(
            typeof(ISeriConsoleLoggerService),
            _ => new SeriLogConsoleService(minLogLevel),
            lifetime);
        services.Add(sd2);
    }
}