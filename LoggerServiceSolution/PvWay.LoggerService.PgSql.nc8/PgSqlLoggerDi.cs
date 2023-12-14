using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.PgSql.nc8;

public static class PgSqlLoggerDi
{
    public static void AddPvWayPgSqlLoggerService(
        this IServiceCollection services,
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        IConfiguration? lwConfig = null,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.TryAddSingleton<IConnectionStringProvider>(_ =>
            new PgSqlConnectionStringProvider(getCsAsync));
        
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));

        services.AddSingleton<ISqlLogWriterConfig>(_ =>
            new PgSqlLogWriterConfig(lwConfig));

        services.AddSingleton<IPgSqlLogWriter, PgSqlLogWriter>();

        var sd = new ServiceDescriptor(
            typeof(ILoggerService),
            typeof(PgSqlLoggerService),
            lifetime);
        services.Add(sd);
        
        var sd2 = new ServiceDescriptor(
            typeof(IPgSqlLoggerService),
            typeof(PgSqlLoggerService),
            lifetime);
        services.Add(sd2);
    }
}