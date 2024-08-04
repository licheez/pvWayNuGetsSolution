using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.Mute.nc6;

public static class PvWayMuteLogger
{
    // CREATORS
    public static ILogWriter CreateWriter()
    {
        return new MuteLogWriter();
    }
    
    public static IMuteLoggerService CreateService(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new MuteLoggerService(
            new LoggerServiceConfig(minLogLevel),
            new MuteLogWriter());
    }

    public static IMuteLoggerService<T> CreateService<T>(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new MuteLoggerService<T>(
            new LoggerServiceConfig(minLogLevel),
            new MuteLogWriter());
    }
    
    // LOG WRITER
    public static void AddPvWayMuteLogWriter(
        this IServiceCollection services)
    {
        services.TryAddSingleton<IMuteLogWriter, MuteLogWriter>();
    }
    
    // SERVICE
    public static void AddPvWayMuteLoggerService(
        this IServiceCollection services,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));
        
        services.AddPvWayMuteLogWriter();

        var descriptors = new List<ServiceDescriptor>
        {
            new (typeof(IMuteLoggerService),
                 typeof(MuteLoggerService),
                 lifetime),
            new (typeof(IMuteLoggerService<>),
                 typeof(MuteLoggerService<>),
                 lifetime)
        };

        foreach (var sd in descriptors)
        {
            services.TryAdd(sd);
        }
    }
}