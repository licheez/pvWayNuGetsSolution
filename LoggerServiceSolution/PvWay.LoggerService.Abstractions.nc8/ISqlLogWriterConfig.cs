namespace PvWay.LoggerService.Abstractions.nc8;

public interface ISqlLogWriterConfig
{
    string SchemaName { get; }
    string TableName { get; }
    string UserIdColumnName { get; }
    string CompanyIdColumnName { get; }
    string MachineNameColumnName { get; }
    string SeverityCodeColumnName { get; }
    string ContextColumnName { get; }
    string TopicColumnName { get; }
    string MessageColumnName { get; }
    string CreateDateUtcColumnName { get; }
}