using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.MsSql.nc6;

public static class PvWayMsSqlLogger
{
    // CREATORS
    public static IMsSqlLogWriter CreateWriter(
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        string? schemaName = "dbo",
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
        return new MsSqlLogWriter(
            new MsSqlConnectionStringProvider(getCsAsync),
            new MsSqlLogWriterConfig(
                schemaName, tableName,
                userIdColumnName, companyIdColumnName,
                machineNameColumnName, severityCodeColumnName,
                contextColumnName, topicColumnName,
                messageColumnName, createDateUtcColumnName));
    }

    public static IMsSqlLoggerService CreateService(
        IMsSqlLogWriter logWriter,
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        var config = new LoggerServiceConfig(minLogLevel);
        return new MsSqlLoggerService(
            config, logWriter);
    }

    public static IMsSqlLoggerService<T> CreateService<T>(
        IMsSqlLogWriter logWriter,
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        var config = new LoggerServiceConfig(minLogLevel);
        return new MsSqlLoggerService<T>(
            config, logWriter);
    }

    // LOG WRITER
    public static void AddPvWayMsSqlLogWriter(
        this IServiceCollection services,
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        IConfiguration? lwConfig = null)
    {
        var csp = new MsSqlConnectionStringProvider(getCsAsync);
        var cfg = new MsSqlLogWriterConfig(lwConfig);
        var logWriter = new MsSqlLogWriter(csp, cfg);
        services.TryAddSingleton<IMsSqlLogWriter>(_ => logWriter);
        services.TryAddSingleton<ISqlLogWriter>(_ => logWriter);
    }
    
    public static void AddPvWayMsSqlLogWriter(
        this IServiceCollection services,
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        string? schemaName = "dbo",
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
        var csp = new MsSqlConnectionStringProvider(getCsAsync);
        var cfg = new MsSqlLogWriterConfig(
            schemaName, tableName,
            userIdColumnName, companyIdColumnName,
            machineNameColumnName, severityCodeColumnName,
            contextColumnName, topicColumnName,
            messageColumnName, createDateUtcColumnName);
        var logWriter = new MsSqlLogWriter(csp, cfg);
        services.TryAddSingleton<IMsSqlLogWriter>(_ => logWriter);
        services.TryAddSingleton<ISqlLogWriter>(_ => logWriter);
    }
   
    // FACTORY
    public static void AddPvWayMsSqlLoggerServiceFactory(
        this IServiceCollection services,
        IConfiguration config,
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        services.AddSingleton<ILoggerServiceFactory<IMsSqlLoggerService>>(_ =>
            new MsSqlLoggerServiceFactory(getCsAsync, config, minLogLevel));
    }
    
    // SERVICE
    /// <summary>
    /// Use this injector if you already injected the IMsSqlLogWriter
    /// </summary>
    /// <param name="services"></param>
    /// <param name="minLogLevel"></param>
    /// <param name="lifetime"></param>
    public static void AddPvWayMsSqlLoggerService(
        this IServiceCollection services,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));

        RegisterService(services, lifetime);
    }
    
    public static void AddPvWayMsSqlLoggerService(
        this IServiceCollection services,
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Singleton,
        IConfiguration? lwConfig = null)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));
        
        services.AddPvWayMsSqlLogWriter(getCsAsync, lwConfig);
        
        RegisterService(services, lifetime);
    }

    public static void AddPvWayMsSqlLoggerService(
        this IServiceCollection services,
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Singleton,
        string? schemaName = "dbo",
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
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));
        
        services.AddPvWayMsSqlLogWriter(getCsAsync, 
            schemaName, tableName,
            userIdColumnName, companyIdColumnName,
            machineNameColumnName, severityCodeColumnName,
            contextColumnName, topicColumnName,
            messageColumnName, createDateUtcColumnName);
        
        RegisterService(services, lifetime);
    }

    
    private static void RegisterService(
        IServiceCollection services, ServiceLifetime lifetime)
    {
        var descriptors = new List<ServiceDescriptor>
        {
            new ServiceDescriptor(typeof(IMsSqlLoggerService),
                typeof(MsSqlLoggerService),
                lifetime),
            new ServiceDescriptor(typeof(ISqlLoggerService),
                typeof(MsSqlLoggerService),
                lifetime),
            new ServiceDescriptor(typeof(IMsSqlLoggerService<>),
                typeof(MsSqlLoggerService<>),
                lifetime),
            new ServiceDescriptor(typeof(ISqlLoggerService<>),
                typeof(MsSqlLoggerService<>),
                lifetime),
        };
        foreach (var sd in descriptors)
        {
            services.TryAdd(sd);
        }
    }

}