using Microsoft.Extensions.DependencyInjection;
using PvWay.LoggerService.Abstractions.nc6;

// ReSharper disable UnusedMember.Global
namespace PvWay.LoggerService.PgSqlLogWriter.nc6;

public static class PvWayPgLoggerService
{
    /// <summary>
    /// This will check that the table is present
    /// into the provided Db and check that the table
    /// complies (column types and length).
    /// Checking the table only occurs once. i.e. The first
    /// time the Service is called.
    /// </summary>
    /// <param name="getConnectionStringAsync"></param>
    /// <param name="tableName"></param>
    /// <param name="schemaName"></param>
    /// <param name="userIdColumnName"></param>
    /// <param name="companyIdColumnName"></param>
    /// <param name="machineNameColumnName"></param>
    /// <param name="severityCodeColumnName"></param>
    /// <param name="contextColumnName"></param>
    /// <param name="topicColumnName"></param>
    /// <param name="messageColumnName"></param>
    /// <param name="createDateColumnName"></param>
    /// <returns>PvWay.LoggerService.Abstractions.nc6.ILogWriter</returns>
    public static ILogWriter CreateLogWriter(
        Func<Task<string>> getConnectionStringAsync,
        string tableName = "AppLog",
        string schemaName = "public",
        string userIdColumnName = "UserId",
        string companyIdColumnName = "CompanyId",
        string machineNameColumnName = "MachineName",
        string severityCodeColumnName = "SeverityCode",
        string contextColumnName = "Context",
        string topicColumnName = "Topic",
        string messageColumnName = "Message",
        string createDateColumnName = "CreateDateUtc")
    {
        return PgSqlLogWriter
            .FactorLogWriter(getConnectionStringAsync,
                tableName, schemaName,
                userIdColumnName, companyIdColumnName,
                machineNameColumnName, severityCodeColumnName,
                contextColumnName, topicColumnName,
                messageColumnName, createDateColumnName);
    }

    /// <summary>
    /// This will check that the table is present
    /// into the provided Db and check that the table
    /// complies (column types and length).
    /// Checking the table only occurs once. i.e. The first
    /// time the Service is called.
    /// </summary>
    /// <param name="getConnectionStringAsync"></param>
    /// <param name="tableName"></param>
    /// <param name="schemaName"></param>
    /// <param name="userIdColumnName"></param>
    /// <param name="companyIdColumnName"></param>
    /// <param name="machineNameColumnName"></param>
    /// <param name="severityCodeColumnName"></param>
    /// <param name="contextColumnName"></param>
    /// <param name="topicColumnName"></param>
    /// <param name="messageColumnName"></param>
    /// <param name="createDateColumnName"></param>
    /// <returns>PvWay.LoggerService.Abstractions.nc6.ILoggerService</returns>
    public static ILoggerService CreateLoggerService(
        Func<Task<string>> getConnectionStringAsync,
        string tableName = "AppLog",
        string schemaName = "public",
        string userIdColumnName = "UserId",
        string companyIdColumnName = "CompanyId",
        string machineNameColumnName = "MachineName",
        string severityCodeColumnName = "SeverityCode",
        string contextColumnName = "Context",
        string topicColumnName = "Topic",
        string messageColumnName = "Message",
        string createDateColumnName = "CreateDateUtc")
    {
        return PgSqlLogWriter
            .FactorLoggerService(getConnectionStringAsync,
                tableName, schemaName,
                userIdColumnName, companyIdColumnName,
                machineNameColumnName, severityCodeColumnName,
                contextColumnName, topicColumnName,
                messageColumnName, createDateColumnName);
    }


    /// <summary>
    /// Adds both the IPvWayPostgreLogWriter and
    /// IPvWayPostgreLoggerService with the provided lifeTime
    /// </summary>
    /// <param name="services"></param>
    /// <param name="lifetime"></param>
    /// <param name="getConnectionStringAsync"></param>
    /// <param name="tableName"></param>
    /// <param name="schemaName"></param>
    /// <param name="userIdColumnName"></param>
    /// <param name="companyIdColumnName"></param>
    /// <param name="machineNameColumnName"></param>
    /// <param name="severityCodeColumnName"></param>
    /// <param name="contextColumnName"></param>
    /// <param name="topicColumnName"></param>
    /// <param name="messageColumnName"></param>
    /// <param name="createDateColumnName"></param>
    public static void AddPvWayPgLogServices(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        Func<Task<string>> getConnectionStringAsync,
        string tableName = "AppLog",
        string schemaName = "public",
        string userIdColumnName = "UserId",
        string companyIdColumnName = "CompanyId",
        string machineNameColumnName = "MachineName",
        string severityCodeColumnName = "SeverityCode",
        string contextColumnName = "Context",
        string topicColumnName = "Topic",
        string messageColumnName = "Message",
        string createDateColumnName = "CreateDateUtc")
    {
        var lwSd = new ServiceDescriptor(
            typeof(IPvWayPostgreLogWriter),
            _ => PgSqlLogWriter
                .FactorLogWriter(getConnectionStringAsync,
                    tableName, schemaName,
                    userIdColumnName, companyIdColumnName,
                    machineNameColumnName, severityCodeColumnName,
                    contextColumnName, topicColumnName,
                    messageColumnName, createDateColumnName),
            lifetime);
        services.Add(lwSd);

        var lsSd = new ServiceDescriptor(
            typeof(IPvWayPostgreLoggerService),
            _ =>
            {
                var lw = PgSqlLogWriter
                    .FactorLoggerService(getConnectionStringAsync,
                        tableName, schemaName,
                        userIdColumnName, companyIdColumnName,
                        machineNameColumnName, severityCodeColumnName,
                        contextColumnName, topicColumnName,
                        messageColumnName, createDateColumnName);
                return lw;
            },
            lifetime);
        services.Add(lsSd);
    }

}