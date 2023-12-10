using Microsoft.Extensions.DependencyInjection;
using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.LoggerService.Console.nc8;

public static class ConsoleLoggerDi
{
    public static void AddPvWayConsoleLoggerService(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Transient,
        SeverityEnu minLogLevel = SeverityEnu.Info)
    {
        var sd = new ServiceDescriptor(
            typeof(ILoggerService),
            _ => new ConsoleLoggerService(minLogLevel),
            lifetime);
        services.Add(sd);
        
        var sd2 = new ServiceDescriptor(
            typeof(IConsoleLoggerService),
            _ => new ConsoleLoggerService(minLogLevel),
            lifetime);
        services.Add(sd2);
    }
}