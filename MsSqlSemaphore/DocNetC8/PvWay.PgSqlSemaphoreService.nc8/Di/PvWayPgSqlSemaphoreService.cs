using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.PgSqlSemaphoreService.nc8.Services;
using PvWay.SemaphoreService.Abstractions.nc8;

namespace PvWay.PgSqlSemaphoreService.nc8.Di;

public static class PvWayPgSqlSemaphoreService
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
        return new Services.SemaphoreService(config);
    }
    
    public static void AddPvWayPgSqlSemaphoreService(
        this IServiceCollection services,
        string schemaName,
        string tableName,
        Func<Task<string>> getCsAsync,
        Action<Exception>? logException,
        Action<string>? logInfo)
    {
        services.TryAddSingleton<ISemaphoreServiceConfig>(_ =>
            new SemaphoreServiceConfig(
                schemaName, tableName, 
                getCsAsync, 
                logException, logInfo));
        services.AddTransient<
            ISemaphoreService, 
            Services.SemaphoreService>();
    }
}