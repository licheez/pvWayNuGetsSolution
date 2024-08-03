using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.Mute.nc6;

public static class PvWayMuteLogger
{
    public static IMuteLoggerService Create(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new MuteLoggerService(new LoggerServiceConfig(minLogLevel));
    }

    public static IMuteLoggerService<T> Create<T>(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new MuteLoggerService<T>(new LoggerServiceConfig(minLogLevel));
    }
    
    public static void AddPvWayMuteLoggerService(
        this IServiceCollection services,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));
        
        var sd = new ServiceDescriptor(
            typeof(ILoggerService), 
            typeof(MuteLoggerService),
            lifetime);
        services.Add(sd);
        
        var sd2 = new ServiceDescriptor(
            typeof(IMuteLoggerService), 
            typeof(MuteLoggerService),
            lifetime);
        services.Add(sd2);
    }
}