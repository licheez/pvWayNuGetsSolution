using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.MsSql.nc6;

public static class PvWayMsSqlLogger
{
    public static IMsSqlLoggerService Create(
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
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
        return new MsSqlLoggerService(
            new LoggerServiceConfig(minLogLevel), 
            new MsSqlLogWriter(
            new MsSqlConnectionStringProvider(getCsAsync), 
            new MsSqlLogWriterConfig(
                schemaName, tableName, 
                userIdColumnName, companyIdColumnName, 
                machineNameColumnName, severityCodeColumnName, 
                contextColumnName, topicColumnName, 
                messageColumnName, createDateUtcColumnName)));
    }

    public static IMsSqlLoggerService<T> Create<T>(
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
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
        return new MsSqlLoggerService<T>(
            new LoggerServiceConfig(minLogLevel), 
            new MsSqlLogWriter(
            new MsSqlConnectionStringProvider(getCsAsync), 
            new MsSqlLogWriterConfig(
                schemaName, tableName, 
                userIdColumnName, companyIdColumnName, 
                machineNameColumnName, severityCodeColumnName, 
                contextColumnName, topicColumnName, 
                messageColumnName, createDateUtcColumnName)));
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

        services.AddSingleton<IMsSqlLogWriter>(_ => 
            new MsSqlLogWriter(
                new MsSqlConnectionStringProvider(getCsAsync),
                new MsSqlLogWriterConfig(lwConfig)));

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
        
        var sd3 = new ServiceDescriptor(
            typeof(ISqlLoggerService),
            typeof(MsSqlLoggerService),
            lifetime);
        services.Add(sd3);
    }
}