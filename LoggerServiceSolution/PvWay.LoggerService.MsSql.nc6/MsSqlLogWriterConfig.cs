using Microsoft.Extensions.Configuration;
using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.MsSql.nc6;

public class MsSqlLogWriterConfig : ISqlLogWriterConfig
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

    public MsSqlLogWriterConfig(IConfiguration? config):
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

    public MsSqlLogWriterConfig(string? schemaName = "dbo",
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
        SchemaName = schemaName ?? "dbo";
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