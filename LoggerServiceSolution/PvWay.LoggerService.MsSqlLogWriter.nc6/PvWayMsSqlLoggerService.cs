using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedMember.Global

namespace PvWay.LoggerService.MsSqlLogWriter.nc6;

public static class PvWayMsSqlLoggerService
{
    /// <summary>
    /// This will check that the table is present
    /// into the provided Db and check that the table
    /// complies (column types and length).
    /// Checking the table only occurs once. i.e. The first
    /// time the FactorLoggerService is called.
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
    /// <returns>A transient LoggerService</returns>
    public static IPvWayMsSqlLogWriter FactorLogWriter(
        Func<Task<string>> getConnectionStringAsync,
        string tableName = "AppLog",
        string schemaName = "dbo",
        string userIdColumnName = "UserId",
        string companyIdColumnName = "CompanyId",
        string machineNameColumnName = "MachineName",
        string severityCodeColumnName = "SeverityCode",
        string contextColumnName = "Context",
        string topicColumnName = "Topic",
        string messageColumnName = "Message",
        string createDateColumnName = "CreateDateUtc")
    {
        return MsSqlLogWriter
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
    /// time the FactorLoggerService is called.
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
    /// <returns>A transient LoggerService</returns>
    public static IPvWayMsSqlLoggerService FactorLoggerService(
        Func<Task<string>> getConnectionStringAsync,
        string tableName = "AppLog",
        string schemaName = "dbo",
        string userIdColumnName = "UserId",
        string companyIdColumnName = "CompanyId",
        string machineNameColumnName = "MachineName",
        string severityCodeColumnName = "SeverityCode",
        string contextColumnName = "Context",
        string topicColumnName = "Topic",
        string messageColumnName = "Message",
        string createDateColumnName = "CreateDateUtc")
    {
        return MsSqlLogWriter
            .FactorLoggerService(getConnectionStringAsync,
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
    /// time the FactorLoggerService is called.
    /// </summary>
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
    /// <param name="services"></param>
    /// <returns>A transient LoggerService</returns>
    public static void AddPvWayMsSqlLogServices(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        Func<Task<string>> getConnectionStringAsync,
        string tableName = "AppLog",
        string schemaName = "dbo",
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
            typeof(IPvWayMsSqlLogWriter),
            _ => MsSqlLogWriter
                .FactorLogWriter(getConnectionStringAsync,
                    tableName, schemaName,
                    userIdColumnName, companyIdColumnName,
                    machineNameColumnName, severityCodeColumnName,
                    contextColumnName, topicColumnName,
                    messageColumnName, createDateColumnName),
            lifetime);
        services.Add(lwSd);

        var lsSd = new ServiceDescriptor(
            typeof(IPvWayMsSqlLoggerService),
            _ =>
            {
                var lw = MsSqlLogWriter
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