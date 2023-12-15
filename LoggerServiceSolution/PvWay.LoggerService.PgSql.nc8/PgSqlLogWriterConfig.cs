using Microsoft.Extensions.Configuration;
using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.LoggerService.PgSql.nc8;

public class PgSqlLogWriterConfig(
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
    : ISqlLogWriterConfig
{
    public string SchemaName { get; } = schemaName ?? "public";
    public string TableName { get; } = tableName ?? "Log";
    public string UserIdColumnName { get; } = userIdColumnName ?? "UserId";
    public string CompanyIdColumnName { get; } = companyIdColumnName ?? "CompanyId";
    public string MachineNameColumnName { get; } = machineNameColumnName ?? "MachineName";
    public string SeverityCodeColumnName { get; } = severityCodeColumnName ?? "SeverityCode";
    public string ContextColumnName { get; } = contextColumnName ?? "Context";
    public string TopicColumnName { get; } = topicColumnName ?? "Topic";
    public string MessageColumnName { get; } = messageColumnName ?? "Message";
    public string CreateDateUtcColumnName { get; } = createDateUtcColumnName ?? "CreateDateUtc";

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
}