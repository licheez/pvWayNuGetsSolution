using Microsoft.Extensions.Configuration;
using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.LoggerService.PgSql.nc8;

public class PgSqlLogWriterConfig(IConfiguration? config) : ISqlLogWriterConfig
{
    public string SchemaName { get; } = config?["schemaName"] ?? "public";
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