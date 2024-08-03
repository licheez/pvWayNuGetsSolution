using Microsoft.Extensions.Configuration;
using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.PgSql.nc6;

public class PgSqlLogWriterConfig : ISqlLogWriterConfig
{
    public string SchemaName { get; }
    public string TableName { get; }
    public string UserIdColumnName { get; }
    public string CompanyIdColumnName { get; }
    public string MachineNameColumnName { get; }
    public string SeverityCodeColumnName { get; }
    public string ContextColumnName { get; }
    public string TopicColumnName { get; }
    public string MessageColumnName { get; }
    public string CreateDateUtcColumnName { get; }

    public PgSqlLogWriterConfig(IConfiguration? config):
        this(
            config?["schemaName"],
            config?["tableName"],
            config?["userIdColumnName"],
            config?["companyIdColumnName"],
            config?["machineNameColumnName"],
            config?["severityCodeColumnName"],
            config?["contextColumnName"],
            config?["topicColumnName"],
            config?["messageColumnName"],
            config?["createDateUtcColumnName"])
    {
    }

    public PgSqlLogWriterConfig(string? schemaName = "public",
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
        SchemaName = schemaName ?? "public";
        TableName = tableName ?? "Log";
        UserIdColumnName = userIdColumnName ?? "UserId";
        CompanyIdColumnName = companyIdColumnName ?? "CompanyId";
        MachineNameColumnName = machineNameColumnName ?? "MachineName";
        SeverityCodeColumnName = severityCodeColumnName ?? "SeverityCode";
        ContextColumnName = contextColumnName ?? "Context";
        TopicColumnName = topicColumnName ?? "Topic";
        MessageColumnName = messageColumnName ?? "Message";
        CreateDateUtcColumnName = createDateUtcColumnName ?? "CreateDateUtc";
    }
}