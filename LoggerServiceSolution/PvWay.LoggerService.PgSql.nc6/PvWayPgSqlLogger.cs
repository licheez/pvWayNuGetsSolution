using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.PgSql.nc6;

public static class PvWayPgSqlLogger
{
    // CREATORS
    public static IPgSqlLogWriter CreateWriter(
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        string? schemaName = "public",
        string? tableName = "Log",
        string? userIdColumnName = "UserId",
        string? companyIdColumnName = "CompanyId",
        string? machineNameColumnName = "MachineName",
        string? severityCodeColumnName = "SeverityCode",
        string? contextColumnName = "Context",
        string? topicColumnName = "Topic",
        string? messageColumnName = "Message",
        string? createDateUtcColumnName = "CreateDateUtc")
    {
        return new PgSqlLogWriter(
            new PgSqlConnectionStringProvider(getCsAsync),
            new PgSqlLogWriterConfig(
                schemaName, tableName,
                userIdColumnName, companyIdColumnName,
                machineNameColumnName, severityCodeColumnName,
                contextColumnName, topicColumnName,
                messageColumnName, createDateUtcColumnName));
    }
    
    public static IPgSqlLoggerService CreateService(
        IPgSqlLogWriter logWriter,
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        var config = new LoggerServiceConfig(minLogLevel);
        return new PgSqlLoggerService(config, logWriter);
    }

    public static IPgSqlLoggerService<T> CreateService<T>(
        IPgSqlLogWriter logWriter,
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        var config = new LoggerServiceConfig(minLogLevel);
        return new PgSqlLoggerService<T>(config, logWriter);
    }
    
    // LOG WRITER
    public static void AddPvWayPgSqlLogWriter(
        this IServiceCollection services,
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        IConfiguration? lwConfig = null)
    {
        var csp = new PgSqlConnectionStringProvider(getCsAsync);
        var cfg = new PgSqlLogWriterConfig(lwConfig);
        var logWriter = new PgSqlLogWriter(csp, cfg); 
        services.TryAddSingleton<IPgSqlLogWriter>(_ => logWriter);
        services.TryAddSingleton<ISqlLogWriter>(_ => logWriter);
    }
    
    // FACTORY
    public static void AddPvWayPgSqlLoggerServiceFactory(
        this IServiceCollection services,
        IConfiguration config,
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        services.TryAddSingleton<ILoggerServiceFactory<IPgSqlLoggerService>>(
            _ =>
                new PgSqlLoggerServiceFactory(getCsAsync, config, minLogLevel));
    }
    
   // SERVICES
   /// <summary>
   /// Use this injector if you already injected the IPgSqlLogWriter
   /// </summary>
   /// <param name="services"></param>
   /// <param name="minLogLevel"></param>
   /// <param name="lifetime"></param>
   public static void AddPvWayPgSqlLoggerService(
       this IServiceCollection services,
       SeverityEnu minLogLevel = SeverityEnu.Trace,
       ServiceLifetime lifetime = ServiceLifetime.Singleton)
   {
       services.TryAddSingleton<ILoggerServiceConfig>(_ =>
           new LoggerServiceConfig(minLogLevel));
       
       RegisterService(services, lifetime);
        
       var sd = new ServiceDescriptor(
           typeof(IPgSqlLoggerService<>),
           typeof(PgSqlLoggerService<>),
           lifetime);
       services.Add(sd);
   }
   
    public static void AddPvWayPgSqlLoggerService(
        this IServiceCollection services,
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        IConfiguration? lwConfig = null,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));
        
        services.AddPvWayPgSqlLogWriter(getCsAsync, lwConfig);
        
        RegisterService(services, lifetime);
    }
    
    private static void RegisterService(
        IServiceCollection services, ServiceLifetime lifetime)
    {
        var descriptors = new List<ServiceDescriptor>
        {
            new ServiceDescriptor(typeof(IMsSqlLoggerService),
                typeof(PgSqlLoggerService),
                lifetime),
            new ServiceDescriptor(typeof(ISqlLoggerService),
                typeof(PgSqlLoggerService),
                lifetime),
            new ServiceDescriptor(typeof(IMsSqlLoggerService<>),
                typeof(PgSqlLoggerService<>),
                lifetime),
            new ServiceDescriptor(typeof(ISqlLoggerService<>),
                typeof(PgSqlLoggerService<>),
                lifetime),
        };
        foreach (var sd in descriptors)
        {
            services.TryAdd(sd);
        }
    }
    
    
}