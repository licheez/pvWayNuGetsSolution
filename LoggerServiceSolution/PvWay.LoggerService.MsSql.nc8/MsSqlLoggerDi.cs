using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.MsSql.nc8;

public static class MsSqlLoggerDi
{
    public static void AddPvWayMsSqlLoggerService(
        this IServiceCollection services,
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        IConfiguration? lwConfig = null,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.TryAddSingleton<IConnectionStringProvider>(_ =>
            new MsSqlConnectionStringProvider(getCsAsync));
        
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));

        services.AddSingleton<ISqlLogWriterConfig>(_ =>
            new MsSqlLogWriterConfig(lwConfig));

        services.AddSingleton<IMsSqlLogWriter, MsSqlLogWriter>();

        var sd = new ServiceDescriptor(
            typeof(ILoggerService),
            typeof(MsSqlLoggerService),
            lifetime);
        services.Add(sd);
        
        var sd2 = new ServiceDescriptor(
            typeof(IMsSqlLoggerService),
            typeof(MsSqlLoggerService),
            lifetime);
        services.Add(sd2);
    }
}