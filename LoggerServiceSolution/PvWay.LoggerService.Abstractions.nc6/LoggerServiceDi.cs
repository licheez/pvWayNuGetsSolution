using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PvWay.LoggerService.Abstractions.nc6;

public static class LoggerServiceDi
{
    public static IServiceCollection AddLoggerServiceConfig(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.Configure<PvWayLoggerServiceConfig>(
            config.GetSection(PvWayLoggerServiceConfig.Section));
    
        return services;
    }
}