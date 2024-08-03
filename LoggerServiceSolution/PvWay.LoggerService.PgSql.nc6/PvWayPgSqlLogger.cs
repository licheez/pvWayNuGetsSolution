using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.PgSql.nc6;

public static class PvWayPgSqlLogger
{
    public static IPgSqlLoggerService Create(
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
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
        return new PgSqlLoggerService(
            new LoggerServiceConfig(minLogLevel), 
            new PgSqlLogWriter(
            new PgSqlConnectionStringProvider(getCsAsync), 
            new PgSqlLogWriterConfig(
                schemaName, tableName, 
                userIdColumnName, companyIdColumnName, 
                machineNameColumnName, severityCodeColumnName, 
                contextColumnName, topicColumnName, 
                messageColumnName, createDateUtcColumnName)));
    }

    public static IPgSqlLoggerService<T> Create<T>(
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
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
        return new PgSqlLoggerService<T>(
            new LoggerServiceConfig(minLogLevel), 
            new PgSqlLogWriter(
            new PgSqlConnectionStringProvider(getCsAsync), 
            new PgSqlLogWriterConfig(
                schemaName, tableName, 
                userIdColumnName, companyIdColumnName, 
                machineNameColumnName, severityCodeColumnName, 
                contextColumnName, topicColumnName, 
                messageColumnName, createDateUtcColumnName)));
    }
    
    
    public static void AddPvWayPgSqlLoggerService(
        this IServiceCollection services,
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        IConfiguration? lwConfig = null,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));

        services.AddSingleton<IPgSqlLogWriter>(_ =>
            new PgSqlLogWriter(
                new PgSqlConnectionStringProvider(getCsAsync),
                new PgSqlLogWriterConfig(lwConfig)));

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
        
        var sd3 = new ServiceDescriptor(
            typeof(ISqlLoggerService),
            typeof(PgSqlLoggerService),
            lifetime);
        services.Add(sd3);
        
    }
}