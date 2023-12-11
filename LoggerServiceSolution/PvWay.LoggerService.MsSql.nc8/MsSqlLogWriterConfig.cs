using Microsoft.Extensions.Configuration;

namespace PvWay.LoggerService.MsSql.nc8;

public class MsSqlLogWriterConfig(IConfiguration? config) : IMsSqlLogWriterConfig
{
    public string SchemaName { get; } = config?["schemaName"] ?? "dbo";
    public string TableName { get; } = config?["tableName"] ?? "Log";
    public string UserIdColumnName { get; } = config?["userIdColumnName"] ?? "UserId";
    public string CompanyIdColumnName { get; } = config?["companyIdColumnName"] ?? "CompanyId";
    public string MachineNameColumnName { get; } = config?["machineNameColumnName"] ?? "MachineName";
    public string SeverityCodeColumnName { get; } = config?["severityCodeColumnName"] ?? "SeverityCode";
    public string ContextColumnName { get; } = config?["contextColumnName"] ?? "Context";
    public string TopicColumnName { get; } = config?["topicColumnName"] ?? "Topic";
    public string MessageColumnName { get; } = config?["messageColumnName"] ?? "Message";
    public string CreateDateUtcColumnName { get; } = config?["createDateUtcColumnName"] ?? "CreateDateUtc";
}