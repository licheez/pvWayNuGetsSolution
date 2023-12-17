using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PvWay.SemaphoreService.Abstractions.nc8;

namespace PvWay.MsSqlSemaphoreService.nc8.Di;

public static class PvWayMsSqlSemaphoreService
{
    public static ISemaphoreService Create(
        string schemaName,
        string tableName,
        Func<Task<string>> getCsAsync,
        Action<Exception>? logException,
        Action<string>? logInfo)
    {
        var config = new SemaphoreServiceConfig(
            schemaName, tableName, getCsAsync,
            logException, logInfo);
        return new SemaphoreService(config);
    }
    
    public static void AddPvWayMsSqlSemaphoreService(
        this IServiceCollection services,
        IConfiguration config,
        Func<Task<string>> getCsAsync,
        Action<Exception>? logException,
        Action<string>? logInfo)
    {
        services.AddSingleton<ISemaphoreServiceConfig>(_ =>
            new SemaphoreServiceConfig(
                config, getCsAsync, logException, logInfo));
        services.AddTransient<ISemaphoreService, SemaphoreService>();
    }
}