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
        services.TryAddSingleton<IMsSqlLogWriter>(_ =>
            new MsSqlLogWriter(
                new MsSqlConnectionStringProvider(getCsAsync),
                new MsSqlLogWriterConfig(lwConfig)));
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
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));
        
        var sd = new ServiceDescriptor(
            typeof(IMsSqlLoggerService<>),
            typeof(MsSqlLoggerService<>),
            lifetime);
        services.Add(sd);
    }

    public static void AddPvWayMsSqlLoggerService(
        this IServiceCollection services,
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        IConfiguration? lwConfig = null,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));

        services.TryAddSingleton<IMsSqlLogWriter>(_ =>
            new MsSqlLogWriter(
                new MsSqlConnectionStringProvider(getCsAsync),
                new MsSqlLogWriterConfig(lwConfig)));

        var sd = new ServiceDescriptor(
            typeof(IMsSqlLoggerService<>),
            typeof(MsSqlLoggerService<>),
            lifetime);
        services.Add(sd);
    }
}