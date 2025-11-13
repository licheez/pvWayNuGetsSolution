using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.SeriConsole.nc6;

public static class PvWayLoggerServiceSeriConsoleDi
{
    public static IServiceCollection TryAddPvWayLoggerServiceSeriWriter(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddLoggerServiceConfig(config);
        services.TryAddSingleton<ILogWriter, SerilogConsoleWriter>();
        services.TryAddSingleton<IConsoleLogWriter, SerilogConsoleWriter>();
        
        return services;
    }

    public static IServiceCollection TryAddPvWayLoggerServiceSeriService(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.TryAddPvWayLoggerServiceSeriWriter(config);
        
        services.TryAddSingleton<ILoggerService, SerilogConsoleService>();
        services.TryAddSingleton<IConsoleLoggerService, SerilogConsoleService>();
        services.TryAddSingleton<ISeriConsoleLoggerService, SerilogConsoleService>();
        
        services.TryAddSingleton<ILoggerProvider, SerilogConsoleLoggerProvider>();
        services.TryAddSingleton<IConsoleLoggerProvider, SerilogConsoleLoggerProvider>();
        
        return services;
    }
    
}